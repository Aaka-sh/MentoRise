//role of this file
//DataContext file acts as a bridge between the application and the database

using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    //DbSet<T> represents a table in the database and a collection of entities
    public DbSet<AppUser> Users { get; set; } //user table
    public DbSet<UserLike> Likes { get; set; } //likes table
    public DbSet<Message> Messages { get; set; } //messages table

    //OnModelCreating method configures relationships and constraints between the entities
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //setting a composite primary key using SourceUserId and TargetUserId
        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        //one to many relationship between AppUser and UserLike
        //one user can like many other users
        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Restrict);

        //one to many relationship between AppUser and UserLike
        //one user can be liked by many users
        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.NoAction);

        //one to many relationship between AppUser and Message
        //one user can send many messages
        builder.Entity<Message>()
            .HasOne(x => x.Recipient)
            .WithMany(x => x.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        //one to many relationship between AppUser and Message
        //one user can receive many messages
        builder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(x => x.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
