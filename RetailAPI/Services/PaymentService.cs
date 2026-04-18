using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.DTOs.AdminDTOs;
using RetailAPI.Models;

namespace RetailAPI.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaymentDto>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payments
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaidAt)
                .ToListAsync();
            return payments.Select(MapToDto).ToList();
        }

        public async Task<PaymentDto?> GetPaymentByOrderIdAsync(int orderId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            return payment == null ? null : MapToDto(payment);
        }

        public async Task<(bool success, string message, PaymentDto? dto)> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var order = await _context.Orders.FindAsync(dto.OrderId);
            if (order == null) return (false, "Order not found.", null);

            var exists = await _context.Payments.AnyAsync(p => p.OrderId == dto.OrderId);
            if (exists) return (false, "Payment already exists for this order.", null);

            var payment = new Payment
            {
                OrderId = dto.OrderId,
                Method = dto.Method,
                Amount = dto.Amount,
                Status = "Success",
                TransactionId = dto.TransactionId ?? Guid.NewGuid().ToString(),
                PaidAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            // Update order status
            order.Status = "Processing";
            await _context.SaveChangesAsync();

            return (true, "Payment recorded.", MapToDto(payment));
        }

        public async Task<(bool success, string message)> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentStatusDto dto)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return (false, "Payment not found.");

            payment.Status = dto.Status;
            await _context.SaveChangesAsync();
            return (true, "Payment status updated.");
        }

        private static PaymentDto MapToDto(Payment p) => new PaymentDto
        {
            PaymentId = p.PaymentId,
            OrderId = p.OrderId,
            Method = p.Method,
            Status = p.Status,
            Amount = p.Amount,
            TransactionId = p.TransactionId,
            PaidAt = p.PaidAt
        };
    }
}
