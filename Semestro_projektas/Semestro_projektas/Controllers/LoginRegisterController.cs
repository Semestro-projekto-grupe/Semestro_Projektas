using System;
using Microsoft.AspNetCore.Mvc;
using Semestro_projektas.Models;

namespace Semestro_projektas.Controllers
{
    public class LoginRegisterController : Controller
    {

        public ActionResult Login()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public ActionResult Login(User user, string submitButton)
        {
            try
            {
                if (submitButton == "Check")
                    {
                        // = kintamasis iš duomenų bazės
                        if (user.NickName != "NickNameIšDuomenųBazės" && user.Password != "PasswordIšDuomenųBazės") //-- papildyti
                        {
                            ModelState.AddModelError("NickName", "Vartotojas tokiu slapyvardžiu nerastas!");
                            ModelState.AddModelError("Password", "Neteisingas slaptažodis!");
                            return View(user);
                        }
                        if (user.NickName != "NickNameIšDuomenųBazės") // -- serverio validacija //pakeisti
                        {
                            ModelState.AddModelError("NickName", "Vartotojas tokiu slapyvardžiu nerastas!");
                        return View(user);
                        }
                        if (user.Password != "PasswordIšDuomenųBazės") // -- serverio validacija //pakeisti
                        {
                            ModelState.AddModelError("Password", "Neteisingas slaptažodis!");
                        return View(user);
                        }
                        return RedirectToAction("Index", "Home"); //-- teisingo patvirtino atveju nukreipimas į kitą valdiklį 
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
                    user.Password = pass; //Užkraunamas slaptažodis į objektą
                    user.Date = Convert.ToDateTime(data); //Užkraunama data į objektą
                    return RedirectToAction("Index", "Home"); // Nukėlimas į kitą kontrolerį arba sekantį šio kontrolerio langą + registracija sėkminga galima prisijungti
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