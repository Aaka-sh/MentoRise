//role of the file
//this file allows us to interact with the Likes table in the database
//this repository class allows us to like and unlike users
//it also helps us to retrieve who a user liked, who liked the user, or mutual likes
using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

//injecting the DataContext and IMapper classes into the LikesRepository class
public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
{
    //adding a new UserLike entity to the Likes table in the database
    public void AddLike(UserLike like)
    {
        context.Likes.Add(like);
    }
    //removing a UserLike entity from the Likes table in the database
    public void DeleteLike(UserLike like)
    {
        context.Likes.Remove(like);
    }
    //getting the ids of all the users that the current user has liked
    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await context.Likes
            .Where(x => x.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }
    //finding a specific "like" relationship between two users
    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    //getting a list of users that the current user has liked or has been liked by, based on the predicate parameter
    public async Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId)
    {
        var likes = context.Likes.AsQueryable();
        switch (predicate)
        {
            case "liked":
                return await likes
                    .Where(x => x.SourceUserId == userId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();

            case "likedBy":
                return await likes
                    .Where(x => x.TargetUserId == userId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
            default:
                var likeIds = await GetCurrentUserLikeIds(userId);
                return await likes
                    .Where(x => x.TargetUserId == userId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
        }
    }

    //saving changes to the database asynchronously and returning true indicating success or failure
    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }

}