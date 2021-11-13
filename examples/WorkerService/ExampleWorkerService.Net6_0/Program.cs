using ExampleWorkerService.Net6_0;
using Razor.Templating.Core;

IHost host = Host.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
        services.AddHostedService<Worker>();
	})
	.Build();

await host.RunAsync();
