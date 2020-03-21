using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Semestro_projektas.Data;
using Semestro_projektas.Data.Repository;
using Semestro_projektas.Models;

namespace Semestro_projektas.Controllers
{
    public class HomeController : Controller
    {
        private IRepository _repo; //Database repo

        public HomeController(IRepository repo)
        {
            _repo = repo;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        ////DB Controller


        public IActionResult DbPost()
        {
            return View();
        }

        public IActionResult AllPosts()
        {
            var posts = _repo.GetAllPosts();
            return View(posts);
        }


        [HttpGet]
        public IActionResult Edit()
        {
            return View(new Post());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            _repo.AddPost(post);

            if (await _repo.SaveChangesAsync())
            {
                return RedirectToAction("Index");
            }
            else {
                return View(post);
            }

        }





    }
}
