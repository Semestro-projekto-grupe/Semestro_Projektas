using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Semestro_projektas.Data;
using Semestro_projektas.Models;

namespace Semestro_projektas.Controllers
{
    public class HomeController : Controller
    {
        private AppDbContext _ctx;

        public HomeController(AppDbContext ctx)
        {
            _ctx = ctx;
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


        [HttpGet]
        public IActionResult Edit()
        {
            return View(new Post());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Post post)
        {
            _ctx.Posts.Add(post);

            await _ctx.SaveChangesAsync();

            return RedirectToAction("Index");
        }





    }
}
