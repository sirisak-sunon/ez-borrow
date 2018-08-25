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
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    //[ApiExplorerSettings(IgnoreApi = true)]
    public class GoodsController : Controller
    {
        private readonly IDistributedCache cache;
        private readonly IDataRepository<Goods> goodsDac;

        public GoodsController(IDistributedCache cache, IDataRepository<Goods> goodsDac)
        {
            this.cache = cache;
            this.goodsDac = goodsDac;
        }

        [HttpGet("api/[controller]/[action]/{id}")]
        public Goods Get(string id)
        {
            var goods = goodsDac.Get(g => g.Id == id);
            return goods;
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

        public IActionResult Create()
        {
            var username = cache.GetString(Utility.username_key);
            if (string.IsNullOrEmpty(username))
            {
                TempData["message"] = "Please login.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.username = username;
            return View();
        }

        [HttpPost]
        public IActionResult Create(Goods goods)
        {
            try
            {
                var username = cache.GetString(Utility.username_key);
                if (string.IsNullOrEmpty(username))
                {
                    TempData["message"] = "Please login.";
                    return RedirectToAction("Index", "Home");
                }

                goods.Id = Guid.NewGuid().ToString();
                goods.CreateDate = DateTime.UtcNow;
                goods.CreateBy = username;
                goodsDac.Create(goods);

                ViewBag.username = username;
                TempData["message"] = "บันทึกสำเร็จ";
            }
            catch (Exception ex)
            {
                TempData["message"] = "Error: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
