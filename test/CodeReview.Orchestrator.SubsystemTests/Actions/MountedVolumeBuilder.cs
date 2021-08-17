using System;
using CodeReview.Orchestrator.SubsystemTests.Models;

namespace CodeReview.Orchestrator.SubsystemTests.Actions
{
    public class MountedVolumeBuilder
    {
        private readonly MountedVolume _mountedVolume = new();

        public MountedVolumeBuilder WithType(string type)
        {
            _mountedVolume.Type = type ?? throw new ArgumentNullException(nameof(type));

            return this;
        }

        public MountedVolumeBuilder WithSource(string source)
        {
            _mountedVolume.Source = source ?? throw new ArgumentNullException(nameof(source));

            return this;
        }

        public MountedVolumeBuilder WithTarget(string target)
        {
            _mountedVolume.Target = target ?? throw new ArgumentNullException(nameof(target));

            return this;
        }

        public MountedVolumeBuilder WithReadOnly(bool isReadOnly)
        {
            _mountedVolume.ReadOnly = isReadOnly;

            return this;
        }

        public MountedVolume Build() => _mountedVolume;
    }
}