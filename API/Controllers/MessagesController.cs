//role of this file
//The purpose of the MessagesController file is to handle all message-related API requests in your ASP.NET Core application.

using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController(IMessageRepository messageRepository,
    IUserRepository userRepository, IMapper mapper) : BaseAPIController
{
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
}
