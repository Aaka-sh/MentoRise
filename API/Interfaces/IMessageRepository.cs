//role of this file
//IMessageRepository tells how messages should be added, retrieved or deleted without exposing the database directly.

using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message); //adds a new message to the database
    void DeleteMessage(Message message); //deletes a message from the database
    Task<Message?> GetMessage(int id); //retrieves a message by its ID
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams); //retrieves all messages for a user
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername); //retrieves a message thread between two users
    Task<bool> SaveAllAsync(); //saves all changes to the database
}
