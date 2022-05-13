using EmailService.Templates.WelcomeEmail;
using FluentEmail.Core;
using Razor.Templating.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services
        .AddFluentEmail("fromemail@test.test")
        .AddSmtpSender("localhost", 25);

var app = builder.Build();

app.MapGet("/", async () =>
{
    var model = new WelcomeEmailModel
    {
        Name = "John",
        Email = "john@example.com",
        CompanyName = "Razor.Templating.Core",
        SenderName = "Soundar"
    };

    var body = await RazorTemplateEngine.RenderAsync("/WelcomeEmail/WelcomeEmailTemplate.cshtml", model);

    var email = await Email
                        .From("soundaranbu@gmail.com", model.SenderName)
                        .To(model.Email, model.Name)
                        .Subject($"Welcome to {model.CompanyName}")
                        .Body(body)
                        .SendAsync();

    return "Email sent successfully";
});

app.Run();