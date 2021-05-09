using System;
using System.Collections.Generic;
using System.Reflection;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public interface ICommandLineOptionsTypeProvider
    {
        ICollection<Type> GetOptions(Assembly assembly);
    }
}