using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace WebApi.Filters
{
    public class MyExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger<MyExceptionFilter> _logger;

        public MyExceptionFilter(ILogger<MyExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async override Task OnExceptionAsync(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred: {Message}", context.Exception.Message);

            var result = new
            {
                Error = "An unexpected error occurred.",
                Detail = context.Exception.Message,
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(result);

            context.ExceptionHandled = true;

            _logger.LogInformation("Exception has been handled and response has been set for action: {Action}", context.ActionDescriptor.DisplayName);
            await Task.CompletedTask;
        }

    }
    
}
