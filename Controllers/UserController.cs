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
using Microsoft.AspNetCore.Authorization;

namespace AuctionHouse.Controllers
{
    public class UserController : Controller
    {
        private AuctionHouseContext _context;
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private IMapper mapper;

        public UserController(AuctionHouseContext context, UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager)
        {
            _context = context;
            this.userManager = userManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
        }

        [Authorize (Roles = "Admin")]
        // GET: User
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        [Authorize (Roles = "Admin")]
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

        // Metoda koja sluzi za proveru emaila pri registraciji
        public IActionResult isEmailUnique( string email ){
            bool exists = this._context.Users.Where (user => user.Email == email).Any ( );

            if(exists){
                return Json("Email already taken");
            }else{
                return Json(true);
            }
        }

        //Metoda koja sluzi za proveru usernamea pri registraciji
        public IActionResult isUsernameUnique( string username ){
            bool exists = this._context.Users.Where (user => user.UserName == username).Any ( );

            if(exists){
                return Json("Username already taken");
            }else{
                return Json(true);
            }

        }

        public async Task<List<SelectListItem>> GetSelectListOfGenders ( ) {

            IEnumerable<Gender> genders = await this._context.genders.ToListAsync ( );

            List<SelectListItem> gendersSelectList = new List<SelectListItem> ( );

            gendersSelectList.Add ( new SelectListItem { Selected = true, Text = string.Empty, Value = "-1"} );

            foreach ( Gender gender in genders ) {
                gendersSelectList.Add ( new SelectListItem { Selected = false, Text = gender.name, Value = gender.name } );
            }

            return gendersSelectList;
        }

        // GET: User/Register
        public async Task<IActionResult> RegisterAsync()
        {
            IEnumerable<Gender> genders = await this._context.genders.ToListAsync ( );

            List<SelectListItem> gendersSelectList = new List<SelectListItem> ( );

            gendersSelectList.Add ( new SelectListItem { Selected = true, Text = string.Empty, Value = "-1"} );

            foreach ( Gender gender in genders ) {
                gendersSelectList.Add ( new SelectListItem { Selected = false, Text = gender.name, Value = gender.name } );
            }

            ViewData["genders"] = gendersSelectList;

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

            if(signInManager.IsSignedIn (User)){
                return RedirectToAction ( nameof (UserController.Index), "User" );
            }else{
                return RedirectToAction ( nameof (UserController.LogIn), "User" );
            }

            
        }

        // GET: User/Edit/5
        [Authorize (Roles = "Admin, User")]
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

            IEnumerable<Gender> genders = await this._context.genders.ToListAsync ( );

            List<SelectListItem> gendersSelectList = new List<SelectListItem> ( );

            int selectedIndex = (user.gender == "Muski" || user.gender == "muski") ? 0 : 1;

            for ( int i=0 ; i < genders.Count(); i++ ) {
               
               bool shouldSelect = selectedIndex == i ? true : false;

                gendersSelectList.Add ( 
                    new SelectListItem {  Selected = shouldSelect, Text = genders.ElementAt(i).name, Value = genders.ElementAt(i).name } 
                );
            }

            ViewData["genders"] = gendersSelectList;


            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin, User")]
        public async Task<IActionResult> Edit(string id, [Bind("firstName,lastName,gender,deletedByAdmin,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] User user)
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

                bool isAdmin = await this.userManager.IsInRoleAsync ( user, "Admin" );

                if(isAdmin){
                    return RedirectToAction ( nameof (UserController.Index), "User" );
                }else{
                    return RedirectToAction ( nameof (HomeController.Index), "Home" );
                }
                
            }
            return View(user);
        }

        // GET: User/Delete/5
        [Authorize (Roles = "Admin")]
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
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: User/DeleteByAdmin/5
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteByAdmin(string id)
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

        // POST: User/DeleteByAdminConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> DeleteByAdminConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if ( user != null ) {

                try {
                    user.deletedByAdmin = true;
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
            }

            return RedirectToAction( nameof( UserController.Index ), "User" );

        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        
        public IActionResult LogIn( string returnUrl ){
            LogInModel model = new LogInModel (){
                returnUrl = returnUrl
            };

            return View ( model );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn ( LogInModel model ){
            
            if ( !ModelState.IsValid ){
                return View ( model ); 
            }
            
            // Provera da user slucajno nije banovan ( userDeletedByAdmin == true )
            
            var user = this._context.Users.Where (user => user.UserName == model.username).FirstOrDefault ( );

            if ( user != null && user.deletedByAdmin == true){
                ModelState.AddModelError ("","Your account is banned. Bjezi odnas!");
                return View ( model );
            }


            // provera da li se password i username poklapaju

            var result = await this.signInManager.PasswordSignInAsync(model.username, model.password, false, false);

            if ( !result.Succeeded ){
                ModelState.AddModelError ("","Username or password is not valid. Zna li ti majka sto cinis?");
                return View ( model );
            }

            if(model.returnUrl != null){
                return Redirect ( model.returnUrl );
            }
            else{
                return RedirectToAction ( nameof (HomeController.Index), "Home" );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut(){
            await this.signInManager.SignOutAsync ( );
            return RedirectToAction (nameof ( HomeController.Index ), "Home");
        }

        
        public async Task<IActionResult> BuyTokens(string id){
            return View();
        }

        



    }
}
