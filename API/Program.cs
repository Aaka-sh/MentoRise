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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();
app.Run();
