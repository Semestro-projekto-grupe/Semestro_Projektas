using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NPoco.DatabaseTypes;
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

        public List<Message> GetChatMessagesByChat(int chatId, string user)
        {
            if (CheckIfChannelExists(user, chatId))
            {
                return _ctx.Messages.Where(m => m.ChannelId == chatId).ToList();
            }
            else
            {
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

        public User GetUser(string id)
        {
            User usr = _ctx.Users.FirstOrDefault(p => p.Id == id);
            User nUsr = new User();
            nUsr.Avatar = usr.Avatar;
            nUsr.Date = usr.Date;
            nUsr.Name = usr.Name;
            nUsr.NickName = usr.NickName;
            nUsr.Surname = usr.Surname;
            nUsr.UserName = usr.UserName;
            nUsr.Id = usr.Id;
            if (usr.NameVisibility == false) {
                nUsr.Name = "(Paslėpta)";
            }

            if(usr.SurnameVisibility == false)
            {
                nUsr.Surname = "(Paslėpta)";
            }

            if (usr.DateVisibility == false) {
                nUsr.Date = new DateTime(1850, 1, 1);
            }
            return nUsr;
        }

        public void CreateChannel(Channel channel, string userName)
        {
            _ctx.Channels.Add(channel);

            User creator = _ctx.Users.FirstOrDefault(p => p.UserName == userName);
            AddChannelUser(channel, creator, RoleTypes.Creator);
        }

        public void AddChannelUser(Channel channel, User user, RoleTypes role)
        {
            ChannelUser channelUser = new ChannelUser();
            channelUser.ChannelId = channel.Id;
            channelUser.Channel = channel;
            channelUser.UserId = user.Id;
            channelUser.Role = role;
            _ctx.ChannelUsers.Add(channelUser);
            user.channelUsers.Add(channelUser);
            channel.channelUsers.Add(channelUser);
            
        }


        public List<Channel> GetUserChannels(string userName)
        {
            // User user = _ctx.Users.FirstOrDefault(p => p.Name == userName);
            string uid = GetUserByName(userName).Id;
            var channels = _ctx.Channels.Where(t => t.channelUsers.Any(s => s.User.Id == uid));
            //foreach (ChannelUser c in user.channelUsers) {
            // channels.Add(c.Channel);
            // }
            return channels.ToList();
        }


        public bool CheckIfChannelExists(string userName, int id)
        {
            string uid = GetUserByName(userName).Id;
            var channels = _ctx.Channels.Where(t => t.channelUsers.Any(s => s.User.Id == uid));
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



        public void AddUserToChannel(string userName, string inviterName, int channelId)
        {
            if (CheckIfChannelExists(inviterName, channelId))
            {
                string uid = GetUserByName(userName).Id;
                User foundUser = _ctx.Users.FirstOrDefault(p => p.Id == uid);
                Channel foundChannel = _ctx.Channels.FirstOrDefault(c => c.Id == channelId);
                AddChannelUser(foundChannel, foundUser, RoleTypes.User);
            }
        }


        public List<User> GetChannelUsers(int chatId, string userName)
        {
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

        public void KickChannelUser(string userId, int channelId, string callerName)
        {
            string callerUid = GetUserByName(callerName).Id;
            ChannelUser chUser = _ctx.ChannelUsers.FirstOrDefault(u => u.UserId == userId && u.ChannelId == channelId);
            ChannelUser callerChUser = _ctx.ChannelUsers.FirstOrDefault(u => u.UserId == callerUid && u.ChannelId == channelId);
            if (callerChUser.Role != RoleTypes.User && chUser.Role != RoleTypes.Creator &&
                callerChUser.Role != chUser.Role && chUser != callerChUser)
            {
                if (callerChUser.Role == RoleTypes.Creator || callerChUser.Role == RoleTypes.Admin ||
                    (callerChUser.Role == RoleTypes.Moderator && chUser.Role == RoleTypes.User))
                {
                    _ctx.Users.FirstOrDefault(u => u.Id == userId).channelUsers.Remove(chUser);
                    _ctx.Channels.FirstOrDefault(c => c.Id == channelId).channelUsers.Remove(chUser);
                    var rez = _ctx.ChannelUsers.Remove(chUser);
                }
            }

        }

        User GetUserByName(string name)
        {
            User usr = _ctx.Users.FirstOrDefault(u => u.UserName == name);
            return usr;
        }




        public bool EditUserData(User user, string change, string pass2 = null, UserManager<User> userManager = null)
        {
            var result = _ctx.Users.SingleOrDefault(b => b.Id == user.Id);
            if (result != null)
            {
                if (change == "data")
                {
                    result.Name = user.Name;
                    result.Surname = user.Surname;
                    result.Date = user.Date;
                    if (user.Avatar != null)
                        result.Avatar = user.Avatar;
                }
                else if (change == "nick")
                {
                    if (!_ctx.Users.Any(u => u.UserName == user.NickName))
                    {
                        ChangeChatMessagesNames(result.UserName, user.NickName);
                        result.NickName = user.NickName;
                        result.UserName = user.NickName;
                        result.NormalizedUserName = user.NickName.ToUpper();
                        _ctx.SaveChanges();
                        return true;
                    }
                    return false;
                }
                else if (change == "papild")
                {
                    result.NameVisibility = user.NameVisibility;
                    result.SurnameVisibility = user.SurnameVisibility;
                    result.DateVisibility = user.DateVisibility;
                }

                    _ctx.SaveChanges();
                return true;
            }
            return false;
        }


        void ChangeChatMessagesNames(string oldName, string newName)
        {
            List<Message> messages = _ctx.Messages.ToList();

            foreach (Message msg in messages)
            {
                if (msg.Author == oldName)
                {
                    msg.Author = newName;
                    _ctx.Messages.Update(msg);
                }
            }
            _ctx.SaveChanges();
        }


        public void DeleteMessage(int messageId, string userName) {
            Message msg = _ctx.Messages.FirstOrDefault(m => m.Author == userName);
            _ctx.Messages.Remove(msg);
            _ctx.SaveChanges();
        }


        public void DeleteChannel(int channelId, string userName) {
            string uid = GetUserByName(userName).Id;
            ChannelUser chUser = _ctx.ChannelUsers.FirstOrDefault(u => u.UserId == uid && u.ChannelId == channelId);
            if (chUser.Role == RoleTypes.Creator)
            {
                List<User> chUsers = _ctx.Users.Where(t => t.channelUsers.Any(s => s.ChannelId == channelId)).ToList();
                List<Message> chMessages = _ctx.Messages.Where(m => m.ChannelId == channelId).ToList();

                foreach (Message m in chMessages)
                {
                    _ctx.Messages.Remove(m);
                }

                foreach (User u in chUsers)
                {
                    KickChannelUser(u.Id, channelId, userName);
                }

                Channel chn = _ctx.Channels.FirstOrDefault(c => c.Id == channelId);
                _ctx.Channels.Remove(chn);
            }
        }


        public Channel GetChannelSettings(int channelId) {
            return _ctx.Channels.FirstOrDefault(c => c.Id == channelId);
        }


        public RoleTypes GetUserRole(string userId, int channelId) {
            ChannelUser usr = _ctx.ChannelUsers.FirstOrDefault(u => u.UserId == userId && u.ChannelId == channelId);
            return usr.Role;
        }

       public void AssignRole(string receiverId, string callerName, int channelId, int roleValue) {
            User caller = GetUserByName(callerName);
            User receiver = GetUser(receiverId);
            ChannelUser callerChUser = _ctx.ChannelUsers.FirstOrDefault(u => u.UserId == caller.Id && u.ChannelId == channelId);
            ChannelUser receiverChUser = _ctx.ChannelUsers.FirstOrDefault(u => u.UserId == receiver.Id && u.ChannelId == channelId);
            if (callerChUser.Role == RoleTypes.Creator || callerChUser.Role == RoleTypes.Admin)
            {
                if (callerChUser.Role != receiverChUser.Role && receiverChUser.UserId != callerChUser.UserId && receiverChUser.Role != RoleTypes.Creator)
                {
                    receiverChUser.Role = (RoleTypes)roleValue;
                }
            }
       }


        public void LeaveChannel(int channelId, string userName) {
            string userId = GetUserByName(userName).Id;
            ChannelUser chUser = _ctx.ChannelUsers.FirstOrDefault(u => u.UserId == userId && u.ChannelId == channelId);
            if (chUser.Role != RoleTypes.Creator)
            {
                _ctx.Users.FirstOrDefault(u => u.Id == userId).channelUsers.Remove(chUser);
                _ctx.Channels.FirstOrDefault(c => c.Id == channelId).channelUsers.Remove(chUser);
                var rez = _ctx.ChannelUsers.Remove(chUser);
            }
        }

    }
}