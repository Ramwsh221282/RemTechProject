using RemTechAvito.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpLogging();
builder.Services.RegisterServices();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors(config =>
{
    config
        .WithOrigins("http://localhost:5174", "http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod();
});

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseHttpLogging();

app.Run();
