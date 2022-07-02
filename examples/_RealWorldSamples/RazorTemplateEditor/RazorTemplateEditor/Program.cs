using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.EntityFrameworkCore;
using RazorTemplateEditor.Data;
using RazorTemplateEditor.RazorProvider;

namespace RazorTemplateEditor;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        builder.Services.AddDbContextFactory<TestDatabaseContext>(opt => opt.UseInMemoryDatabase("test"));
        builder.Services.AddTransient<DbRazorViewProvider>();

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
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}