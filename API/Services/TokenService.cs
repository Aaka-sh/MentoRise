using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

//TokenService implements ITokenService and uses IConfiguration as a dependency
//IConfiguration provides a secret key used for signing JWTs
public class TokenService(IConfiguration config) : ITokenService
{
    //this service is used with dependency injection
    public string CreateToken(AppUser user)
    {
        //the secret key used to sign in the token is retrieved from config files
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access tokenkey from app settings");
        //longer key strengthens security 
        if (tokenKey.Length < 64) throw new Exception("Your token needs to be longer");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        //claims store information about a user inside the token
        var claims = new List<Claim>
        {
            //Name identifier is used to uniquely identify the user using their username
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //token properties
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //attaches claims
            Subject = new ClaimsIdentity(claims),
            //sets an expiration date: 7 days
            Expires = DateTime.UtcNow.AddDays(7),
            //Uses HMAC signature for validation
            SigningCredentials = creds
        };

        //generating and writing the security
        var tokenHandler = new JwtSecurityTokenHandler();
        //creating JWT token with the provided claims, expiration and credentials
        var token = tokenHandler.CreateToken(tokenDescriptor);
        //returning the final JWT as a string
        return tokenHandler.WriteToken(token);
    }
}
