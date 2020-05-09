using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Semestro_projektas.Data.Repository;
using Semestro_projektas.Models;

namespace Semestro_projektas.Controllers
{
    public class SettingsController : Controller
    {
        private IRepository _repo;


        public SettingsController(IRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Settings()
        {
            User user = _repo.GetUser("a8de697e-dcb1-4b29-b00e-93f6d6813b13");
            ViewData["year"] = user.Date.Year;
            ViewData["month"] = user.Date.Month;
            ViewData["day"] = user.Date.Day;
            ViewData["year2"] = DateTime.Now.Year;
            return View(user);
        }

        [HttpPost]
        public IActionResult Settings(User user, string pass, string password, string data, string change) 
        {
            ModelState.AddModelError("Password", "");
            DataBack(data);
            try
            {
                //Index.@HTML pateiktos tik readonly reikšmės, kurių automatiškai pakeisti be validacijos iš back-endo pusės neina.
                if (change == "data")
                {
                    if (data.Contains("—") || data.Length == 1)
                    {
                        ModelState.AddModelError("Date", "Pateikta neteisinga gimimo data!");
                        return View(user);
                    }
                    user.Date = Convert.ToDateTime(data); //Užkraunama data į objektą

                    string fixedName = user.Name.Substring(0, 1).ToUpper() + user.Name.Substring(1);
                    user.Name = fixedName;

                    string fixedSurname = user.Surname.Substring(0, 1).ToUpper() + user.Surname.Substring(1);
                    user.Surname = fixedSurname;

                    user.UserName = user.NickName;
                    return RedirectToAction("Chat", "Chat");
                }
                else
                {
                    ViewData["show"] = "t";
                    if (pass != password) //patikrinimas ar įvesti slaptažodžiai sutampa
                    {
                        ModelState.AddModelError("Password", "Slaptažodžiai nesutampa!");
                       // return RedirectToAction("Chat", "Chat");
                        return View(user);
                    }
                    user.Name = "sfsfs2";
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
                    user.Password = pass; //Užkraunamas slaptažodis į objektą
                  //  bool register = _repo.RegisterUser(user, user.Password, userManager, roleManager);

                     return RedirectToAction("Chat", "Chat");
                }
                return View(user);
            }
            catch (Exception)
            {
                return View(user); //exeption gaudyklė
            }
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