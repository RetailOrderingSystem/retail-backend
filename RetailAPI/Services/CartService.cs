using RetailAPI.DTOs.OrderDTOs;
using RetailAPI.Models;
using RetailAPI.Repositories;

namespace RetailAPI.Services
{
    public class CartService
    {
        private readonly CartRepository _cartRepo;

        public CartService(CartRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }

        public async Task<CartResponseDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId)
                       ?? await _cartRepo.CreateCartAsync(userId);

            return MapToDto(cart);
        }

        public async Task<CartResponseDto> AddToCartAsync(int userId, AddToCartDto dto)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId)
                       ?? await _cartRepo.CreateCartAsync(userId);

            var existing = await _cartRepo.GetCartItemAsync(cart.CartId, dto.ProductId);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                await _cartRepo.UpdateCartItemAsync(existing);
            }
            else
            {
                // Fetch price from DB (do not trust client)
                var productPrice = await GetProductPriceAsync(dto.ProductId);
                var item = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = productPrice
                };
                await _cartRepo.AddCartItemAsync(item);
            }

            var updated = await _cartRepo.GetCartByUserIdAsync(userId);
            return MapToDto(updated!);
        }

        public async Task<CartResponseDto> UpdateCartItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            var item = await _cartRepo.GetCartItemByIdAsync(cartItemId)
                ?? throw new Exception("Cart item not found.");

            if (dto.Quantity <= 0)
            {
                await _cartRepo.RemoveCartItemAsync(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
                await _cartRepo.UpdateCartItemAsync(item);
            }

            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            return MapToDto(cart!);
        }

        public async Task<CartResponseDto> RemoveFromCartAsync(int userId, int cartItemId)
        {
            var item = await _cartRepo.GetCartItemByIdAsync(cartItemId)
                ?? throw new Exception("Cart item not found.");
            await _cartRepo.RemoveCartItemAsync(item);
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            return MapToDto(cart!);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId);
            if (cart != null)
                await _cartRepo.ClearCartAsync(cart.CartId);
        }

        private async Task<decimal> GetProductPriceAsync(int productId)
        {
            // Price lookup via EF via CartRepository context
            return await _cartRepo.GetProductPriceAsync(productId);
        }

        private CartResponseDto MapToDto(Cart cart)
        {
            var items = cart.CartItems.Select(ci => new CartItemResponseDto
            {
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                ProductName = ci.Product?.Name ?? "",
                ImageUrl = ci.Product?.ImageUrl,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice,
                SubTotal = ci.Quantity * ci.UnitPrice
            }).ToList();

            return new CartResponseDto
            {
                CartId = cart.CartId,
                Items = items,
                TotalAmount = items.Sum(i => i.SubTotal),
                TotalItems = items.Sum(i => i.Quantity)
            };
        }
    }
}
