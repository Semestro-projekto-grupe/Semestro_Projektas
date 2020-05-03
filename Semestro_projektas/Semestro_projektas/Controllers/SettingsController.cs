using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            return View(user);
        }

        [HttpPost]
        public IActionResult Settings(User user, string pass, string password, string data) 
        {
            DataBack(data);
            try
            {
                //Index.@HTML pateiktos tik readonly reikšmės, kurių automatiškai pakeisti be validacijos iš back-endo pusės neina.
                if (pass != password) //patikrinimas ar įvesti slaptažodžiai sutampa
                {
                    ModelState.AddModelError("Password", "Slaptažodžiai nesutampa!");
                    if (data.Contains("—") || data.Length == 1)
                    {
                        ModelState.AddModelError("Date", "Pateikta neteisinga gimimo data!");
                    }
                    return View(user);
                }
                if (data.Contains("—") || data.Length == 1)
                {
                    ModelState.AddModelError("Date", "Pateikta neteisinga gimimo data!");
                    return View(user);
                }
                if (ModelState.IsValid)
                {
                    //šioje vietoje turi būti patikrinimas serverio pusėje slaptažodžiui ir nickName (validacijos klaidų metimas)
                    /*if ( user.Nickname == ?)    //if su boolean salyga
                    {
                        ModelState.AddModelError("Nickname", "Toks slapyvardis jau yra naudojamas!");
                        return View(user);
                    }
                    if(pass == ?)      //if su boolean salyga//jei sutaps frontende pateikti slaptažodžiai pass ir password kintamieji bus lygūs
                    {
                         ModelState.AddModelError("Password", "Toks slaptažodis jau yra naudojamas!");
                    }*/
                    //----------------------------------------------------------------------------
                    //Duomenų perkėlimas į duomenų bazę
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
                    user.Password = pass; //Užkraunamas slaptažodis į objektą
                    user.Date = Convert.ToDateTime(data); //Užkraunama data į objektą

                    string fixedName = user.Name.Substring(0, 1).ToUpper() + user.Name.Substring(1);
                    user.Name = fixedName;

                    string fixedSurname = user.Surname.Substring(0, 1).ToUpper() + user.Surname.Substring(1);
                    user.Surname = fixedSurname;

                    user.UserName = user.NickName;

                    //bool register = _repo.RegisterUser(user, user.Password, userManager, roleManager);
                    //
                    /*if (!register)
                    {
                        ModelState.AddModelError("NickName", "Toks vartotojo vardas jau egzistuoja!");
                        return View(user);
                    }
                    TempData["Success"] = "Successful";*/
                    return RedirectToAction("Chat", "Chat"); // Nukėlimas į kitą kontrolerį arba sekantį šio kontrolerio langą + registracija sėkminga galima prisijungti
                }
                return View(user); //Perkėlimas į sekančio kontrolerio vaizdą
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