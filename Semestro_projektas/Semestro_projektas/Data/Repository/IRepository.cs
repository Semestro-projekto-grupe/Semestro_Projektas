using Microsoft.AspNetCore.Identity;
using Semestro_projektas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Data.Repository
{
    public interface IRepository
    {

        Post GetPost(int id);
        List<Post> GetAllPosts();
        void AddPost(Post post);
        void RemovePost(int id);
        void UpdatePost(Post post);

        Message GetMessage(int id);
        List<Message> GetChatMessages();
        List<Message> GetChatMessagesByChat(int chatId, string user);
        void SaveMessage(Message message);
        void EditMessage(Message msg);
        void RemoveMessage(int id);
        


        Task<bool> SaveChangesAsync();

        bool RegisterUser(User user, string password, UserManager<User> userManager,
             RoleManager<IdentityRole> roleManager);

        List<User> GetUsers();
        User GetUser(string id);

        void CreateChannel(Channel channel, string userName);
        List<Channel> GetUserChannels(string userName);
        void AddUserToChannel(string userName, string inviterName, int channelId);
        List<User> GetChannelUsers(int chatId, string userName);
        void KickChannelUser(string userId, int channelId);
    }
}
