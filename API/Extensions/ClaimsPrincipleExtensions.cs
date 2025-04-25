using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    //extension method to get the username from the ClaimsPrincipal object
    public static string GetUsername(this ClaimsPrincipal user)
    {
        //throws an exception if the username is not found in the token
        var username = user.FindFirstValue(ClaimTypes.Name)
            ?? throw new Exception("Username not found in token");
        return username;
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        //throws an exception if the username is not found in the token
        var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("Username not found in token"));
        return userId;
    }
}
