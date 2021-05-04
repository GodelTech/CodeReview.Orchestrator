using System;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public interface ICommandLineJsonConvert
    {
        string Serialize(params Type[] type);
    }
}