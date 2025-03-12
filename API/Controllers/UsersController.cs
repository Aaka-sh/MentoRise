using System;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] //when on localhost://5001/api/user, will redirect to UserController
public class UsersController(DataContext context): ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<AppUser>> GetUsers()
    {
        var users = context.Users.ToList();
        return users;
    }

    [HttpGet("{id:int}")] //to get an individual user
    public ActionResult<AppUser> GetUser(int id)
    {
        var user = context.Users.Find(id);
        //if the user is null return NotFound
        if(user == null) return NotFound();
        //user can not be null here
        return user;
    }
}
