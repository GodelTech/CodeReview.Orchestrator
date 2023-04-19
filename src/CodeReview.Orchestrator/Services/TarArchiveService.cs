using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class TarArchiveService : ITarArchiveService
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IOutputFolderPathCalculator _outputFolderPathCalculator;

        public TarArchiveService(
            IFileService fileService,
            IDirectoryService directoryService,
            IOutputFolderPathCalculator outputFolderPathCalculator)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _outputFolderPathCalculator = outputFolderPathCalculator ?? throw new ArgumentNullException(nameof(outputFolderPathCalculator));
        }

        public Stream CreateInMemory(string path)
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

        public Stream CreateInFile(string path, string tmpFilePath)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            if (string.IsNullOrWhiteSpace(tmpFilePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(tmpFilePath));

            var stream = _fileService.Open(tmpFilePath);

            using (var writer = WriterFactory.Open(stream, ArchiveType.Tar, CompressionType.None))
            {
                writer.WriteAll(path, "*", SearchOption.AllDirectories);
            }

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
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