using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;
using GodelTech.CodeReview.Orchestrator.Utils;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerVolumeImporter : IDockerVolumeImporter
    {
        private static readonly ByteSize MaxInMemoryArchiveSize = ByteSize.FromMegabytes(300);

        private readonly ITempFileFactory _tempFileFactory;
        private readonly IContainerService _containerService;
        private readonly IDirectoryService _directoryService;
        private readonly ITarArchiveService _tarArchiveService;
        private readonly IDockerEngineContext _dockerEngineContext;

        public DockerVolumeImporter(
            ITempFileFactory tempFileFactory,
            IContainerService containerService,
            IDirectoryService directoryService,
            ITarArchiveService tarArchiveService,
            IDockerEngineContext dockerEngineContext)
        {
            _tempFileFactory = tempFileFactory ?? throw new ArgumentNullException(nameof(tempFileFactory));
            _containerService = containerService ?? throw new ArgumentNullException(nameof(containerService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _tarArchiveService = tarArchiveService ?? throw new ArgumentNullException(nameof(tarArchiveService));
            _dockerEngineContext = dockerEngineContext ?? throw new ArgumentNullException(nameof(dockerEngineContext));
        }

        public async Task ImportVolumesAsync(IProcessingContext context, params Volume[] volumes)
        {
            var containerId = await CreateWorkerContainerAsync(context, volumes);

            try
            {
                foreach (var volume in volumes)
                {
                    var folderPath = context.ResolvePath(volume.FolderToImport);

                    if (!_directoryService.Exists(folderPath))
                        continue;

                    Stream outStream;
                    using var _ = _directoryService.GetDirectorySize(folderPath) <= MaxInMemoryArchiveSize
                        ? CreateArchiveInMemory(folderPath, out outStream)
                        : CreateArchiveInFileSystem(folderPath, out outStream);

                    await _containerService.ImportFilesIntoContainerAsync(containerId, volume.TargetFolder, outStream);
                }
            }
            finally
            {
                await _containerService.RemoveContainerAsync(containerId);
            }
        }

        private IDisposable CreateArchiveInMemory(string path, out Stream stream)
        {
            stream = _tarArchiveService.CreateInMemory(path);

            return Disposable.CreateDisposableAction(stream, static toDispose => { toDispose.Dispose(); });
        }

        private IDisposable CreateArchiveInFileSystem(string path, out Stream stream)
        {
            var tmpFile = _tempFileFactory.Create();
            stream = _tarArchiveService.CreateInFile(path, tmpFile.Path);

            return Disposable.CreateDisposableAction((stream, tmpFile), static toDispose =>
            {
                toDispose.stream.Dispose();
                toDispose.tmpFile.Dispose();
            });
        }

        private async Task<string> CreateWorkerContainerAsync(IProcessingContext context, ICollection<Volume> volumes)
        {
            var mountedVolumes = ResolveMountedVolumes(context, volumes).ToArray();

            return await _containerService.CreateContainerAsync(_dockerEngineContext.Engine.WorkerImage, mountedVolumes);
        }

        private static IEnumerable<MountedVolume> ResolveMountedVolumes(IProcessingContext context, ICollection<Volume> volumes)
        {
            return volumes.Select(volume => new MountedVolume
            {
                IsBindMount = false,
                Source = context.ResolveVolumeId(volume.Name),
                Target = volume.TargetFolder
            });
        }
    }
}