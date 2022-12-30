using System;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class TempFileFactory : ITempFileFactory
    {
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public TempFileFactory(IPathService pathService, IFileService fileService)
        {
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public TempFile Create()
        {
            var tmpFile = _pathService.GetTempFileName();

            return CreateInternal(tmpFile);
        }

        public TempFile Create(string ext)
        {
            var oldPath = _pathService.GetTempFileName();
            var newPath = _pathService.ChangeExtensions(oldPath, ext);

            _fileService.Move(oldPath, newPath);

            return CreateInternal(newPath);
        }

        private TempFile CreateInternal(string path)
        {
            return new TempFile(path, () =>
            {
                if (_fileService.Exists(path))
                    _fileService.Delete(path);
            });
        }
    }
}