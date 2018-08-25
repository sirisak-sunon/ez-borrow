using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ez.Borrow.Models;
using Microsoft.Extensions.Caching.Distributed;
using Ez.Borrow.Repositories;

namespace Ez.Borrow.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BorrowController : Controller
    {
        private readonly IDistributedCache cache;
        private readonly IDataRepository<Goods> goodsDac;

        public BorrowController(IDistributedCache cache, IDataRepository<Goods> goodsDac)
        {
            this.cache = cache;
            this.goodsDac = goodsDac;
        }

        public IActionResult Index()
        {
            var username = cache.GetString(Utility.username_key);
            if (string.IsNullOrEmpty(username))
            {
                TempData["message"] = "Please login.";
                return RedirectToAction("Index", "Home");
            }

            var goodsList = goodsDac.List(g => true);

            ViewBag.username = username;
            return View(goodsList);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
