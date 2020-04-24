using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public LoginRegisterController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IRepository repo) {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            _repo = repo;
        }

        public ActionResult Login()
        {
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
                        ModelState.AddModelError("NickName", "Vartotojas tokiu slapyvardžiu nerastas!");
                        ModelState.AddModelError("Password", "Neteisingas slaptažodis!");
                        return View(user);
                    }
                    else if (user.NickName == null)
                    {
                        ModelState.AddModelError("NickName", "Vartotojas tokiu slapyvardžiu nerastas!");
                        return View(user);
                    }
                    else if (user.Password == null)
                    {
                        ModelState.AddModelError("Password", "Neteisingas slaptažodis!");
                        return View(user);
                    }
                    else if (user.Password.Length < 6)
                    {
                        ModelState.AddModelError("Password", "Minimalus slaptažodžio ilgis 6 simboliai!");
                        return View(user);
                    }
                    var result = await signInManager.PasswordSignInAsync(user.NickName, user.Password, false, false);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("Password", "Įvesti neteisingi prisijungimo duomenys!");
                        return View(user);
                    }
                    return RedirectToAction("Chat", "Home"); //-- teisingo patvirtino atveju nukreipimas į kitą valdiklį 
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
                else if(Regex.Matches(pass, temp).Count == pass.Length) //Vienodų simbolių naudojimo atvejis slaptažodyje
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

                    bool register = _repo.RegisterUser(user, user.Password, userManager, roleManager);
                    if (!register) {
                        ModelState.AddModelError("NickName", "Toks vartotojo vardas jau egzistuoja!");
                        return View(user);
                    }
                    TempData["Success"] = "Successful";
                    return RedirectToAction("Login", "LoginRegister");
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