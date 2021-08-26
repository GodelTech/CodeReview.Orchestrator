using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Activities;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class DockerContextSwitcher : IDockerContextSwitcher
    {
        private readonly IPathService _pathService;
        private readonly IDockerEngineContext _engineContext;
        private readonly IDockerEngineProvider _engineProvider;
        private readonly ITempFolderFactory _tempFolderFactory;
        private readonly IDockerVolumeExporter _dockerVolumeExporter;
        private readonly IDockerVolumeImporter _dockerVolumeImporter;
        
        public DockerContextSwitcher(
            IPathService pathService, 
            IDockerEngineContext engineContext, 
            IDockerEngineProvider engineProvider, 
            ITempFolderFactory tempFolderFactory,
            IDockerVolumeExporter dockerVolumeExporter, 
            IDockerVolumeImporter dockerVolumeImporter)
        {
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _engineContext = engineContext ?? throw new ArgumentNullException(nameof(engineContext));
            _engineProvider = engineProvider ?? throw new ArgumentNullException(nameof(engineProvider));
            _tempFolderFactory = tempFolderFactory ?? throw new ArgumentNullException(nameof(tempFolderFactory));
            _dockerVolumeExporter = dockerVolumeExporter ?? throw new ArgumentNullException(nameof(dockerVolumeExporter));
            _dockerVolumeImporter = dockerVolumeImporter ?? throw new ArgumentNullException(nameof(dockerVolumeImporter));
        }

        public async Task SwitchAsync(IProcessingContext context, RequirementsManifest requirements)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (requirements == null)
                throw new ArgumentNullException(nameof(requirements));

            var matchingEngine = _engineProvider.Find(requirements);

            if (_engineContext.Engine == null)
            {
                _engineContext.Engine = matchingEngine;
                return;
            }

            if (_engineContext.Engine.Equals(matchingEngine))
                return;

            await SwitchVolumesAsync(context, matchingEngine);
        }

        private async Task SwitchVolumesAsync(IProcessingContext context, DockerEngine newEngine)
        {
            using var folder = _tempFolderFactory.Create();

            var volumes = CloneVolumes(context).ToList();
            
            var volumesToExport = GetVolumesToExport(context, folder.Path);
            await _dockerVolumeExporter.ExportVolumesAsync(context, volumesToExport);

            await context.CleanUpVolumesAsync();

            _engineContext.Engine = newEngine;

            await context.InitializeAsync(volumes);

            var volumesToImport = GetVolumesToImport(context, folder.Path);
            await _dockerVolumeImporter.ImportVolumesAsync(context, volumesToImport);
        }

        private Volume[] GetVolumesToImport(IProcessingContext context, string folder) =>
            CloneVolumes(context)
                .Select(volume =>
                {
                    volume.TargetFolder = GetFolderForOs(volume, _engineContext.Engine.Os);
                    volume.FolderToImport = _pathService.Combine(folder, volume.Name);

                    return volume;
                })
                .ToArray();

        private Volume[] GetVolumesToExport(IProcessingContext context, string folder) =>
            CloneVolumes(context)
                .Select(volume =>
                {
                    volume.TargetFolder = GetFolderForOs(volume, _engineContext.Engine.Os);
                    volume.FolderToOutput = _pathService.Combine(folder, volume.Name);

                    return volume;
                })
                .ToArray();

        private static IEnumerable<Volume> CloneVolumes(IProcessingContext context) =>
            context.Volumes
                .Select(volume => volume.Clone())
                .Cast<Volume>();


        private static string GetFolderForOs(Volume volume, OperatingSystemType os)
        {
            return os == OperatingSystemType.Linux
                ? $"/{volume.Name}"
                : $"C:\\{volume.Name}";
        }
    }
}