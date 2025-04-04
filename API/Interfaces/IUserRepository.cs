using System;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync(); //when we save our changes to the database, return true if successful
    Task<IEnumerable<AppUser>> GetUsersAsync(); //get all users
    Task<AppUser?> GetUserByIdAsync(int id); //get user by id
    Task<AppUser?> GetUserByUsernameAsync(string username); //get user by username
    Task<IEnumerable<MemberDto>> GetMembersAsync(); //get all members
    Task<MemberDto?> GetMemberAsync(string username); //get member by username
}
