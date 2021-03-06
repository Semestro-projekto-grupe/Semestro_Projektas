﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Semestro_projektas.Data.Repository;
using Semestro_projektas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Controllers
{
    public class ChatController : Controller
    {
        private IRepository _repo;

        //naudojama lokalizacijai
        //naudojimas: _localizer["pvz"] / arba per ILocalizer metodus
        //"isvercia" "pvz" i current culture nustatyta kalba
        private readonly IStringLocalizer<ChatController> _localizer;

        public ChatController(IRepository repo, IStringLocalizer<ChatController> localizer)
        {
            _repo = repo;
            _localizer = localizer;
        }


        public IActionResult Chat()
        {
            if (!User.Identity.IsAuthenticated) {
                return RedirectToAction("Login", "LoginRegister");
            }
            ViewData["User"] = _repo.GetUsers();
            //ViewData["userChannels"] = _repo.GetUserChannels(User.Identity.Name);
            var messages = _repo.GetChatMessages();
            List<string> roles = new List<string>();
            roles.Add("Administratorius");
            roles.Add("Moderatorius");
            roles.Add("Vartotojas");
            ViewData["Roles"] = roles;
            return View(messages);
        }



        [HttpPost]
        public async Task<JsonResult> Send(string name, string text, int channelId)
        {
            Message msg = new Message();
            User author = _repo.GetUserByName(name);
            Channel channel = _repo.GetChannelSettings(channelId);
            msg.Author = author;
            msg.Content = text;
            msg.Created = DateTime.Now;
            msg.Channel = channel;
            msg.AuthorName = author.UserName;
            msg.ChannelId = channel.Id;
            if (User.Identity.Name == name && msg.Content.Length <= 2000)
            {
                _repo.SaveMessage(msg);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg");
            }
            else
            {
                return Json("failed to save data");
            }
            
        }

        [HttpPost]
        public async Task<JsonResult> GetUsersJson()
        {
            List<User> users = _repo.GetUsers();
            List<string> userNames = new List<string>();

            foreach (User u in users) {
                userNames.Add(u.UserName);
            }

            var json = JsonConvert.SerializeObject(userNames);
            return Json(json);
        }

        [HttpPost]
        public async Task<JsonResult> GetChatMessages(int chatId, string userName, int count = 20)
        {
            List<Message> messages = null;
            //<(Id, AuthorName, Avatar, Created, Content, Role1, Role2)>
            List <(int, string, string, DateTime, string, int, int)> messagesToGet = new List<(int, string, string, DateTime, string, int, int)>();
            if (User.Identity.Name == userName)
            {
                messages = _repo.GetChatMessagesByChat(chatId, userName, count);
                foreach (Message msg in messages) {
                    User usr = _repo.GetUserByName(msg.AuthorName);
                    ChannelUser chUsr = _repo.GetChannelUser(chatId, userName);
                    ChannelUser authorUsr = _repo.GetChannelUser(chatId, msg.AuthorName);
                    messagesToGet.Add((msg.Id, msg.AuthorName, usr.Avatar, msg.Created, msg.Content, (int)chUsr.Role, (int)authorUsr.Role));
                }
            }
            else {
                messages = null;
            }
            // foreach (var c in chn) {
            //chnNames.Add("{c.nam}"c.Name);
            // }
            var json = JsonConvert.SerializeObject(messagesToGet);
            return Json(json);
            /*if (await _repo.SaveChangesAsync())
            {
                return Json(chn);
            }
            else
            {
                return Json("failed to save data");
            }*/

        }

        [HttpPost]
        public async Task<JsonResult> EditMessage(int messageId, string message, string userName)
        {
            if (User.Identity.Name == userName)
            {
                _repo.EditMessage(messageId, message, userName);
            }
            // foreach (var c in chn) {
            //chnNames.Add("{c.nam}"c.Name);
            // }
            if (await _repo.SaveChangesAsync())
            {
                return Json("edited msg");
            }
            else
            {
                return Json("failed to edit msg");
            }
            /*if (await _repo.SaveChangesAsync())
            {
                return Json(chn);
            }
            else
            {
                return Json("failed to save data");
            }*/

        }


        [HttpPost]
        public async Task<JsonResult> CreateChannel(string name, string userName)
        {
            if (name.Length <= 40)
            {
                Channel channel = new Channel();
                channel.Name = name;
                channel.CreationDate = DateTime.Now;
                _repo.CreateChannel(channel, userName);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("Kanalas sukurtas");
            }
            else
            {
                return Json("failed to save data");
            }

        }



        [HttpPost]
        public async Task<JsonResult> GetUserChannels(string userName)
        {
            if (User.Identity.Name == userName)
            {
                List<Channel> chn = _repo.GetUserChannels(userName);
                List<(int, string, bool)> chnData = new List<(int, string, bool)>();
                User usr = _repo.GetUserByName(userName);
                foreach (Channel c in chn) {
                    ChannelUser cUser = _repo.GetChannelUser(c.Id, userName);
                    chnData.Add((c.Id, c.Name, cUser.ReceivedNotification));
                }
                var json = JsonConvert.SerializeObject(chnData);
                return Json(json);
            }
            else {
                return null;
            }
            /*if (await _repo.SaveChangesAsync())
            {
                return Json(chn);
            }
            else
            {
                return Json("failed to save data");
            }*/

        }


        [HttpPost]
        public async Task<JsonResult> AddUserToChannel(string userName, string inviterName, int channelId)
        {
            if (User.Identity.Name == inviterName)
            {
                _repo.AddUserToChannel(userName, inviterName, channelId);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("Success");
            }
            else
            {
                return Json("failed to save data");
            }

        }


        
        [HttpPost]
        public async Task<JsonResult> GetChannelUsers(int chatId, string userName)
        {
            if (User.Identity.Name == userName)
            {
                List<User> chu = _repo.GetChannelUsers(chatId, userName);
                // foreach (var c in chn) {
                //chnNames.Add("{c.nam}"c.Name);
                // }
                var json = JsonConvert.SerializeObject(chu);
                return Json(json);
            }
            else {
                return null;
            }
            /*if (await _repo.SaveChangesAsync())
            {
                return Json(chn);
            }
            else
            {
                return Json("failed to save data");
            }*/

        }

        [HttpPost]
        public async Task<JsonResult> KickChannelUser(int channelId, string userId, string caller)
        {
            if (User.Identity.Name == caller)
            {
                _repo.KickChannelUser(userId, channelId, caller);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg " + "DELETE ChannelUsers FROM ChannelUsers WHERE ChannelUsers.UserId = '" + userId + "' AND ChannelUsers.ChannelId = " + channelId + ";");
            }
            else
            {
                return Json("failed to save data " + "DELETE ChannelUsers FROM ChannelUsers WHERE ChannelUsers.UserId = '" + userId + "' AND ChannelUsers.ChannelId = " + channelId + ";");
            }

        }

        [HttpPost]
        public async Task<JsonResult> GetUserProfile(string userId, int channelId)
        {
            var data = _repo.GetUser(userId);
            var roleType = _repo.GetUserRole(userId, channelId);
            var chUser = _repo.GetChannelUser(channelId, data.UserName);
            string role = "Nėra";

            switch (roleType)
            {
                case RoleTypes.Creator:
                    role = "Kanalo kūrėjas";
                    break;
                case RoleTypes.Admin:
                    role = "Administratorius";
                    break;
                case RoleTypes.Moderator:
                    role = "Moderatorius";
                    break;
                case RoleTypes.User:
                    role = "Vartotojas";
                    break;
            }

            var temp = Tuple.Create(data.UserName, data.Name, data.Surname, data.Date.ToString("yyyy-MM-dd"), data.Avatar,
                role, _repo.CountUserMessages(channelId, data.UserName), chUser.DateJoined.ToString("yyyy-MM-dd H:mm:ss"));
            return Json(temp);
        }


        [HttpPost]
        public async Task<JsonResult> DeleteMessage(int messageId, string userName, string caller, int channel)
        {
            if (User.Identity.Name == caller)
            {
                _repo.DeleteMessage(messageId, userName, caller, channel);
            }

            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg");
            }
            else
            {
                return Json("failed to save data");
            }

        }


        
        [HttpPost]
        public async Task<JsonResult> GetChannelSettings(int channelId, string userName)
        {
            if (User.Identity.Name == userName)
            {
                var ch = _repo.GetChannelSettings(channelId);
                var json = JsonConvert.SerializeObject(ch);
                ChannelUser activeUser = _repo.GetMostActiveUser(ch.Id, userName);
                User usr = _repo.GetChannelUser(activeUser);
                var temp = Tuple.Create(ch.Id, ch.Name, _repo.CountChannelMessages(channelId, userName),
                    usr.UserName, _repo.CountUserMessages(channelId, usr.UserName), ch.CreationDate.ToString("yyyy-MM-dd H:mm:ss"));
                return Json(temp);
            }
            else {
                return Json("fail read ch settings");
            }

        }

        [HttpPost]
        public async Task<JsonResult> DeleteChannel(int channelId, string userName)
        {
            if (User.Identity.Name == userName)
            {
                _repo.DeleteChannel(channelId, userName);

            }

            if (await _repo.SaveChangesAsync())
            {
                return Json("deleted chn");
            }
            else
            {
                return Json("failed to delete chn");
            }

        }

        [HttpPost]
        public async Task<JsonResult> AssignRole(string receiverId, string callerName, int channelId, int roleValue)
        {
            if (User.Identity.Name == callerName)
            {
                _repo.AssignRole(receiverId, callerName, channelId, roleValue);

            }

            if (await _repo.SaveChangesAsync())
            {
                return Json("added role");
            }
            else
            {
                return Json("failed to add role");
            }

        }

        [HttpPost]
        public async Task<JsonResult> LeaveChannel(int channelId, string userName)
        {
            if (User.Identity.Name == userName)
            {
                _repo.LeaveChannel(channelId, userName);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg " + "DELETE ChannelUsers FROM ChannelUsers WHERE ChannelUsers.UserId = '" + userName + "' AND ChannelUsers.ChannelId = " + channelId + ";");
            }
            else
            {
                return Json("failed to save data " + "DELETE ChannelUsers FROM ChannelUsers WHERE ChannelUsers.UserId = '" + userName + "' AND ChannelUsers.ChannelId = " + channelId + ";");
            }

        }


        [HttpPost]
        public async Task<JsonResult> SendNotification(int channel, string userName)
        {
            if (User.Identity.Name == userName)
            {
                _repo.SendNotification(channel, userName);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg " + "DELETE ChannelUsers FROM");
            }
            else
            {
                return Json("failed to save data " + "DELETE ChannelUsers FROM");
            }

        }

        [HttpPost]
        public async Task<JsonResult> RemoveNotification(int channel, string userName)
        {
            if (User.Identity.Name == userName)
            {
                _repo.RemoveNotification(channel, userName);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg " + "DELETE ChannelUsers FROM ChannelUsers WHERE ");
            }
            else
            {
                return Json("failed to save data " + "DELETE ChannelUsers FROM ChannelUsers");
            }

        }

        
        [HttpPost]
        public async Task<JsonResult> SearchInChat(int channel, string userName, string searchWord)
        {


            List<Message> messages = null;
            //<(Id, AuthorName, Avatar, Created, Content, Role1, Role2)>
            List<(int, string, string, DateTime, string, int, int)> messagesToGet = new List<(int, string, string, DateTime, string, int, int)>();
            if (User.Identity.Name == userName)
            {
                messages = _repo.SearchInChat(channel, userName, searchWord);
                foreach (Message msg in messages)
                {
                    User usr = _repo.GetUserByName(msg.AuthorName);
                    ChannelUser chUsr = _repo.GetChannelUser(channel, userName);
                    ChannelUser authorUsr = _repo.GetChannelUser(channel, msg.AuthorName);
                    messagesToGet.Add((msg.Id, msg.AuthorName, usr.Avatar, msg.Created, msg.Content, (int)chUsr.Role, (int)authorUsr.Role));
                }
            }
            else
            {
                messages = null;
            }
            // foreach (var c in chn) {
            //chnNames.Add("{c.nam}"c.Name);
            // }
            var json = JsonConvert.SerializeObject(messagesToGet);
            return Json(json);

        }


        [HttpPost]
        public async Task<JsonResult> DeleteMessagesCommand(int channelId, string userName, int messageCount)
        {

            if (User.Identity.Name == userName)
            {
                _repo.DeleteMessagesCommand(channelId, userName, messageCount);
            }
            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg " + "DELETE ChannelUsers FROM ChannelUsers WHERE ");
            }
            else
            {
                return Json(new { success = false, responseText = "The attached file is not supported." });
            }

        }

    }
}
