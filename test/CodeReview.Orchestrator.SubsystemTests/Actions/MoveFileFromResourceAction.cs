using CodeReview.Orchestrator.SubsystemTests.Utils;
using GodelTech.StoryLine.Contracts;

namespace CodeReview.Orchestrator.SubsystemTests.Actions
{
    public class MoveFileFromResourceAction : IAction
    {
        public string ResourceName { private get; init; }
        public string FilePath { private get; init; }
        
        public void Execute(IActor actor)
        {
            FileHelper.CopyFromResource(ResourceName, FilePath);
        }
    }
}