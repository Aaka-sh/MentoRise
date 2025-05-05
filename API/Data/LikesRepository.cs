//role of the file
//this file allows us to interact with the Likes table in the database
//this repository class allows us to like and unlike users
//it also helps us to retrieve who a user liked, who liked the user, or mutual likes
using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
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
    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = context.Likes.AsQueryable();
        IQueryable<MemberDto> query;
        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(x => x.SourceUserId == likesParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            case "likedBy":
                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);
                query = likes
                    .Where(x => x.TargetUserId == likesParams.UserId && likeIds.Contains(x.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;
        }
        return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    //saving changes to the database asynchronously and returning true indicating success or failure
    public async Task<bool> SaveChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }

}