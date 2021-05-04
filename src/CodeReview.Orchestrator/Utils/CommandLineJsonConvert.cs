using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;
using Newtonsoft.Json;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public class CommandLineJsonConvert : ICommandLineJsonConvert
    {
        public string Serialize(params Type[] type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));

            return JsonConvert.SerializeObject(
                new { Commands =  type.Select(SerializeType) },
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        private static object SerializeType(Type type)
        {
            var verbAttribute = GetVerbAttribute(type);
            var optionAttributes = GetOptionAttributes(type);

            return new
            {
                Name = verbAttribute.Name,
                IsDefault = verbAttribute.IsDefault,
                Description = verbAttribute.HelpText,
                Parameters = optionAttributes.Select(option => new
                {
                    LongName = option.LongName,
                    ShortName = option.ShortName,
                    IsOptional = option.Required,
                    Description = option.HelpText,
                    DefaultValue = option.Default?.ToString()
                })
            };
        }

        private static IEnumerable<OptionAttribute> GetOptionAttributes(Type type)
        {
            return type
                .GetProperties()
                .Select(p => p.GetCustomAttribute<OptionAttribute>());
        }
        
        private static VerbAttribute GetVerbAttribute(Type type)
        {
            return type.GetCustomAttribute<VerbAttribute>();
        }
    }
}