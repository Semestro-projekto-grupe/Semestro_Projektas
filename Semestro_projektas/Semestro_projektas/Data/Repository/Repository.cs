using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Semestro_projektas.Models;

namespace Semestro_projektas.Data.Repository
{
    public class Repository : IRepository
    {


        private AppDbContext _ctx;

        public Repository(AppDbContext ctx)
        {
            _ctx = ctx;
        }


        public List<Message> GetChatMessages()
        {
            return _ctx.Messages.ToList();
        }

        public List<Message> GetChatMessagesByChat(int chatId, string user) {
            if (CheckIfChannelExists(user, chatId)){
                return _ctx.Messages.Where(m => m.ChannelId == chatId).ToList();
            }
            else {
                return new List<Message>();
            }
        }

        public void SaveMessage(Message msg)
        {
            if (CheckIfChannelExists(msg.Author, msg.ChannelId))
            {
                _ctx.Messages.Add(msg);
            }
        }

        



        public void EditMessage(int id, string text, string user)
        {
            Message msg = _ctx.Messages.FirstOrDefault(m => m.Id == id);
            if (msg.Author == user)
            {
                msg.Content = text;
                _ctx.Messages.Update(msg);
            }
        }

        public void RemoveMessage(int id)
        {
            _ctx.Messages.Remove(GetMessage(id));
        }

        public Message GetMessage(int id)
        {
            return _ctx.Messages.FirstOrDefault(p => p.Id == id);
        }

        public void AddPost(Post post)
        {
            _ctx.Posts.Add(post);
        }

        public List<Post> GetAllPosts()
        {
            return _ctx.Posts.ToList();
        }

        public Post GetPost(int id)
        {
            return _ctx.Posts.FirstOrDefault(p => p.Id == id);
        }

        public void RemovePost(int id)
        {
            _ctx.Posts.Remove(GetPost(id));
        }

        public void UpdatePost(Post post)
        {
            _ctx.Posts.Update(post);
        }


        public async Task<bool> SaveChangesAsync()
        {

            if (await _ctx.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public bool RegisterUser(User user, string password, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            //var userManager = UserManager<User>;
            //var roleManager = RoleManager<IdentityRole>;

            _ctx.Database.EnsureCreated();

            var userRole = new IdentityRole("ChatUser");
            if (!_ctx.Roles.Any(r => r == userRole))
            {
                //sukurti role
                roleManager.CreateAsync(userRole).GetAwaiter().GetResult();


            }

            if (!_ctx.Users.Any(u => u.UserName == user.UserName))
            {

                var result = userManager.CreateAsync(user, password)
                   .GetAwaiter().GetResult();

                //prideti vartotojui role
                userManager.AddToRoleAsync(user, userRole.Name).GetAwaiter().GetResult();

                return true;

            }
            else
            {

                return false;
            }
        }

        public List<User> GetUsers()
        {
            return _ctx.Users.ToList();
        }


        public void CreateChannel(Channel channel, string userName)
        {
            _ctx.Channels.Add(channel);

            User creator = _ctx.Users.FirstOrDefault(p => p.UserName == userName);
            AddChannelUser(channel, creator);

        }

        public void AddChannelUser(Channel channel, User user)
        {
            ChannelUser channelUser = new ChannelUser();
            channelUser.ChannelId = channel.Id;
            channelUser.Channel = channel;
            channelUser.UserId = user.Id;
            _ctx.ChannelUsers.Add(channelUser);
            user.channelUsers.Add(channelUser);
        }

        public List<Channel> GetUserChannels(string userName)
        {
            // User user = _ctx.Users.FirstOrDefault(p => p.Name == userName);
            var channels = _ctx.Channels.Where(t => t.channelUsers.Any(s => s.User.UserName == userName));
            //foreach (ChannelUser c in user.channelUsers) {
            // channels.Add(c.Channel);
            // }
            return channels.ToList();
        }


        public bool CheckIfChannelExists(string userName, int id)
        {
            var channels = _ctx.Channels.Where(t => t.channelUsers.Any(s => s.User.UserName == userName));
            int index = channels.ToList().FindIndex(f => f.Id == id);
            if (index >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public void AddUserToChannel(string userName, string inviterName, int channelId) {
            if (CheckIfChannelExists(inviterName, channelId)) {
                User foundUser = _ctx.Users.FirstOrDefault(p => p.UserName == userName);
                Channel foundChannel = _ctx.Channels.FirstOrDefault(c => c.Id == channelId);
                AddChannelUser(foundChannel, foundUser);
            }
        }


        public List<User> GetChannelUsers(int chatId, string userName) {
            if (CheckIfChannelExists(userName, chatId))
            {
               // List<ChannelUser> cu = _ctx.ChannelUsers.Any(c => c.ChannelId )
                return _ctx.Users.Where(t => t.channelUsers.Any(s => s.ChannelId == chatId)).ToList();
            }
            else
            {
                return new List<User>();
            }

        }

        public void KickChannelUser(string userId, int channelId) {
            var chUser = new ChannelUser { ChannelId = channelId, UserId = userId };
            _ctx.ChannelUsers.Attach(chUser);
            _ctx.ChannelUsers.Remove(chUser);
        }

    }
}
