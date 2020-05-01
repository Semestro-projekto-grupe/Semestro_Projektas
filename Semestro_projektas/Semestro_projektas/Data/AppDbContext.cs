using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Semestro_projektas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {

        public DbSet<Post> Posts { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelUser> ChannelUsers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        //rysiu kurimui
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ChannelUser>().HasKey(c => new { c.UserId, c.ChannelId });

            

           modelBuilder.Entity<ChannelUser>()
        .HasOne(c => c.User)
        .WithMany(t => t.channelUsers)
        .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<ChannelUser>()
        .HasOne(c => c.Channel)
        .WithMany(t => t.channelUsers) // If you add `public ICollection<UserBook> UserBooks { get; set; }` navigation property to Book model class then replace `.WithMany()` with `.WithMany(b => b.UserBooks)`
        .HasForeignKey(c => c.ChannelId);


        }

    }
}
