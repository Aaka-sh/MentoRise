using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

//account controller is deriving BaseAPIController (created to avoid repetition)
//DataContext service is injected into the AccountController class
public class AccountController(DataContext context, ITokenService tokenService, IMapper mapper) : BaseAPIController
{
    [HttpPost("register")] //url for this endpoint: account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerdto)
    {

        if (await UserExists(registerdto.Username)) return BadRequest("Username is taken");

        //HMACSHA512 class is used for secure hashing
        //using var ensures automatic disposal of the cryptographic object
        //this is used for securely hashing passowrds along with unique salt
        using var hmac = new HMACSHA512();
        var user = mapper.Map<AppUser>(registerdto);
        user.UserName = registerdto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x =>
            x.UserName == logindto.Username.ToLower());

        if (user == null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        //this will check if the values are equal keeping case sensitivity in mind
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
