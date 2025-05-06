//role of this utility class
//it is a static helper class that contains extension methods for IServiceCollection
//it centralizes the registration of services, repositories, database context and other configuraions

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
        services.AddControllers(); //Registers controllers used for MVC/Web API.
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        }); //Adds the DataContext (your EF Core DbContext) to Dependency Injection (DI) container.
        services.AddCors();//in case the CORS policy blocks the request

        //services.AddScoped() helps to create one instance per HTTP request
        services.AddScoped<ITokenService, TokenService>();  //registering the ITokenService class as a scoped service
        services.AddScoped<IUserRepository, UserRepository>(); //registering the IUserRepository class as a scoped service
        services.AddScoped<ILikesRepository, LikesRepository>(); //registering the ILikesRepository class as a scoped service
        services.AddScoped<IMessageRepository, MessageRepository>(); //registering the IMessageRepository class as a scoped service
        services.AddScoped<IPhotoService, PhotoService>(); //registering the IPhotoService class as a scoped service
        services.AddScoped<LogUserActivity>(); //Adds a service/filter that tracks the user’s last active time
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); //Registers AutoMapper for mapping between Entities and DTOs
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings")); //Configures Cloudinary settings from appsettings.json
        return services;
    }
}
