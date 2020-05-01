﻿using Microsoft.AspNetCore.Mvc;
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


        public ChatController(IRepository repo)
        {
            _repo = repo;
        }


        public IActionResult Chat()
        {
            ViewData["User"] = _repo.GetUsers();
            //ViewData["userChannels"] = _repo.GetUserChannels(User.Identity.Name);
            var messages = _repo.GetChatMessages();

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

            _repo.SaveMessage(msg);
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
        public async Task<JsonResult> GetChatMessages(int chatId, string userName)
        {
            List<Message> messages = _repo.GetChatMessagesByChat(chatId, userName);
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
        public async Task<JsonResult> CreateChannel(string name, string userName)
        {
            Channel channel = new Channel();
            channel.Name = name;
            _repo.CreateChannel(channel, userName);
            if (await _repo.SaveChangesAsync())
            {
                return Json("sent msg " + _repo.GetUserChannels(userName));
            }
            else
            {
                return Json("failed to save data");
            }

        }



        [HttpPost]
        public async Task<JsonResult> GetUserChannels(string userName)
        {
            List<Channel> chn =  _repo.GetUserChannels(userName);
            List<string> chnNames = new List<string>();
           // foreach (var c in chn) {
                //chnNames.Add("{c.nam}"c.Name);
           // }
            var json = JsonConvert.SerializeObject(chn);
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






    }
}
