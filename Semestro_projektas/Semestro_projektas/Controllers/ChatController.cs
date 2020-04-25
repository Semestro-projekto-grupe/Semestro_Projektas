using Microsoft.AspNetCore.Mvc;
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
            var messages = _repo.GetChatMessages();

            return View(messages);
        }



        [HttpPost]
        public async Task<JsonResult> Send(string name, string text)
        {
            Message msg = new Message();
            msg.Author = name;
            msg.Content = text;
            msg.Created = DateTime.Now;

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


    }
}
