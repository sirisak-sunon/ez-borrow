using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ez.Borrow.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace Ez.Borrow.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private readonly IDistributedCache cache;

        public HomeController(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public IActionResult Index()
        {
            var username = cache.GetString(Utility.username_key);
            if (!string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Index", "Goods");
            }

            return View();
        }

        public IActionResult Login(string username)
        {
            cache.SetString(Utility.username_key, username);
            return RedirectToAction("Index", "Goods");
        }

        public IActionResult Logout()
        {
            cache.Remove(Utility.username_key);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
