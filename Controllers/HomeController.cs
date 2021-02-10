using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AuctionHouse.Models;
using AuctionHouse.Models.Database;

namespace AuctionHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AuctionHouseContext _context;

        public HomeController(AuctionHouseContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            /*ICollection<Auction> auctions =  _context.auctions.Where(a => a.state == Auction.AuctionState.READY)
                                                            .Where(a => a.openDate < DateTime.Now).ToList();

            foreach (var auction in auctions) {
                auction.state = Auction.AuctionState.OPEN;
                _context.Update(auction);
            }
            _context.SaveChangesAsync();*/

            return View();
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
