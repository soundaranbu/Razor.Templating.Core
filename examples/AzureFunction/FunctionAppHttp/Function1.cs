using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Razor.Templating.Core;

namespace FunctionAppHttp
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            var renderedString = await RazorTemplateEngine.RenderAsync("/Views/Home/Index.cshtml");
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult(renderedString);
        }
    }
}
