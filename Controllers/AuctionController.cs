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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AuctionHouse.Controllers
{   
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly AuctionHouseContext _context;

        private UserManager<User> _userManager;

        private SignInManager<User> _signInManager;

        private int incCurrentPrice = 100;

        public AuctionController(AuctionHouseContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Auction
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var auctionHouseContext = _context.auctions.Include(a => a.owner).Include(a => a.winner).OrderByDescending ( a => a.createDate );
            return View( await auctionHouseContext.ToListAsync());
        }

        // GET: Auction/userId
        [Authorize (Roles = "User")]
        public async Task<IActionResult> MyAuctions(string id)
        {
            if (id == null){
                id = User.FindFirst ("id").Value;
            }

            var myAuctions = _context.auctions.Where (a => a.ownerId == id).Include(a => a.owner).Include(a => a.winner);
            return View(await myAuctions.ToListAsync());
        }

        // GET: Auction/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.auctions
                .Include(a => a.owner)
                .Include(a => a.winner)
                .Include(a => a.bids)
                .FirstOrDefaultAsync(m => m.Id == id);

            IEnumerable<Bid> bids = await _context.bids.Include (b => b.user).Where (b => b.auctionId == id).OrderByDescending (b => b.bidDate).Take(10).ToListAsync();
            ViewData["bids"] = bids;


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

        // POST: Auction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuctionModel auctionModel)
        {
            if (!ModelState.IsValid)
            {
                return View (auctionModel);
            }

            // Dohvatam ID ulogovanog Usera
            string loggedUserId = User.FindFirst ("id").Value;

            Auction auction = new Auction ( ){
                name = auctionModel.name,
                description = auctionModel.description,
                startPrice = auctionModel.startPrice,
                currentPrice = auctionModel.startPrice,
                createDate = DateTime.Now,
                openDate = auctionModel.openDate,
                closeDate = auctionModel.closeDate,
                state = Auction.AuctionState.DRAFT,
                ownerId = loggedUserId,
                owner = await _context.Users.FirstOrDefaultAsync ( u => u.Id.Equals(loggedUserId))
            };

            
            using ( BinaryReader reader = new BinaryReader ( auctionModel.image.OpenReadStream ( ) ) ) {
                auction.image = reader.ReadBytes ( Convert.ToInt32 ( reader.BaseStream.Length ) );
            };

            _context.Add(auction);
            await _context.SaveChangesAsync();

            if ( User.FindFirst (ClaimTypes.Role).Value == "Admin"){
                return RedirectToAction(nameof(Index));
            }else{
                return RedirectToAction(nameof(MyAuctions), "Auction");
            }

            
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
            
            EditAuctionModel editAuctionModel = new EditAuctionModel () {
                Id = auction.Id,
                name = auction.name,
                description = auction.description,
                base64Data = Convert.ToBase64String(auction.image),
                startPrice = auction.startPrice,
                openDate = auction.openDate,
                closeDate = auction.closeDate
            };

            return View( editAuctionModel );
        }

        
        // POST: Auction/Bid
        // Fukncija koja se poziva na click BID dugmeta 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Bid(int? id, byte[] rowVersion)
        {

            if (id == null)
            {
                return Json (false);
            }


            var auctionToUpdate = await _context.auctions.Include(a => a.winner).Include(a=> a.owner).FirstOrDefaultAsync(a => a.Id == id);

            if (auctionToUpdate == null)
            {
                return Json (false);
            }

            _context.Entry(auctionToUpdate).Property("RowVersion").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Auction>(auctionToUpdate, "", a => a.currentPrice)){
                try{

                    User loggedUser = await this._userManager.GetUserAsync(base.User);

                    if(loggedUser.tokens <= 0) return Json("ERROR");

                    auctionToUpdate.currentPrice += incCurrentPrice;
                    auctionToUpdate.winner = loggedUser;
                    auctionToUpdate.winnerId = loggedUser.Id;

                    Bid bid = new Bid () {
                        price = auctionToUpdate.currentPrice,
                        bidDate = DateTime.Now,
                        user = loggedUser,
                        userId = loggedUser.Id,
                        auctionId = auctionToUpdate.Id
                    };

                    _context.Add(bid);

                    loggedUser.tokens -= 1;

                    await _context.SaveChangesAsync();

                    auctionToUpdate = await _context.auctions.Include(a => a.winner).Include(a => a.bids).FirstOrDefaultAsync(a => a.Id == id);               
    
                    UpdatedAuction resultAuction = new UpdatedAuction () {
                        currentPrice = auctionToUpdate.currentPrice,
                        RowVersion = auctionToUpdate.RowVersion,
                        winnerUsername = auctionToUpdate.winner.UserName,
                        numberOfBids = auctionToUpdate.bids.Count()
                    };

                    return Json(resultAuction);
                }
                catch (DbUpdateConcurrencyException ex) {
                    return Json (ex);
                }
            }


            return Json(false);

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> getAuctionUpdateData(int? id)
        {
            var auctionToUpdate = await _context.auctions.Include(a => a.winner).FirstOrDefaultAsync(a => a.Id == id);

            var bids = await _context.bids.Where(b => b.auctionId == auctionToUpdate.Id).ToListAsync();

            UpdatedAuction resultAuction = new UpdatedAuction () {
                currentPrice = auctionToUpdate.currentPrice,
                RowVersion = auctionToUpdate.RowVersion,
                winnerUsername = auctionToUpdate.winner.UserName,
                numberOfBids = bids.Count
            };


            return Json(resultAuction);
        }

        // POST: Auction/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAuctionModel auctionModel)
        {
            
            var auction = await _context.auctions.FindAsync(id);
            

            if (id != auction.Id)
            {
                return NotFound();
            }

            if ( auction.state != Auction.AuctionState.DRAFT) {
                auctionModel.base64Data = Convert.ToBase64String(auction.image);
                ModelState.AddModelError ("", "This auction can not be edited.");
                ModelState.AddModelError ("", "Sorry!");
                return View (auctionModel);
            }

            if (ModelState.IsValid)
            {
                try
                {

                    auction.name = auctionModel.name;
                    auction.description = auctionModel.description;
                    auction.startPrice = auctionModel.startPrice;
                    auction.currentPrice = auctionModel.startPrice;
                    auction.openDate = auctionModel.openDate;
                    auction.closeDate = auctionModel.closeDate;

                    if ( auctionModel.image != null ) {
                        using ( BinaryReader reader = new BinaryReader ( auctionModel.image.OpenReadStream ( ) ) ) {
                            auction.image = reader.ReadBytes ( Convert.ToInt32 ( reader.BaseStream.Length ) );
                        };
                    }
            
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

                if ( User.FindFirst (ClaimTypes.Role).Value == "Admin"){
                    return RedirectToAction(nameof(Index));
                }else{
                    return RedirectToAction(nameof(MyAuctions), "Auction");
                }
            }
            
            return View(auctionModel);
        }

        // GET: Auction/Delete/5
        [Authorize (Roles = "Admin")]
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
        [Authorize (Roles = "Admin")]
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

        // Metoda za validaciju closeDatea ( closeDate mora piti posle openDatea  )
        public bool isCloseDateOk ( DateTime openDate, DateTime closeDate){

            int result = DateTime.Compare(closeDate, openDate);

            if (result <= 0)
                return false;
            else
                return true;
                
        }


         // Metoda za validaciju openDatea ( openDate mora bar danas  )
        public bool isOpenDateOk ( DateTime openDate){

            var result = DateTime.Now - openDate;

            if (result.Days == 0)
                return true;
            
            int res = DateTime.Compare(openDate, DateTime.Now);

            if (res >= 0)
                return true;
            else
                return false;
                
        }

        // POST: Auction/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            
            var auction = await _context.auctions.FindAsync(id);

            try
            {
                auction.state = Auction.AuctionState.READY;

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

        // POST: Auction/SetStateToDeleted/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> SetStateToDeleted(int id)
        {
            
            var auction = await _context.auctions.FindAsync(id);

            try
            {
                auction.state = Auction.AuctionState.DELETED;

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


        // GET: Searched Auctions
        [AllowAnonymous]
        public async Task<IActionResult> Search ( string searchString, string minPrice, string maxPrice, string status, int? pageNumber )
        {

            ICollection<Auction> auctionsList =  _context.auctions.Where(a => a.state == Auction.AuctionState.READY).ToList();

            foreach (var auction in auctionsList) {
                int res = DateTime.Compare(DateTime.Now, auction.openDate);
                if(res > 0){
                    auction.state = Auction.AuctionState.OPEN;
                    _context.Update(auction);
                }
                
            }
            await _context.SaveChangesAsync();

            ViewData["CurrentFilter"] = searchString;
            int pageSize = 6;

            var auctions = from a in _context.auctions select a;

            if (pageNumber == null || pageNumber == 0)
            {
                pageNumber = 1;
            }


            if (!String.IsNullOrEmpty(searchString))
            {
                auctions = auctions.Where(a => a.name.Contains(searchString) || a.description.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(minPrice))
            {
                auctions = auctions.Where(a => a.currentPrice >= Int32.Parse(minPrice));
            }

            if (!String.IsNullOrEmpty(maxPrice))
            {
                auctions = auctions.Where(a => a.currentPrice <= Int32.Parse(maxPrice));
            }

            if (!String.IsNullOrEmpty(status))
            {
                if( status == "open" ){
                    auctions = auctions.Where(a => a.state == Auction.AuctionState.OPEN);
                }
                else if( status == "sold" ){
                    auctions = auctions.Where(a => a.state == Auction.AuctionState.SOLD);
                }
                else if( status == "expired" ){
                    auctions = auctions.Where(a => a.state == Auction.AuctionState.EXPIRED);
                }
                else if( status == "draft" ){
                    auctions = auctions.Where(a => a.state == Auction.AuctionState.DRAFT);
                }
                else if( status == "deleted" ){
                    auctions = auctions.Where(a => a.state == Auction.AuctionState.DELETED);
                }
                else if( status == "ready" ){
                    auctions = auctions.Where(a => a.state == Auction.AuctionState.READY);
                }
                else if( status == "which_I_bought" ){
                    string loggedUserId = User.FindFirst ("id").Value;
                    auctions = auctions.Where(a => a.winnerId == loggedUserId).Where(a => a.state == Auction.AuctionState.SOLD);
                }
                

            }

            if (String.IsNullOrEmpty(searchString) && String.IsNullOrEmpty(minPrice) && String.IsNullOrEmpty(maxPrice) && status == "-1"){
                auctions = auctions.Where(a => a.state == Auction.AuctionState.OPEN);
            }

            auctions = auctions.Include(a => a.winner).Include(a => a.bids).OrderByDescending (a => a.createDate);

            SearchedAuctionsModel searchedAuctionsModel = new SearchedAuctionsModel () {
                auctions = await auctions.AsNoTracking().ToListAsync()
            };
            
            return PartialView("SearchedAuctions", await PaginatedList<Auction>.CreateAsync(auctions.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        [AllowAnonymous]
        public async Task<Boolean> FinishTheAuction (int auctionId) {

            Auction auction = await _context.auctions.FirstAsync(a => a.Id == auctionId);

            if (auction.state == Auction.AuctionState.OPEN) {
                Bid bid = await _context.bids.FirstOrDefaultAsync(b => b.auctionId == auctionId);

                if (bid != null) {
                    auction.state = Auction.AuctionState.SOLD;
                }else {
                    auction.state = Auction.AuctionState.EXPIRED;
                }

                _context.Update(auction);
                await _context.SaveChangesAsync();
            } 

            return true;
        }


    }
}
/*
ViewData["ownerId"] = new SelectList(_context.Users, "Id", "Id", auction.ownerId);
*/