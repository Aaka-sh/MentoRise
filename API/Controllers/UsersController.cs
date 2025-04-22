using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        userParams.CurrentUsername = User.GetUsername(); //get the current username from the token
        var users = await userRepository.GetMemberAsync(userParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
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

        if (user.Photos.Count == 0) photo.IsMain = true; //set the first photo as main

        user.Photos.Add(photo);
        if (await userRepository.SaveAllAsync())
            return CreatedAtAction(nameof(GetUser),
            new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")] //endpoint for setting a photo as the main photo
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername()); //fetch the user by username using the userRepository
        if (user == null) return BadRequest("Could not find user"); //returns 400 Bad Request if user not found
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);//fetch the photo by id using LINQ
        if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");//returns 400 Bad Request if photo not found or already main
        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);//fetch the current main photo using LINQ
        if (currentMain != null) currentMain.IsMain = false; //set the current main photo to not main
        photo.IsMain = true; //set the new main photo to main

        if (await userRepository.SaveAllAsync()) return NoContent(); //returns 204 No Content if successful
        return BadRequest("Failed to set main photo"); //returns 400 Bad Request if failed
    }

    [HttpDelete("delete-photo/{photoId:int}")] //endpoint for deleting a photo
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(User.GetUsername()); //fetch the user by username using the userRepository
        if (user == null) return BadRequest("Could not find user"); //returns 400 Bad Request if user not found
        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId); //fetch the photo by id using LINQ
        if (photo == null || photo.IsMain) return BadRequest("This photo can not be deleted"); //returns 404 Not Found if photo not found
        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId); //delete the photo from the cloud using the photoService
            if (result.Error != null) return BadRequest(result.Error.Message); //returns 400 Bad Request if failed to delete photo
        }
        user.Photos.Remove(photo);
        if (await userRepository.SaveAllAsync()) return Ok();  //returns 200 OK if successful
        return BadRequest("Failed to delete the photo"); //returns 400 Bad Request if failed to delete photo
    }
}
