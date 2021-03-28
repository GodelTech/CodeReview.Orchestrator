using System;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class TempFolderFactory : ITempFolderFactory
    {
        private readonly IPathService _pathService;
        private readonly IDirectoryService _directoryService;
        private readonly IGuidFactory _guidFactory;

        public TempFolderFactory(
            IPathService pathService, 
            IDirectoryService directoryService,
            IGuidFactory guidFactory
        )
        {
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _guidFactory = guidFactory ?? throw new ArgumentNullException(nameof(guidFactory));
        }
        
        public TempFolder Create()
        {
            var userTempFolderPath = _pathService.GetTempPath();
            var tempFolder = _pathService.Combine(userTempFolderPath, _guidFactory.CreateAsString());

            _directoryService.CreateDirectory(tempFolder);
            
            return new TempFolder(tempFolder, () =>
            {
                if (_directoryService.Exists(tempFolder))
                    _directoryService.Delete(tempFolder, true);
            });
        }
    }
}