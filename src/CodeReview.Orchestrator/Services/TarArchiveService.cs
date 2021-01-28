using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace ReviewItEasy.Orchestrator.Services
{
    public class TarArchiveService : ITarArchiveService
    {
        private readonly IDirectoryService _directoryService;
        private readonly IOutputFolderPathCalculator _outputFolderPathCalculator;
        private readonly IPathService _pathService;

        public TarArchiveService(
            IDirectoryService directoryService,
            IOutputFolderPathCalculator outputFolderPathCalculator,
            IPathService pathService)
        {
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _outputFolderPathCalculator = outputFolderPathCalculator ?? throw new ArgumentNullException(nameof(outputFolderPathCalculator));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }
        
        public Stream Create(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            
            var outStream = new MemoryStream();

            using (var writer = WriterFactory.Open(outStream, ArchiveType.Tar, CompressionType.None))
            {
                writer.WriteAll(path, "*", SearchOption.AllDirectories);
            }
            
            outStream.Position = 0;

            return outStream;
        }

        public void Extract(Stream inStream, string folderPath, string pathToRemove)
        {
            if (inStream == null) 
                throw new ArgumentNullException(nameof(inStream));
            if (folderPath == null) 
                throw new ArgumentNullException(nameof(folderPath));

            pathToRemove ??= string.Empty;

            var reader = ReaderFactory.Open(inStream);
            
            while (reader.MoveToNextEntry())
            {
                if (reader.Entry.IsDirectory) 
                    continue;

                var outputDirectory = _outputFolderPathCalculator.CalculateOutputDirectoryName(
                    folderPath,
                    pathToRemove,
                    reader.Entry.Key);
                
                if (!_directoryService.Exists(outputDirectory))
                    _directoryService.CreateDirectory(outputDirectory);
                
                reader.WriteEntryToDirectory(outputDirectory, new ExtractionOptions
                {
                    ExtractFullPath = false, 
                    Overwrite = true
                });
            }
        }

        private string CalculateOutputDirectoryName(string folderPath, int pathToRemoveLength, string archiveEntryPath)
        {
            var fullPath = _pathService.GetFullPath(folderPath);
            var entryDirectoryName = _pathService.GetDirectoryName(archiveEntryPath);

            // TODO: Path doesn't container / (e.g. artifats/roslyn.zip)
            // as result when /artifacts is removed from artifacts (without slash) issue thrown
            
            return _pathService.Combine(fullPath, entryDirectoryName.Substring(pathToRemoveLength));
        }
    }
}