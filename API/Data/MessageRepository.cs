//role of this file
//implementation of the IMessageRepository interface
//provides the actual logic for adding, retrieving, and deleting messages from the database

using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Data;

public class MessageRepository(DataContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        // Add the message to the context
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        // Remove the message from the context
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(int id)
    {
        // Find the message by its ID asynchronously
        return await context.Messages.FindAsync(id);
    }

    public Task<PagedList<MessageDto>> GetMessagesForUser()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        // Save all changes to the context asynchronously
        return await context.SaveChangesAsync() > 0;
    }
}
