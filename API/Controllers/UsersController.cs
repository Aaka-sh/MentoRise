using System;
using API.Data; //contains the DataContext class (DbContext)
using API.DTOs;
using API.Entities; //contains the API user entity class
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; //provides functionalities for building API controllers

namespace API.Controllers;

[Authorize] //ensures that only authenticated users can access the endpoints in this controller
public class UsersController(IUserRepository userRepository) : BaseAPIController
{

    //fetch all users
    //[HttpGet]: Maps this method to an HTTP GET request at api/users.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();
        return Ok(users); //returns 200 OK with the list of users
    }

    //fetching a single user
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        //fetch the user by username using the userRepository
        var user = await userRepository.GetMemberAsync(username);
        //if when the user is not found, return 404 NotFound 
        if (user == null) return NotFound();
        //else return the 200 OK HttpResponse
        return user;
    }
}
