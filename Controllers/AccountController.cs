using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BankAccounts.Models;
using Microsoft.AspNetCore.Identity;

namespace BankAccounts_main.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private static BankAccountsContext db;

        public AccountController(ILogger<AccountController> logger, BankAccountsContext context)
        {
            _logger = logger;
            db = context;
        }
        private int? uid
        {
            get
            {
                return HttpContext.Session.GetInt32("UserID");
            }
        }

        private bool isLoggedIn
        {
            get
            {
                return uid != null;
            }
        }

        [HttpGet("/landing")]
        public IActionResult Landing()
        {
            if (!isLoggedIn)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.AllTransactions = db.Transactions.Where(t => t.UserID == HttpContext.Session.GetInt32("UserID")).Include(holder => holder.AccountHolder).ToList();
            ViewBag.User = HttpContext.Session.GetString("FirstName");
            return View("Landing");
        }

        [HttpPost("/newtransaction")]
        public IActionResult NewTransaction(Transaction newT)
        {
            List<Transaction> history = db.Transactions.Where(t => t.UserID == HttpContext.Session.GetInt32("UserID")).ToList();
            int total = 0;
            foreach(Transaction a in history)
            {
                total += a.Amount;
            }
            if((total+newT.Amount)<0)
            {
                ModelState.AddModelError("Amount", "Cannot overdraw account.");
                ViewBag.AllTransactions = db.Transactions.Where(t => t.UserID == HttpContext.Session.GetInt32("UserID")).Include(holder => holder.AccountHolder).ToList();
                ViewBag.User = HttpContext.Session.GetString("FirstName");
                return View("Landing");
            }

            newT.UserID = (int)uid;

            db.Add(newT);
            db.SaveChanges();
            return RedirectToAction("Landing");
        }
    }
}