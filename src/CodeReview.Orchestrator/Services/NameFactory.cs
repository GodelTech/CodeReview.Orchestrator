using System;

namespace CodeReview.Orchestrator.Services
{
    public class NameFactory : INameFactory
    {
        public string CreateVolumeName(string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(prefix));

            return prefix + "-" + Guid.NewGuid();
        }
    }
}