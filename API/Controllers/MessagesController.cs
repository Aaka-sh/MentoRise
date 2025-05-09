//role of this file
//The purpose of the MessagesController file is to handle all message-related API requests in your ASP.NET Core application.

using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize] //Authorize attribute ensures that only authenticated users can access the endpoints in this controller.
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername(); //getting the currently logged in user's username
        var message = await messageRepository.GetMessage(id); //fetching the message by id
        if (message == null) return BadRequest("Can not delete this message"); //if message not found, return a bad request
        if (message.SenderUsername != username && message.RecipientUsername != username) //if the message is not sent or received by the current user, return a forbidden response
            return Forbid();
        if (message.SenderUsername == username) message.SenderDeleted = true; //if the message is sent by the current user, mark it as deleted
        if (message.RecipientUsername == username) message.RecipientDeleted = true; //if the message is received by the current user, mark it as deleted
        if (message is { SenderDeleted: true, RecipientDeleted: true }) //if both sender and recipient have deleted the message, remove it from the repository
            messageRepository.DeleteMessage(message);
        if (await messageRepository.SaveAllAsync()) return Ok(); //if saving is successful, return an OK response
        return BadRequest("Failed to delete the message"); //if saving fails, return a bad request
    }
}
