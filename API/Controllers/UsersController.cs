using System;
using System.Security.Claims;
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
public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseAPIController
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


    //update user details
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        //fetch the user from the repository using the GetUserByUsernameAsync method
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //if when the user is not found, return 404 NotFound 
        if (username == null) return BadRequest("No username found in token");
        var user = await userRepository.GetUserByUsernameAsync(username);
        if (user == null) return BadRequest("User not found");
        //else update the user details using AutoMapper
        mapper.Map(memberUpdateDto, user);
        //save changes to the database
        if (await userRepository.SaveAllAsync()) return NoContent(); //returns 204 No Content if successful
        return BadRequest("Failed to update user"); //returns 400 Bad Request if failed
    }
}
