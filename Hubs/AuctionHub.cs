using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AuctionHouse.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task NotifyUsers(int? carId, int? auctionId)
        {
            await Clients.All.SendAsync("UpdateAuction", carId, auctionId);
        }
    }
}