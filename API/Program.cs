using API.Data;
using API.Extensions;
using API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration); //application service extension
builder.Services.AddIdentityServices(builder.Configuration); //Identity Service Extension

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>(); //Exception Middleware
//allow the request if the request is originated from Angular application running on 4200
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope(); //create a scope for the application
var services = scope.ServiceProvider; //get the service provider from the scope
try
{
    var context = services.GetRequiredService<DataContext>(); //get the data context from the service provider
    await context.Database.MigrateAsync(); //migrate the database
    await Seed.SeedUsers(context); //seed the users
}
catch (DbUpdateException ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>(); //get the logger from the service provider
    logger.LogError(ex, "An error occurred during migration"); //log the error
}
app.Run();
