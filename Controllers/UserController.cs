using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AuctionHouse.Models.Database;
using Microsoft.AspNetCore.Identity;
using AuctionHouse.Models.View;
using AutoMapper;
using static AuctionHouse.Models.Database.IdentityRoleConfiguration;

namespace AuctionHouse.Controllers
{
    public class UserController : Controller
    {
        private AuctionHouseContext _context;
        private UserManager<User> userManager;
        private IMapper mapper;

        public UserController(AuctionHouseContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public IActionResult isEmailUnique( string email ){
            bool exists = this._context.Users.Where (user => user.Email == email).Any ( );

            if(exists){
                return Json("Email already taken");
            }else{
                return Json(true);
            }
        }

        public IActionResult isUsernameUnique( string username ){
            bool exists = this._context.Users.Where (user => user.UserName == username).Any ( );

            if(exists){
                return Json("Username already taken");
            }else{
                return Json(true);
            }

        }

        // GET: User/Register
        public IActionResult Register()
        {
            ViewData["genders"] = new SelectList(
                new List<SelectListItem>
                {
                    new SelectListItem { Selected = true, Text = string.Empty, Value = "-1"},
                    new SelectListItem { Selected = false, Text = "Muski", Value = "muski"},
                    new SelectListItem { Selected = false, Text = "Zenski", Value = "zenski"},
                }, "Value" , "Text", 1);

            return View();
        }

        // POST: User/Register
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register( RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View ( registerModel );
            }

            User user = this.mapper.Map<User> ( registerModel );

            IdentityResult result = await this.userManager.CreateAsync(user, registerModel.password);

            if( !result.Succeeded ){
                foreach( IdentityError error in result.Errors ){
                    ModelState.AddModelError ("", error.Description);
                }

                return View ( registerModel );
            }

            result = await this.userManager.AddToRoleAsync( user, Roles.user.Name );

            if( !result.Succeeded ){
                foreach( IdentityError error in result.Errors ){
                    ModelState.AddModelError ("", error.Description);
                }

                return View ( registerModel );
            }

            return RedirectToAction ( nameof (HomeController.Index), "Home" );
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("firstName,lastName,gender,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
