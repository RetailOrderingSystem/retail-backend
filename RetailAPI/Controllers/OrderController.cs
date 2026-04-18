using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailAPI.DTOs.OrderDTOs;
using RetailAPI.Services;
using System.Security.Claims;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService) { _orderService = orderService; }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST api/order/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderCreateDto dto)
        {
            var order = await _orderService.PlaceOrderAsync(GetUserId(), dto);
            return Ok(new { success = true, message = "Order placed successfully.", data = order });
        }

        // GET api/order/{orderId}
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrderAsync(orderId, GetUserId());
            return Ok(new { success = true, message = "Order fetched.", data = order });
        }

        // GET api/order/history
        [HttpGet("history")]
        public async Task<IActionResult> OrderHistory()
        {
            var orders = await _orderService.GetOrderHistoryAsync(GetUserId());
            return Ok(new { success = true, message = "Order history.", data = orders });
        }

        // POST api/order/{orderId}/reorder
        [HttpPost("{orderId}/reorder")]
        public async Task<IActionResult> Reorder(int orderId)
        {
            var order = await _orderService.ReorderAsync(orderId, GetUserId());
            return Ok(new { success = true, message = "Reorder placed.", data = order });
        }
    }
}
