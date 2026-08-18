using RedFast.API.Middlewares;
using RedFast.Modules.Core;
using RedFast.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCoreModule(builder.Configuration);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddOpenApi();


builder.Services.AddExceptionHandler<GlobalValidationExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseStatusCodePages();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapCoreEndpoints();

app.Run();
