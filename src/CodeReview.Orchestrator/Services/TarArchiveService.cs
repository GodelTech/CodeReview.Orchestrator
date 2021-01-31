using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class TarArchiveService : ITarArchiveService
    {
        private readonly IDirectoryService _directoryService;
        private readonly IOutputFolderPathCalculator _outputFolderPathCalculator;

        public TarArchiveService(
            IDirectoryService directoryService,
            IOutputFolderPathCalculator outputFolderPathCalculator)
        {
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _outputFolderPathCalculator = outputFolderPathCalculator ?? throw new ArgumentNullException(nameof(outputFolderPathCalculator));
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
    }
}