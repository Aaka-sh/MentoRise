using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize] //ensures that only authenticated users can access the endpoints in this controller
public class UsersController(IUserRepository userRepository, IMapper mapper,
 IPhotoService photoService) : BaseAPIController
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
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("User not found");
        //else update the user details using AutoMapper
        mapper.Map(memberUpdateDto, user);
        //save changes to the database
        if (await userRepository.SaveAllAsync()) return NoContent(); //returns 204 No Content if successful
        return BadRequest("Failed to update user"); //returns 400 Bad Request if failed
    }

    [HttpPost("add-photo")] //endpoint for adding a photo

    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file) //
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        if (user == null) return BadRequest("Cannot update user");
        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null) return BadRequest(result.Error.Message);
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        user.Photos.Add(photo);
        if (await userRepository.SaveAllAsync())
            return mapper.Map<PhotoDto>(photo);
        return BadRequest("Problem adding photo");
    }
}
