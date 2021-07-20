using System;
using System.ComponentModel.DataAnnotations;
using YamlDotNet.Serialization;

namespace GodelTech.CodeReview.Orchestrator.Model
{
    public class Volume : ICloneable
    {
        [YamlIgnore] public string Name { get; set; }

        public bool ReadOnly { get; set; } = true;

        [MaxLength(Constants.MaxPathLength)] public string TargetFolder { get; set; }

        [MaxLength(Constants.MaxPathLength)] public string FolderToOutput { get; set; }

        [MaxLength(Constants.MaxPathLength)] public string FolderToImport { get; set; }
        
        public object Clone() => MemberwiseClone();
    }
}