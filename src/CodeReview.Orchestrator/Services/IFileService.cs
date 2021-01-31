﻿using System.Threading.Tasks;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public interface IFileService
    {
        Task<string> ReadAllTextAsync(string path);
        Task WriteAllTextAsync(string path, string text);
        bool Exists(string path);
    }
}