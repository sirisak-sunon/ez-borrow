using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ez.Borrow.Models;
using Microsoft.Extensions.Caching.Distributed;
using Ez.Borrow.Repositories;
using Microsoft.AspNetCore.Http;

namespace Ez.Borrow.Controllers
{
    //[ApiExplorerSettings(IgnoreApi = true)]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[Controller]")]
    public class BorrowController : Controller
    {
        private readonly IDistributedCache cache;
        private readonly IDataRepository<Goods> goodsDac;
        private readonly IDataRepository<BorrowLog> borrowDac;

        public BorrowController(
            IDistributedCache cache,
            IDataRepository<Goods> goodsDac,
            IDataRepository<BorrowLog> borrowDac
            )
        {
            this.cache = cache;
            this.goodsDac = goodsDac;
            this.borrowDac = borrowDac;
        }

        [HttpGet("[action]/{id}")]
        public BorrowLog Get(string id)
        {
            var borrow = borrowDac.Get(g => g.Id == id);
            return borrow;
        }

        [HttpGet("[action]")]
        public IEnumerable<Goods> List()
        {
            var goodsList = goodsDac.List(g => true);
            return goodsList;
        }

        [HttpGet("[action]/{username}")]
        public IEnumerable<BorrowLog> ListBorrow(string username)
        {
            var borrowList = borrowDac.List(b => (b.Borrower == username || b.Witness == username) && b.WitnessConfirmDate.HasValue);
            return borrowList;
        }

        [HttpPost("[action]/{username}/{id}")]
        public RequestResponse BorrowRequest(string username, string id)
        {
            try
            {
                var goods = goodsDac.Get(d => d.Id == id);
                var borrow = new BorrowLog
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.UtcNow,
                    Borrower = username,
                    BorrowGoods = new List<Goods>
                    {
                        goods
                    }
                };
                borrowDac.Create(borrow);
                return new RequestResponse
                {
                    Code = 200,
                    Message = borrow.Id,
                };
            }
            catch (Exception ex)
            {
                return new RequestResponse
                {
                    Code = 500,
                    Message = "Error: " + ex.Message,
                };
            }
        }

        [HttpPost("[action]/{username}/{id}")]
        public RequestResponse BorrowConfirm(string username, string id)
        {
            try
            {
                var borrow = borrowDac.Get(b => b.Id == id);
                borrow.Witness = username;
                borrow.WitnessConfirmDate = DateTime.UtcNow;
                borrowDac.UpdateOne(b => b.Id == id, borrow);
                return new RequestResponse
                {
                    Code = 200,
                    Message = borrow.Id,
                };
            }
            catch (Exception ex)
            {
                return new RequestResponse
                {
                    Code = 500,
                    Message = "Error: " + ex.Message,
                };
            }
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
