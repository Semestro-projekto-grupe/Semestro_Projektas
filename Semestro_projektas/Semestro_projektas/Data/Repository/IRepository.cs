﻿using Microsoft.AspNetCore.Identity;
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
        List<Message> GetChatMessagesByChat(int chatId, string user, int count);
        void SaveMessage(Message message);
        void EditMessage(int id, string text, string user);
        void RemoveMessage(int id);



        Task<bool> SaveChangesAsync();

        bool RegisterUser(User user, string password, UserManager<User> userManager,
             RoleManager<IdentityRole> roleManager);

        List<User> GetUsers();
        User GetUser(string id);
        RoleTypes GetUserRole(string userId, int channelId);
        void CreateChannel(Channel channel, string userName);
        List<Channel> GetUserChannels(string userName);
        void AddUserToChannel(string userName, string inviterName, int channelId);
        List<User> GetChannelUsers(int chatId, string userName);
        void KickChannelUser(string userId, int channelId, string callerName);

        bool EditUserData(User user, string change, string pass = null, UserManager<User> userManager = null);

        void DeleteMessage(int messageId, string userName, string caller, int channelId);

        void DeleteChannel(int channelId, string userName);

        Channel GetChannelSettings(int channelId);

        void AssignRole(string receiverId, string callerName, int channelId, int roleValue);

        void LeaveChannel(int channelId, string userName);

        User GetUserByName(string name);

        void SendNotification(int channel, string userName);

        void RemoveNotification(int channel, string userName);

        List<Message> SearchInChat(int channel, string userName, string searchWord);

       void DeleteMessagesCommand(int channelId, string userName, int messageCount);

        ChannelUser GetChannelUser(int chatId, string userName);

        ChannelUser GetMostActiveUser(int channelId, string userName);
        User GetChannelUser(ChannelUser chUser);

        int CountUserMessages(int channelId, string userName);

        int CountChannelMessages(int channelId, string userName);
    }
}
