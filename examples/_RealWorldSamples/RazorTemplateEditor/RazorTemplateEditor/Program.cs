using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using Razor.Templating.Core.Dynamic;
using Razor.Templating.Core.Dynamic.Data;
using RazorTemplateEditor.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<TestDatabaseContext>(o => o.UseInMemoryDatabase("TestDb"));
builder.Services.AddSingleton<DbRazorViewProvider>();
builder.Services.AddMvcCore().AddRazorRuntimeCompilation();
builder.Services.AddOptions<MvcRazorRuntimeCompilationOptions>()
    .Configure<DbRazorViewProvider>((option, source) =>
    {
        option.FileProviders.Add(source);
    });

builder.Services.AddRazorTemplating();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
