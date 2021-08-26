using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class YamlSerializer : IYamlSerializer
    {
        public T Deserialize<T>(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(content));

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            return deserializer.Deserialize<T>(content);
        }

        public string Serialize(object person)
        {
            if (person == null) 
                throw new ArgumentNullException(nameof(person));
            
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();

            return serializer.Serialize(person);
        }
    }
}
