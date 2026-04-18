using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.DTOs.AdminDTOs;
using RetailAPI.Models;

namespace RetailAPI.Services
{
    public class CouponService
    {
        private readonly AppDbContext _context;

        public CouponService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _context.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return coupons.Select(MapToDto).ToList();
        }

        public async Task<CouponDto?> GetCouponByIdAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            return coupon == null ? null : MapToDto(coupon);
        }

        public async Task<(bool success, string message, CouponDto? dto)> CreateCouponAsync(CreateCouponDto dto)
        {
            var exists = await _context.Coupons.AnyAsync(c => c.Code == dto.Code.ToUpper());
            if (exists) return (false, "Coupon code already exists.", null);

            var coupon = new Coupon
            {
                Code = dto.Code.ToUpper(),
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderAmount = dto.MinOrderAmount,
                MaxUsageCount = dto.MaxUsageCount,
                ExpiresAt = dto.ExpiresAt,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return (true, "Coupon created.", MapToDto(coupon));
        }

        public async Task<(bool success, string message)> UpdateCouponAsync(int id, UpdateCouponDto dto)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return (false, "Coupon not found.");

            if (dto.DiscountType != null) coupon.DiscountType = dto.DiscountType;
            if (dto.DiscountValue.HasValue) coupon.DiscountValue = dto.DiscountValue.Value;
            if (dto.MinOrderAmount.HasValue) coupon.MinOrderAmount = dto.MinOrderAmount;
            if (dto.MaxUsageCount.HasValue) coupon.MaxUsageCount = dto.MaxUsageCount;
            if (dto.IsActive.HasValue) coupon.IsActive = dto.IsActive.Value;
            if (dto.ExpiresAt.HasValue) coupon.ExpiresAt = dto.ExpiresAt;

            await _context.SaveChangesAsync();
            return (true, "Coupon updated.");
        }

        public async Task<(bool success, string message)> DeleteCouponAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return (false, "Coupon not found.");

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return (true, "Coupon deleted.");
        }

        public async Task<(bool valid, string message, decimal discount)> ValidateCouponAsync(string code, decimal orderAmount)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code.ToUpper() && c.IsActive);
            if (coupon == null) return (false, "Invalid or inactive coupon.", 0);

            if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt < DateTime.UtcNow)
                return (false, "Coupon has expired.", 0);

            if (coupon.MaxUsageCount.HasValue && coupon.UsedCount >= coupon.MaxUsageCount)
                return (false, "Coupon usage limit reached.", 0);

            if (coupon.MinOrderAmount.HasValue && orderAmount < coupon.MinOrderAmount)
                return (false, $"Minimum order amount is {coupon.MinOrderAmount:C}.", 0);

            decimal discount = coupon.DiscountType == "Percentage"
                ? orderAmount * coupon.DiscountValue / 100
                : coupon.DiscountValue;

            return (true, "Coupon is valid.", Math.Min(discount, orderAmount));
        }

        private static CouponDto MapToDto(Coupon c) => new CouponDto
        {
            CouponId = c.CouponId,
            Code = c.Code,
            DiscountType = c.DiscountType,
            DiscountValue = c.DiscountValue,
            MinOrderAmount = c.MinOrderAmount,
            MaxUsageCount = c.MaxUsageCount,
            UsedCount = c.UsedCount,
            IsActive = c.IsActive,
            ExpiresAt = c.ExpiresAt,
            CreatedAt = c.CreatedAt
        };
    }
}
