using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommandLine;
using Newtonsoft.Json;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public class OptionMetadataProvider : IOptionMetadataProvider
    {
        private readonly ICommandLineOptionsTypeProvider _commandLineOptionsTypeProvider;
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public OptionMetadataProvider(ICommandLineOptionsTypeProvider commandLineOptionsTypeProvider)
        {
            _commandLineOptionsTypeProvider = commandLineOptionsTypeProvider ?? throw new ArgumentNullException(nameof(commandLineOptionsTypeProvider));

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore, 
                Formatting = Formatting.Indented
            };
        }

        public string GetOptionsMetadata()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var optionTypes = _commandLineOptionsTypeProvider.GetOptions(assembly);

            return JsonConvert.SerializeObject(
                new
                {
                    Commands =  optionTypes.Select(SerializeType)
                },
                _jsonSerializerSettings);
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