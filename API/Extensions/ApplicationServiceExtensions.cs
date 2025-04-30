using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

//extension methods are static
//static methods allow us to use the methods inside the class without creating an object
public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
    IConfiguration config)
    {
        //This adds MVC and Web API controllers
        //adding the DataContext class (of type DbContext) in the dependency injection container
        //to configure SQLite database with a connection string from app configuration
        services.AddControllers();
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        //in case the CORS policy blocks the request
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILikesRepository, LikesRepository>(); //registering the ILikesRepository class as a scoped service
        services.AddScoped<IPhotoService, PhotoService>(); //registering the IPhotoService class as a scoped service
        services.AddScoped<LogUserActivity>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        return services;
    }
}
