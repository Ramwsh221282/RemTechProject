using RemTech.Infrastructure;
using RemTech.Infrastructure.PostgreSql;
using RemTech.Shared.SDK.DependencyInjection;
using RemTech.WebApi.Configuration.Endpoints;
using RemTech.WebApi.Extras;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(conf =>
{
    conf.AddPolicy(
        "frontend",
        p => p.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().Build()
    );
});

builder.Services.AddOpenApi();
builder.Services.InjectPostgres(Constants.PostgreSqlFilePath);
builder.Services.InjectMarkedServices();
builder.Services.AddInitialParsersInDb();
WebApplication app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.MapEndpoints();
app.Run();
