using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Semestro_projektas.Data.Repository;
using Semestro_projektas.Models;

namespace Semestro_projektas.Controllers
{
    public class SettingsController : Controller
    {
        private IRepository _repo;
        private readonly UserManager<User> userManager;

        public SettingsController(UserManager<User> userManager, IRepository repo)
        {
            this.userManager = userManager;
            _repo = repo;
        }
        public async Task <IActionResult> Settings()
        {
            User user = await userManager.GetUserAsync(User);
            ViewData["year"] = user.Date.Year;
            ViewData["month"] = user.Date.Month;
            ViewData["day"] = user.Date.Day;
            ViewData["year2"] = DateTime.Now.Year;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(User user, string pass, string password, string pass2, string data, string change, string id) 
        {
            //ModelState.AddModelError("Password", "");
            DataBack(data);
            // try
            // {
            if (change == "data")
            {
                if (data.Contains("—") || data.Length == 1)
                {
                    ModelState.AddModelError("Date", "Pateikta neteisinga gimimo data!");
                    return View(user);
                }
                user.Date = Convert.ToDateTime(data); //Užkraunama data į objektą
                if (user.Name == null || user.Surname == null)
                    return View(user);
                string fixedName = user.Name.Substring(0, 1).ToUpper() + user.Name.Substring(1);
                user.Name = fixedName;

                string fixedSurname = user.Surname.Substring(0, 1).ToUpper() + user.Surname.Substring(1);
                user.Surname = fixedSurname;

                _repo.EditUserData(user, change);
                //return RedirectToAction("Chat", "Chat");
            }
            else if (change == "nick")
            {
                if (user.NickName == null)
                    return View(user);
                user.UserName = user.NickName;
                bool result = _repo.EditUserData(user, change, pass2);
                if (!result)
                {
                    ModelState.AddModelError("NickName", "Toks slapyvardis jau yra!");
                    return View(user);
                }
            }
            else
            {
                ViewData["show"] = "t";
                if (pass == null || password == null || pass2 == null)
                {
                    if (pass2 == null)
                    {
                        ModelState.AddModelError("pass", "Neįvestas slaptažodis!");
                        ModelState.AddModelError("Password", " ");
                        return View(user);
                    }
                    ModelState.AddModelError("Password", "Neįvestas slaptažodis!");
                    return View(user);
                }
                if (pass != password) //patikrinimas ar įvesti slaptažodžiai sutampa
                {
                    ModelState.AddModelError("Password", "Slaptažodžiai nesutampa!");
                    // return RedirectToAction("Chat", "Chat");
                    return View(user);
                }
                string temp = pass.Substring(0, 1);
                if (Regex.Matches(pass, "[^a-zA-Z]").Count == pass.Length) //Patikra dėl neraidžių naudojimo (turi būti bent viena raidė)
                {
                    ModelState.AddModelError("Password", "Slaptažodyje privalo būti bent viena raidė!");
                    return View(user);
                }
                else if (Regex.Matches(pass, temp).Count == pass.Length) //Vienodų simbolių naudojimo atvejis slaptažodyje
                {
                    ModelState.AddModelError("Password", "Slaptažodis negali būti sudarytas iš vienodų simbolių!");
                    return View(user);
                }
                ModelState.AddModelError("Password", " ");
                var user2 = await userManager.GetUserAsync(User);
                var result = await userManager.ChangePasswordAsync(user2, pass2, password);
                user.Name = result.ToString();
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("pass", "Neteisingas slaptažodis!");
                    return View(user);
                }
            }
            ViewData["Success2"] = "tt";
                return View(user);
           /* }
            catch (Exception)
            {
                return View(user); //exeption gaudyklė
            }*/
        }
        public void DataBack(string data)
        {
            if (data.Length > 1)
            {
                string[] temp = data.Split('-');
                ViewData["year"] = temp[0].Contains("—") ? "0" : temp[0];
                ViewData["month"] = temp[1].Contains("—") ? "0" : temp[1];
                ViewData["day"] = temp[2].Contains("—") ? "0" : temp[2];
                ViewData["year2"] = DateTime.Now.Year;
            }
        }
    }
}