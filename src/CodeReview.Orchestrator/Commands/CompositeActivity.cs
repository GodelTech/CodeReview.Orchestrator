using System;
using System.Threading.Tasks;

namespace CodeReview.Orchestrator.Commands
{
    public class CompositeActivity : IActivity
    {
        private readonly IActivity[] _activities;

        public CompositeActivity(params IActivity[] activities)
        {
            _activities = activities ?? throw new ArgumentNullException(nameof(activities));
        }

        public async Task<bool> ExecuteAsync(IProcessingContext context)
        {
            if (context == null) 
                throw new ArgumentNullException(nameof(context));

            var isSuccess = true;

            foreach (var activity in _activities)
            {
                var isCommandExecutedSuccessfully = await activity.ExecuteAsync(context);
                if (isCommandExecutedSuccessfully) 
                    continue;

                isSuccess = false;
                break;
            }

            return isSuccess;
        }
    }
}