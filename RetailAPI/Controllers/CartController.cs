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
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        public CartController(CartService cartService) { _cartService = cartService; }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync(GetUserId());
            return Ok(new { success = true, message = "Cart fetched.", data = cart });
        }

        // POST api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var cart = await _cartService.AddToCartAsync(GetUserId(), dto);
            return Ok(new { success = true, message = "Item added to cart.", data = cart });
        }

        // PUT api/cart/item/{cartItemId}
        [HttpPut("item/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(int cartItemId, [FromBody] UpdateCartItemDto dto)
        {
            var cart = await _cartService.UpdateCartItemAsync(GetUserId(), cartItemId, dto);
            return Ok(new { success = true, message = "Cart updated.", data = cart });
        }

        // DELETE api/cart/item/{cartItemId}
        [HttpDelete("item/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            var cart = await _cartService.RemoveFromCartAsync(GetUserId(), cartItemId);
            return Ok(new { success = true, message = "Item removed.", data = cart });
        }

        // DELETE api/cart/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(GetUserId());
            return Ok(new { success = true, message = "Cart cleared." });
        }
    }
}
