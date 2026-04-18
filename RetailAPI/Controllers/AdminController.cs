using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailAPI.DTOs.AdminDTOs;
using RetailAPI.Services;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        // GET api/admin/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return Ok(new { success = true, message = "Dashboard stats retrieved.", data = stats });
        }

        // GET api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(new { success = true, message = "Users retrieved.", data = users });
        }

        // PATCH api/admin/users/{id}/status
        [HttpPatch("users/{id}/status")]
        public async Task<IActionResult> ToggleUserStatus(int id, [FromBody] UpdateUserStatusDto dto)
        {
            var result = await _adminService.ToggleUserStatusAsync(id, dto.IsActive);
            if (!result) return NotFound(new { success = false, message = "User not found." });
            return Ok(new { success = true, message = $"User status updated to {(dto.IsActive ? "Active" : "Inactive")}." });
        }

        // DELETE api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result) return NotFound(new { success = false, message = "User not found." });
            return Ok(new { success = true, message = "User deleted." });
        }

        // GET api/admin/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _adminService.GetAllOrdersAsync();
            return Ok(new { success = true, message = "Orders retrieved.", data = orders });
        }

        // GET api/admin/orders/{id}
        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _adminService.GetOrderByIdAsync(id);
            if (order == null) return NotFound(new { success = false, message = "Order not found." });
            return Ok(new { success = true, message = "Order retrieved.", data = order });
        }

        // PATCH api/admin/orders/{id}/status
        [HttpPatch("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid status value." });

            var result = await _adminService.UpdateOrderStatusAsync(id, dto.Status);
            if (!result) return NotFound(new { success = false, message = "Order not found." });
            return Ok(new { success = true, message = "Order status updated." });
        }
    }
}
