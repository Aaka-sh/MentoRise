using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//This adds MVC and Web API controllers
builder.Services.AddControllers();
//adding the DataContext class (of type DbContext) in the dependency injection container
//to configure SQLite database with a connection string from app configuration
builder.Services.AddDbContext<DataContext>(opt => {
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//in case the CORS policy blocks the request
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
//allow the request if the request is originated from Angular application running on 4200
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
    .WithOrigins("http://localhost:4200","https://localhost:4200"));
app.MapControllers();
app.Run();
