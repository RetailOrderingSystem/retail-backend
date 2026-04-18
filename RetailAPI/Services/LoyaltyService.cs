using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.Models;

namespace RetailAPI.Services
{
    public class LoyaltyService
    {
        private readonly AppDbContext _context;
        private const int PointsPerRupee = 1;       // 1 point per ₹10 spent
        private const int RupeePerPoint = 10;        // every ₹10 = 1 point

        public LoyaltyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUserPointsBalanceAsync(int userId)
        {
            var earned = await _context.LoyaltyPoints
                .Where(lp => lp.UserId == userId && lp.Type == "Earned")
                .SumAsync(lp => lp.Points);

            var redeemed = await _context.LoyaltyPoints
                .Where(lp => lp.UserId == userId && lp.Type == "Redeemed")
                .SumAsync(lp => lp.Points);

            return earned - redeemed;
        }

        public async Task<List<LoyaltyPoint>> GetUserPointsHistoryAsync(int userId)
            => await _context.LoyaltyPoints
                .Where(lp => lp.UserId == userId)
                .OrderByDescending(lp => lp.CreatedAt)
                .ToListAsync();

        public async Task EarnPointsForOrderAsync(int userId, int orderId, decimal orderAmount)
        {
            int points = (int)(orderAmount / RupeePerPoint) * PointsPerRupee;
            if (points <= 0) return;

            _context.LoyaltyPoints.Add(new LoyaltyPoint
            {
                UserId = userId,
                Points = points,
                Type = "Earned",
                Description = $"Earned for Order #{orderId}",
                OrderId = orderId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }

        public async Task<(bool success, string message)> RedeemPointsAsync(int userId, int points)
        {
            int balance = await GetUserPointsBalanceAsync(userId);
            if (balance < points) return (false, $"Insufficient points. Balance: {balance}");

            _context.LoyaltyPoints.Add(new LoyaltyPoint
            {
                UserId = userId,
                Points = points,
                Type = "Redeemed",
                Description = $"Redeemed {points} points",
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return (true, $"Successfully redeemed {points} points.");
        }
    }
}
