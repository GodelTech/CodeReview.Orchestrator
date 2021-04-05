using System;

namespace CodeReview.Orchestrator.SubsystemTests.Actions.Docker
{
    public class StubDockerImportSourceActivity : StubDockerImportActivity<StubDockerImportSourceActivity>
    {
        private string _sourceId;
        
        protected override object Mounts 
        {
            get
            {
                return new[]
                {
                    new
                    {
                        Type = "volume",
                        Source = _sourceId,
                        Target = "/src"
                    }
                };
            }
        }

        public StubDockerImportSourceActivity WithSourceId(string sourceId)
        {
            _sourceId = sourceId ?? throw new ArgumentNullException(nameof(sourceId));

            return this;
        }
    }
}