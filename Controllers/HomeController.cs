using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BankAccounts.Models;

namespace BankAccounts.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BankAccountsContext db;

        public HomeController(ILogger<HomeController> logger, BankAccountsContext context)
        {
            _logger = logger;
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(db.Users.Any(user => user.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View("Index");
                }
            }

            if(ModelState.IsValid == false)
            {
                return View("Index");
            }
            
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);

            db.Add(newUser);
            db.SaveChanges();

            HttpContext.Session.SetInt32("UserID", newUser.UserID);
            HttpContext.Session.SetString("FirstName", newUser.FirstName);

            return RedirectToAction("Landing", "Account");
        }

        [HttpGet("/loginpage")]
        public IActionResult LoginPage()
        {
            return View("Login");
        }

        [HttpPost("/login")]
        public IActionResult Login(LoginUser loginUser)
        {
            if(!ModelState.IsValid)
            {
                return View("Login");
            }

            User dbUser = db.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);
            if(dbUser == null)
            {
                ModelState.AddModelError("Email", "Email not found.");
                return View("Login");
            }

            PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
            PasswordVerificationResult pwCompare = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

            if(pwCompare == 0)
            {
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Login");
            }


            HttpContext.Session.SetInt32("UserID", dbUser.UserID);
            HttpContext.Session.SetString("FirstName", dbUser.FirstName);

            return RedirectToAction("Landing", "Account");
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
    }
}
