//role of this file
//implementation of the IMessageRepository interface
//provides the actual logic for adding, retrieving, and deleting messages from the database

using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace API.Data;

public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
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

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username),
            _ => query.Where(x => x.Recipient.UserName == messageParams.Username && x.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
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
