//role of this file
//The purpose of the MessagesController file is to handle all message-related API requests in your ASP.NET Core application.

using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController(IMessageRepository messageRepository,
    IUserRepository userRepository, IMapper mapper) : BaseAPIController
{
    //Create function creates a new message and saves it to the database.
    [HttpPost] // POST: api/messages
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var username = User.GetUsername(); //get the currently logged in user's username
        if (username == createMessageDto.RecipientUsername.ToLower()) //preventing to send messages to yourself
            return BadRequest("You cannot send messages to yourself");
        var sender = await userRepository.GetUserByUsernameAsync(username); //fetching the sender's details
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername); //fetching the recipient's details
        if (sender == null || recipient == null) return BadRequest("Can not send messages at this time");

        var message = new Message //creating a new message entity
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        messageRepository.AddMessage(message); //adding the message to the repository
        if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message)); //save to the database and return the message DTO
        return BadRequest("Failed to send message"); //if saving fails, return an error message
    }

    //GetMessagesForUser function returns a filtered and paginated list of messages for the current user based on the specified message parameters.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
        [FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.GetUsername(); //setting the username for the message params to current user's username so the user sees only their messages
        var messages = await messageRepository.GetMessagesForUser(messageParams); //fetching messages for the user based on the message params
        Response.AddPaginationHeader(messages); //adding pagination header to the response
        return messages; //returning the messages
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        var currentUsername = User.GetUsername(); //get the currently logged in user's username
        return Ok(await messageRepository.GetMessageThread(currentUsername, username)); //fetching the message thread between the current user and the specified user         
    }
}
