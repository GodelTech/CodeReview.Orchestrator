using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CodeReview.Orchestrator.Model;
using Microsoft.Extensions.Logging;

namespace CodeReview.Orchestrator.Services
{
    public class ManifestValidator : IManifestValidator
    {
        private readonly ILogger<ManifestValidator> _logger;

        public ManifestValidator(ILogger<ManifestValidator> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public bool IsValid(AnalysisManifest manifest)
        {
            if (manifest == null) 
                throw new ArgumentNullException(nameof(manifest));

            var validationResults = new List<ValidationResult>();

            if (TryValidateObject(manifest, new Dictionary<object, object>(), validationResults))
                return true;

            foreach (var error in validationResults)
            {
                _logger.LogError("{@members}: {errorMessage}", error.MemberNames.ToArray(), error.ErrorMessage);
            }

            // TODO: Validate templates used by script
            
            return false;
        }

        public bool TryValidateObject(object obj, Dictionary<object, object> items, List<ValidationResult> validationResults)
        {
            var validationContext = new ValidationContext(obj, items);

            validationResults ??= new List<ValidationResult>();
            var result = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            if (obj == null)
                return result;


            var properties = obj.GetType()
                .GetProperties()
                .Where(prop => prop.CanRead && prop.GetIndexParameters().Length == 0)
                .Where(prop => CanTypeBeValidated(prop.PropertyType))
                .ToList();

            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value == null)
                    continue;

                var valueEnumerable = value as IEnumerable<object> ?? new[] { value };
                var nestedValidationResults = new List<ValidationResult>();
                
                foreach (var valueToValidate in valueEnumerable)
                {
                    if (TryValidateObject(valueToValidate, items, nestedValidationResults)) 
                        continue;
                    
                    result = false;

                    foreach (var validationResult in nestedValidationResults)
                    {
                        validationResults.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => property.Name + '.' + x)));
                    }
                }
            }


            return result;
        }

        private static bool CanTypeBeValidated(Type type)
        {
            if (type == null)
                return false;
            if (type == typeof(string))
                return false;
            if (type.IsValueType)
                return false;

            if (!type.IsArray || !type.HasElementType) 
                return true;
            
            var elementType = type.GetElementType();
            
            return CanTypeBeValidated(elementType);
        }
    }
}
