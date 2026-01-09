using Razor.Templating.Core;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/render", async () =>
{
    //Render View From the RCL
    var renderedString = await RazorTemplateEngine.RenderAsync("/Views/Home/Index.cshtml");
    return renderedString;
})
.WithName("render");

app.Run();
