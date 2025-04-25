using System;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next(); // Call the next filter or action

        //checking if the user is authenticated
        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;
        var userId = resultContext.HttpContext.User.GetUserId(); // Get the username from the token
        var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await repo.GetUserByIdAsync(userId); // Fetch the user from the database
        if (user == null) return; // If the user is not found, do nothing
        user.LastActive = DateTime.UtcNow; // Update the LastActive property to the current UTC time
        await repo.SaveAllAsync(); // Save the changes to the database
    }
}
