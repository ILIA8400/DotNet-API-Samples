using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Filters
{
    public class ActionLogger : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public ActionLogger(ILogger<ActionLogger> logger)
        {
            this._logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionName = context.ActionDescriptor.DisplayName;

            var inputs = context.ActionArguments
                                 .Select(arg => $"{arg.Key}: {arg.Value}")
                                 .ToList();

            _logger.LogInformation("{action} action was called with the inputs of {inputs}", actionName, string.Join(", ", inputs));

            await next();
        }
    }
}
