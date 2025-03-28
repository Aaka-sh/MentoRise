using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        //if any users exist in the database, return
        if (await context.Users.AnyAsync()) return;
        //read the json data from the UserSeedData.json file
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        //create a json serializer options object and set the PropertyNameCaseInsensitive property to true
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        //deserialize the json data into a list of AppUser objects
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        //if the users list is null, return
        if (users == null) return;

        foreach (var user in users)
        {
            using var hmac = new HMACSHA512();
            user.UserName = user.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            user.PasswordSalt = hmac.Key;

            context.Users.Add(user);
        }

    }
}
