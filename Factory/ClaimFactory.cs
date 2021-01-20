using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AuctionHouse.Models.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AuctionHouse.Factories{
    public class ClaimFactory : UserClaimsPrincipalFactory<User>
    {
        private UserManager<User> userManager;
        public ClaimFactory(UserManager<User> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            this.userManager = userManager;
        }

        public override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            return base.CreateAsync(user);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            ClaimsIdentity claimsIdentity = await base.GenerateClaimsAsync(user);

            claimsIdentity.AddClaim (
                new Claim ( "fullName", user.firstName + " " + user.lastName )
            );

            claimsIdentity.AddClaim (
                new Claim ( "id", user.Id )
            );

            IList<string> roles = await this.userManager.GetRolesAsync( user );

            foreach (string role in roles){
                claimsIdentity.AddClaim (
                    new Claim (ClaimTypes.Role, role)
                );
            }

            return claimsIdentity;
        }
    }
}