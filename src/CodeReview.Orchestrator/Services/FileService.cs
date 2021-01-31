using System;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class FileService : IFileService
    {
        public Task<string> ReadAllTextAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            return File.ReadAllTextAsync(path);
        }

        public Task WriteAllTextAsync(string path, string text)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(text));

            return File.WriteAllTextAsync(path, text);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }
    }
}
