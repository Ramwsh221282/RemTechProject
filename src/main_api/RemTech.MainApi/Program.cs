global using RemTechCommon.Utils.CqrsPattern;
using RemTech.MainApi.Common.Extensions;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(conf =>
{
    conf.AddPolicy(
        "frontend",
        p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().Build()
    );
});
builder.Services.AddOpenApi();
builder.Services.RegisterServices();
var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapAllEndpoints();
app.Run();
