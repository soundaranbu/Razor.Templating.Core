using Razor.Templating.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorTemplating();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/render", async (RazorTemplateEngine engine) =>
{
    //Render View From the RCL
    var renderedString = await engine.RenderAsync("/Views/Home/Index.cshtml");
    return renderedString;
})
.WithName("render");

app.Run();
