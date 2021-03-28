using System;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class GuidFactory : IGuidFactory
    {
        public string CreateAsString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}