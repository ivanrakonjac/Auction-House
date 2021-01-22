using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionHouse.Models.Database;
using AuctionHouse.Models.View;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace AuctionHouse.Controllers
{
    public class AuctionController : Controller
    {
        private readonly AuctionHouseContext _context;

        public AuctionController(AuctionHouseContext context)
        {
            _context = context;
        }

        // GET: Auction
        public async Task<IActionResult> Index()
        {
            var auctionHouseContext = _context.auctions.Include(a => a.owner).Include(a => a.winner);
            return View(await auctionHouseContext.ToListAsync());
        }

        // GET: Auction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.auctions
                .Include(a => a.owner)
                .Include(a => a.winner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // GET: Auction/Create
        public IActionResult Create()
        {
            return View();
        }

        // Metoda za validaciju closeDatea ( closeDate mora piti posle openDatea  )
        public bool isCloseDateOk ( DateTime openDate, DateTime closeDate){

            int result = DateTime.Compare(closeDate, openDate);

            if (result <= 0)
                return false;
            else
                return true;
                
        }

        // POST: Auction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuctionModel createAuctionModel)
        {
            if (!ModelState.IsValid)
            {
                return View (createAuctionModel);
            }

            if( !isCloseDateOk ( createAuctionModel.openDate, createAuctionModel.closeDate ) ){
                ModelState.AddModelError ("closeDate", "Close Date must be after Open Date");
                return View (createAuctionModel);
            }

            Auction auction = new Auction ( ){
                name = createAuctionModel.name,
                description = createAuctionModel.description,
                startPrice = createAuctionModel.startPrice,
                currentPrice = createAuctionModel.startPrice,
                createDate = DateTime.Now,
                openDate = createAuctionModel.openDate,
                closeDate = createAuctionModel.closeDate,
                state = Auction.AuctionState.DRAFT
            };

            
            using ( BinaryReader reader = new BinaryReader ( createAuctionModel.image.OpenReadStream ( ) ) ) {
                auction.image = reader.ReadBytes ( Convert.ToInt32 ( reader.BaseStream.Length ) );
            };

            _context.Add(auction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Auction/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            ViewData["ownerId"] = new SelectList(_context.Users, "Id", "Id", auction.ownerId);
            ViewData["winnerId"] = new SelectList(_context.Users, "Id", "Id", auction.winnerId);
            return View(auction);
        }

        // POST: Auction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,image,startPrice,currentPrice,createDate,openDate,closeDate,state,RowVersion,ownerId,winnerId")] Auction auction)
        {
            if (id != auction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionExists(auction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ownerId"] = new SelectList(_context.Users, "Id", "Id", auction.ownerId);
            ViewData["winnerId"] = new SelectList(_context.Users, "Id", "Id", auction.winnerId);
            return View(auction);
        }

        // GET: Auction/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.auctions
                .Include(a => a.owner)
                .Include(a => a.winner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // POST: Auction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auction = await _context.auctions.FindAsync(id);
            _context.auctions.Remove(auction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionExists(int id)
        {
            return _context.auctions.Any(e => e.Id == id);
        }
    }
}
