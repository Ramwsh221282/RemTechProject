using RemTech.MainApi.ParsersManagement.DependencyInjection;
using RemTech.MainApi.ParsersManagement.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

builder.Services.RegisterParserDependencies();

var app = builder.Build();
app.MapParserEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
