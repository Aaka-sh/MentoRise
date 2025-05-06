//role of this file
//this DTO helps to receive user input for sending a message
//it helps to protect the data model and simplify input validation

namespace API.DTOs;

public class CreateMessageDto
{
    public required string RecipientUsername { get; set; } //who is receiving the message
    public required string Content { get; set; } //the content of the message
}
