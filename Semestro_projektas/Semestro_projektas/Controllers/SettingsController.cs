using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.Extensions.Localization;
using Semestro_projektas.Data.Repository;
using Semestro_projektas.Models;

namespace Semestro_projektas.Controllers
{
    public class SettingsController : Controller
    {
        private IRepository _repo;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IStringLocalizer<HomeController> _localizer;

        public SettingsController(  UserManager<User> userManager,
                                    SignInManager<User> signInManager,
                                    IRepository repo,
                                    IStringLocalizer<HomeController> localizer)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _repo = repo;
            _localizer = localizer;
        }
        public async Task<IActionResult> Settings()
        {
            User user = await userManager.GetUserAsync(User);
            ViewData["year"] = user.Date.Year;
            ViewData["month"] = user.Date.Month;
            ViewData["day"] = user.Date.Day;
            ViewData["year2"] = DateTime.Now.Year;
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(User user, string pass, string password, string pass2, string data, string change, IFormFile file)
        {
            DataBack(data);
            try
            {
                user.Avatar = _repo.GetUser(user.Id).Avatar;
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
                    if (file == null || file.Length == 0) //Dėl failo nebuvimo galimybės tenka taip rašyti
                    {
                        _repo.EditUserData(user, change);
                    }
                    else
                    {
                        if (user.Avatar == file.FileName || Regex.IsMatch(file.FileName, "((.png)|(.jpg))$"))
                        {
                            if (user.Avatar != "student.png")
                            {
                                string delete = Path.Combine(
                                 Directory.GetCurrentDirectory(), "wwwroot/avatars",
                                 user.Avatar);
                                FileInfo fi = new FileInfo(delete);
                                System.IO.File.Delete(delete);
                                fi.Delete();
                            }
                            string rename = user.NickName + file.FileName.Substring(file.FileName.Length - 4, 4);
                            user.Avatar = rename;
                            var path = Path.Combine(
                                 Directory.GetCurrentDirectory(), "wwwroot/avatars",
                                 rename);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            _repo.EditUserData(user, change);
                            View(user);
                        }
                        else
                        {
                            ModelState.AddModelError("Avatar", "Galimi formatai: (.jpg, .png)!");
                        }

                    }
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
                    user.SecurityStamp = "editUserName";
                    await signInManager.SignInAsync(user, true);
                }
                else if (change == "papild") 
                {
                    _repo.EditUserData(user, change);
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
