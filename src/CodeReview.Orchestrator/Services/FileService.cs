using System;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class FileService : IFileService
    {
        public void Delete(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            File.Delete(path);
        }

        public Stream Open(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            return File.Open(path, FileMode.Open);
        }

        public void Move(string sourceFileName, string destFileName)
        {
            if (string.IsNullOrWhiteSpace(sourceFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(sourceFileName));
            if (string.IsNullOrWhiteSpace(destFileName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(destFileName));
            
            File.Move(sourceFileName, destFileName);
        }

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