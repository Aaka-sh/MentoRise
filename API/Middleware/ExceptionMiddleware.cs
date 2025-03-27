//this code defines Exception Middleware 
//it globally handles exceptions in the application
//it logs the exception and returns a response to the client
using System;
using System.Net; //this lib is needed to reference HttpStatusCode
using System.Text.Json; //used for JSON Serialization
using API.Errors; //contains error handling classes

namespace API.Middleware;

// RequestDelegate next: Represents the next middleware in the request pipeline.
// ILogger<ExceptionMiddleware> logger: Logger for logging errors.
// IHostEnvironment env: Provides environment information (e.g., Development, Production).
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message); //logs the error using ILogger
            context.Response.ContentType = "application/json"; //response is set to JSON format
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //if development environment, return the full exception message and stack trace
            //otherwise, return a generic error message (Production)
            var response = env.IsDevelopment()
                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

            //configures the JSON serializer to use camelCase for property names
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            //converts the ApiException object to a JSON string
            var json = JsonSerializer.Serialize(response, options);
            //Writes the JSON response back to the client
            await context.Response.WriteAsync(json);
        }
    }
}
