using WorkerService;
using Razor.Templating.Core;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddRazorTemplating();
var host = builder.Build();
host.Run();
