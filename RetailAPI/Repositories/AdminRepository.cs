using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.Models;

namespace RetailAPI.Repositories
{
    public class AdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        // ─── Users ───────────────────────────────────────────────────────────
        public async Task<List<User>> GetAllUsersAsync()
            => await _context.Users.Include(u => u.Role).ToListAsync();

        public async Task<User?> GetUserByIdAsync(int id)
            => await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        // ─── Orders ──────────────────────────────────────────────────────────
        public async Task<List<Order>> GetAllOrdersAsync()
            => await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .Include(o => o.Coupon)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

        public async Task<Order?> GetOrderByIdAsync(int id)
            => await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Include(o => o.Payment)
                .Include(o => o.Coupon)
                .FirstOrDefaultAsync(o => o.OrderId == id);

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // ─── Dashboard Stats ─────────────────────────────────────────────────
        public async Task<int> GetTotalUsersCountAsync()
            => await _context.Users.CountAsync();

        public async Task<int> GetTotalOrdersCountAsync()
            => await _context.Orders.CountAsync();

        public async Task<decimal> GetTotalRevenueAsync()
            => await _context.Payments.Where(p => p.Status == "Success").SumAsync(p => p.Amount);

        public async Task<int> GetTotalProductsCountAsync()
            => await _context.Products.CountAsync();
    }
}
