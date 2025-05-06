// role of this file
// Message entity class which defines the structure of a message object in the application.
// when a message object is created required properties must be initialized
namespace API.Entities;

public class Message
{
    public int Id { get; set; }
    public required string SenderUsername { get; set; }
    public required string RecipientUsername { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow; //Whenever a new message object is created, MessageSent time will be set to the current UTC time.
    public bool SenderDeleted { get; set; }
    public bool RecipientDeleted { get; set; }

    //navigation properties
    public int SenderId { get; set; }
    public AppUser Sender { get; set; } = null!; //null-forgiving operator is used to indicate that the property will be initialized later, avoiding nullability warnings.
    public int RecipientId { get; set; }
    public AppUser Recipient { get; set; } = null!; //null-forgiving operator is used to indicate that the property will be initialized later, avoiding nullability warnings.
}
