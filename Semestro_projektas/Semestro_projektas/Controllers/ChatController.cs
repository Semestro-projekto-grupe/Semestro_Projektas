using Microsoft.AspNetCore.Mvc;
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
        //naudojimas: _localizer["pvz"]
        //"isvercia" "pvz" i current culture nustatyta kalba
        private readonly IStringLocalizer<HomeController> _localizer;

        public ChatController(IRepository repo, IStringLocalizer<HomeController> localizer)
        {
            _repo = repo;
            _localizer = localizer;
        }


        public IActionResult Chat()
        {
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
            msg.Author = name;
            msg.Content = text;
            msg.Created = DateTime.Now;
            msg.ChannelId = channelId;
            if (User.Identity.Name == name)
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
            var json = JsonConvert.SerializeObject(users);
            return Json(json);
        }

        [HttpPost]
        public async Task<JsonResult> GetChatMessages(int chatId, string userName)
        {
            List<Message> messages = null;
            if (User.Identity.Name == userName)
            {
                messages = _repo.GetChatMessagesByChat(chatId, userName);
            }
            else {
                messages = null;
            }
            // foreach (var c in chn) {
            //chnNames.Add("{c.nam}"c.Name);
            // }
            var json = JsonConvert.SerializeObject(messages);
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
            Channel channel = new Channel();
            channel.Name = name;
            _repo.CreateChannel(channel, userName);
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
                List<string> chnNames = new List<string>();
                // foreach (var c in chn) {
                //chnNames.Add("{c.nam}"c.Name);
                // }
                var json = JsonConvert.SerializeObject(chn);
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

            var temp = Tuple.Create(data.UserName, data.Name, data.Surname, data.Date.ToString("yyyy-MM-dd"), data.Avatar, role);
            return Json(temp);
        }


        [HttpPost]
        public async Task<JsonResult> DeleteMessage(int messageId, string userName)
        {
            if (User.Identity.Name == userName)
            {
                _repo.DeleteMessage(messageId, userName);
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
                return Json(json);
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


    }
}
