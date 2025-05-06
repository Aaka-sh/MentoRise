//role of this file
//MessageDto class is a Data Transfer Object (DTO) that defines the structure of a message object that will be sent to the client.
//It is used to transfer data between the server and the client, ensuring that only the necessary data is sent over the network.
//Instead of returning the full Message, this file returns a clean and flattened version of the data

namespace API.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public required string SenderUsername { get; set; }
    public required string SenderPhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public required string RecipientUsername { get; set; }
    public required string RecipientPhotoUrl { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }


}
