using RetailAPI.DTOs.AdminDTOs;
using RetailAPI.Models;
using RetailAPI.Repositories;

namespace RetailAPI.Services
{
    public class AdminService
    {
        private readonly AdminRepository _adminRepo;

        public AdminService(AdminRepository adminRepo)
        {
            _adminRepo = adminRepo;
        }

        // ─── Dashboard ────────────────────────────────────────────────────────
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            return new DashboardStatsDto
            {
                TotalUsers = await _adminRepo.GetTotalUsersCountAsync(),
                TotalOrders = await _adminRepo.GetTotalOrdersCountAsync(),
                TotalRevenue = await _adminRepo.GetTotalRevenueAsync(),
                TotalProducts = await _adminRepo.GetTotalProductsCountAsync()
            };
        }

        // ─── Users ────────────────────────────────────────────────────────────
        public async Task<List<AdminUserDto>> GetAllUsersAsync()
        {
            var users = await _adminRepo.GetAllUsersAsync();
            return users.Select(u => new AdminUserDto
            {
                UserId = u.UserId,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                RoleName = u.Role?.RoleName ?? "",
                IsActive = u.IsActive,
                IsEmailVerified = u.IsEmailVerified,
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public async Task<bool> ToggleUserStatusAsync(int userId, bool isActive)
        {
            var user = await _adminRepo.GetUserByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _adminRepo.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _adminRepo.GetUserByIdAsync(userId);
            if (user == null) return false;

            await _adminRepo.DeleteUserAsync(user);
            return true;
        }

        // ─── Orders ───────────────────────────────────────────────────────────
        public async Task<List<AdminOrderDto>> GetAllOrdersAsync()
        {
            var orders = await _adminRepo.GetAllOrdersAsync();
            return orders.Select(MapToAdminOrderDto).ToList();
        }

        public async Task<AdminOrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _adminRepo.GetOrderByIdAsync(orderId);
            return order == null ? null : MapToAdminOrderDto(order);
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _adminRepo.GetOrderByIdAsync(orderId);
            if (order == null) return false;

            order.Status = status;
            await _adminRepo.UpdateOrderAsync(order);
            return true;
        }

        private static AdminOrderDto MapToAdminOrderDto(Order o) => new AdminOrderDto
        {
            OrderId = o.OrderId,
            UserName = o.User?.FullName ?? "",
            UserEmail = o.User?.Email ?? "",
            TotalAmount = o.TotalAmount,
            Status = o.Status,
            PaymentStatus = o.Payment?.Status,
            PaymentMethod = o.Payment?.Method,
            CreatedAt = o.OrderDate,
            Items = o.OrderItems?.Select(oi => new AdminOrderItemDto
            {
                ProductName = oi.Product?.Name ?? "",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Subtotal = oi.SubTotal
            }).ToList() ?? new()
        };
    }
}
