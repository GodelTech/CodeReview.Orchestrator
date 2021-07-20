using System;
using System.Linq;
using GodelTech.CodeReview.Orchestrator.Model;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public class ManifestExpressionExpander : IManifestExpressionExpander
    {
        private readonly IExpressionEvaluator _expressionEvaluator;
        private readonly IVariableExpressionProvider _variableExpressionProvider;

        public ManifestExpressionExpander(IExpressionEvaluator expressionEvaluator,
            IVariableExpressionProvider variableExpressionProvider)
        {
            _expressionEvaluator = expressionEvaluator ?? throw new ArgumentNullException(nameof(expressionEvaluator));
            _variableExpressionProvider = variableExpressionProvider ?? throw new ArgumentNullException(nameof(variableExpressionProvider));
        }
        
        public void Expand(AnalysisManifest manifest)
        {
            if (manifest == null) 
                throw new ArgumentNullException(nameof(manifest));

            _variableExpressionProvider.SetVariables(manifest.Variables);

            foreach (var (_, volume) in manifest.Volumes)
            {
                if (!string.IsNullOrWhiteSpace(volume.TargetFolder))
                    volume.TargetFolder = _expressionEvaluator.Evaluate(volume.TargetFolder);
                
                if (!string.IsNullOrWhiteSpace(volume.FolderToImport))
                    volume.FolderToImport = _expressionEvaluator.Evaluate(volume.FolderToImport);
                
                if (!string.IsNullOrWhiteSpace(volume.FolderToOutput))
                    volume.FolderToOutput = _expressionEvaluator.Evaluate(volume.FolderToOutput);
            }
            
            manifest.Variables = manifest.Variables.ToDictionary(x => x.Key, x => _expressionEvaluator.Evaluate(x.Value), StringComparer.OrdinalIgnoreCase);

            foreach (var activity in manifest.Activities)
            {
                ExpandActivity(activity.Value);
            }
        }

        private void ExpandActivity(ActivityManifest manifest)
        {
            manifest.Image = _expressionEvaluator.Evaluate(manifest.Image);
            manifest.Environment = manifest.Environment.ToDictionary(x => x.Key, x => _expressionEvaluator.Evaluate(x.Value));
            
            if (manifest.Command != null)
                manifest.Command = manifest.Command.Select(x => _expressionEvaluator.Evaluate(x)).ToArray();

            foreach (var (_, volume) in manifest.Volumes)
            {
                if (!string.IsNullOrWhiteSpace(volume.TargetFolder))
                    volume.TargetFolder = _expressionEvaluator.Evaluate(volume.TargetFolder);
            }
        }
    }
}