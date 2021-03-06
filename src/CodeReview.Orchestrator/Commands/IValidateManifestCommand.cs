﻿using System.Threading.Tasks;
using GodelTech.CodeReview.Orchestrator.Options;

namespace GodelTech.CodeReview.Orchestrator.Commands
{
    public interface IValidateManifestCommand
    {
        Task<int> ExecuteAsync(EvaluateOptions options);
    }
}