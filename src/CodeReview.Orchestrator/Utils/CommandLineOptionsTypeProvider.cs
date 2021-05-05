using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public class CommandLineOptionsTypeProvider : ICommandLineOptionsTypeProvider
    {
        public ICollection<Type> GetOptions(Assembly assembly)
        {
            if (assembly is null) throw new ArgumentNullException(nameof(assembly));
            
            return assembly.GetTypes()
                .Where(p => CustomAttributeExtensions.GetCustomAttribute<VerbAttribute>((MemberInfo) p) is not null)
                .ToArray();
        }
    }
}