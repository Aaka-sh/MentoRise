using System;
using API.Data; //contains the DataContext class (DbContext)
using API.Entities; //contains the API user entity class
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //provides functionalities for building API controllers

namespace API.Controllers;

//injecting the DataContext service to allow Database access 
public class UsersController(DataContext context): BaseAPIController
{
    [AllowAnonymous]
    //fetch all users
    //[HttpGet]: Maps this method to an HTTP GET request at api/users.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();//retreives all the users from the Users table
        return users; //returning a list of app user objects
    }


    [Authorize]
    //fetching a single user
    [HttpGet("{id:int}")] 
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        //finding user using id (primary key)
        var user = await context.Users.FindAsync(id);
        //if when the user is not found, return 404 NotFound 
        if(user == null) return NotFound();
        //else return the 200 OK HttpResponse
        return user;
    }
}
