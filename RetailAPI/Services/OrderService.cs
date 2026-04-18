using RetailAPI.DTOs.OrderDTOs;
using RetailAPI.Models;
using RetailAPI.Repositories;

namespace RetailAPI.Services
{
    public class OrderService
    {
        private readonly OrderRepository _orderRepo;
        private readonly CartRepository _cartRepo;

        public OrderService(OrderRepository orderRepo, CartRepository cartRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
        }

        public async Task<OrderResponseDto> PlaceOrderAsync(int userId, OrderCreateDto dto)
        {
            var cart = await _cartRepo.GetCartByUserIdAsync(userId)
                ?? throw new Exception("Cart is empty.");

            if (!cart.CartItems.Any())
                throw new Exception("Cannot place order with empty cart.");

            var order = new Order
            {
                UserId = userId,
                DeliveryAddress = dto.DeliveryAddress,
                Notes = dto.Notes,
                Status = OrderStatus.Pending,
                DeliveryFee = 30,
                Discount = 0,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.UnitPrice) + 30,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice,
                    SubTotal = ci.Quantity * ci.UnitPrice
                }).ToList()
            };

            var created = await _orderRepo.CreateOrderAsync(order);
            await _cartRepo.ClearCartAsync(cart.CartId);

            var full = await _orderRepo.GetOrderByIdAsync(created.OrderId);
            return MapToDto(full!);
        }

        public async Task<OrderResponseDto> GetOrderAsync(int orderId, int userId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new Exception("Order not found.");
            if (order.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");
            return MapToDto(order);
        }

        public async Task<List<OrderResponseDto>> GetOrderHistoryAsync(int userId)
        {
            var orders = await _orderRepo.GetOrdersByUserIdAsync(userId);
            return orders.Select(MapToDto).ToList();
        }

        public async Task<OrderResponseDto> ReorderAsync(int orderId, int userId)
        {
            var original = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new Exception("Order not found.");
            if (original.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            var cart = await _cartRepo.GetCartByUserIdAsync(userId)
                       ?? await _cartRepo.CreateCartAsync(userId);
            await _cartRepo.ClearCartAsync(cart.CartId);

            foreach (var item in original.OrderItems)
            {
                var ci = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                await _cartRepo.AddCartItemAsync(ci);
            }

            var dto = new OrderCreateDto
            {
                DeliveryAddress = original.DeliveryAddress,
                Notes = "Reorder of #" + orderId
            };
            return await PlaceOrderAsync(userId, dto);
        }

        private OrderResponseDto MapToDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderId = order.OrderId,
                Status = order.Status.ToString(),
                DeliveryAddress = order.DeliveryAddress,
                TotalAmount = order.TotalAmount,
                DeliveryFee = order.DeliveryFee,
                Discount = order.Discount,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "",
                    ImageUrl = oi.Product?.ImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SubTotal = oi.SubTotal
                }).ToList()
            };
        }
    }
}
