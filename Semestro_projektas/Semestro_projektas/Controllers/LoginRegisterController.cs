using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Semestro_projektas.Data.Repository;
using Semestro_projektas.Models;

namespace Semestro_projektas.Controllers
{
    public class LoginRegisterController : Controller
    {

        private readonly UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private IRepository _repo; //Database repo
        private readonly IStringLocalizer<LoginRegisterController> _localizer;

        public LoginRegisterController( UserManager<User> userManager,
                                        RoleManager<IdentityRole> roleManager,
                                        SignInManager<User> signInManager,
                                        IRepository repo,
                                        IStringLocalizer<LoginRegisterController> localizer)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            _repo = repo;
            _localizer = localizer;
        }

        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Chat", "Chat");
            }
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user, string submitButton)
        {
            try
            {
                if (submitButton == "Check")
                {

                    if (user.NickName == null && user.Password == null) //-- papildyti
                    {
                        ModelState.AddModelError("NickName", _localizer["Vartotojas tokiu slapyvardžiu nerastas!"]);
                        ModelState.AddModelError("Password", _localizer["Neteisingas slaptažodis!"]);
                        return View(user);
                    }
                    else if (user.NickName == null)
                    {
                        ModelState.AddModelError("NickName", _localizer["Vartotojas tokiu slapyvardžiu nerastas!"]);
                        return View(user);
                    }
                    else if (user.Password == null)
                    {
                        ModelState.AddModelError("Password", _localizer["Neteisingas slaptažodis!"]);
                        return View(user);
                    }
                    else if (user.Password.Length < 6)
                    {
                        ModelState.AddModelError("Password", _localizer["Minimalus slaptažodžio ilgis 6 simboliai!"]);
                        return View(user);
                    }
                    var result = await signInManager.PasswordSignInAsync(user.NickName, user.Password, false, false);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("Password", _localizer["Įvesti neteisingi prisijungimo duomenys!"]);
                        return View(user);
                    }
                    return RedirectToAction("Chat", "Chat"); //-- teisingo patvirtino atveju nukreipimas į kitą valdiklį 
                                                             //tačiau be serverio validacijos šiuo metu neveikia
                }
                else
                    return RedirectToAction("Register", "LoginRegister");
            }
            catch (Exception)
            {
                return View(user);
            }
        }

        public IActionResult Register()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public ActionResult Register(User user, string pass, string password, string data)
        {
            DataBack(data);
            try
            {
                //Index.@HTML pateiktos tik readonly reikšmės, kurių automatiškai pakeisti be validacijos iš back-endo pusės neina.
                if (pass != password) //patikrinimas ar įvesti slaptažodžiai sutampa
                {
                    ModelState.AddModelError("Password", _localizer["Slaptažodžiai nesutampa!"]);
                    if (data.Contains("—") || data.Length == 1)
                    {
                        ModelState.AddModelError("Date", _localizer["Pateikta neteisinga gimimo data!"]);
                    }
                    return View(user);
                }
                if (data.Contains("—") || data.Length == 1)
                {
                    ModelState.AddModelError("Date", _localizer["Pateikta neteisinga gimimo data!"]);
                    return View(user);
                }
                if (ModelState.IsValid)
                {
                    //Duomenų perkėlimas į duomenų bazę
                    string temp = pass.Substring(0, 1);
                    if (Regex.Matches(pass, "[^a-zA-Z]").Count == pass.Length) //Patikra dėl neraidžių naudojimo (turi būti bent viena raidė)
                    {
                        ModelState.AddModelError("Password", _localizer["Slaptažodyje privalo būti bent viena raidė!"]);
                        return View(user);
                    }
                    else if (Regex.Matches(pass, temp).Count == pass.Length) //Vienodų simbolių naudojimo atvejis slaptažodyje
                    {
                        ModelState.AddModelError("Password", _localizer["Slaptažodis negali būti sudarytas iš vienodų simbolių!"]);
                        return View(user);
                    }
                    user.Password = pass; //Užkraunamas slaptažodis į objektą
                    user.Date = Convert.ToDateTime(data); //Užkraunama data į objektą

                    string fixedName = user.Name.Substring(0, 1).ToUpper() + user.Name.Substring(1);
                    user.Name = fixedName;

                    string fixedSurname = user.Surname.Substring(0, 1).ToUpper() + user.Surname.Substring(1);
                    user.Surname = fixedSurname;

                    user.UserName = user.NickName;
                    user.Avatar = "student.png";
                    bool register = _repo.RegisterUser(user, user.Password, userManager, roleManager);
                    if (!register)
                    {
                        ModelState.AddModelError("NickName", _localizer["Toks vartotojo vardas jau egzistuoja!"]);
                        return View(user);
                    }
                    //turetu but panasiai
                    //TempData["Success"] = _localizer.GetString("Registracija sėkminga!!!");
                    //bandant priskirt luzta, lieka neisversta
                    TempData["Success"] = "Registracija sėkminga!!!";
                    return RedirectToAction("Login", "LoginRegister"); // Nukėlimas į kitą kontrolerį arba sekantį šio kontrolerio langą + registracija sėkminga galima prisijungti
                }
                return View(user); //Perkėlimas į sekančio kontrolerio vaizdą
            }
            catch (Exception)
            {
                return View(user); //exeption gaudyklė
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "LoginRegister");
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
