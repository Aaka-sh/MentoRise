using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

//account controller is deriving BaseAPIController (created to avoid repetition)
public class AccountController(DataContext context): BaseAPIController
{
    [HttpPost("register")] //url for this endpoint: account/register
    public async Task<ActionResult<AppUser>> Register(RegisterDto registerdto){

        if(await UserExists(registerdto.Username)) return BadRequest("Username is taken");
        //HMACSHA512 class is used for secure hashing
        //using var ensures automatic disposal of the cryptographic object
        //this is used for securely hashing passowrds along with unique salt
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerdto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    private async Task<bool> UserExists(string username)
    {
        //this will check if the values are equal keeping case sensitivity in mind
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
