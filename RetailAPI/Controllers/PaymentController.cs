using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailAPI.DTOs.AdminDTOs;
using RetailAPI.Services;
using System.Security.Claims;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly CouponService _couponService;
        private readonly LoyaltyService _loyaltyService;

        public PaymentController(
            PaymentService paymentService,
            CouponService couponService,
            LoyaltyService loyaltyService)
        {
            _paymentService = paymentService;
            _couponService = couponService;
            _loyaltyService = loyaltyService;
        }

        // ─── Payments ─────────────────────────────────────────────────────────

        // GET api/payment  [Admin only]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(new { success = true, message = "Payments retrieved.", data = payments });
        }

        // GET api/payment/order/{orderId}
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrder(int orderId)
        {
            var payment = await _paymentService.GetPaymentByOrderIdAsync(orderId);
            if (payment == null) return NotFound(new { success = false, message = "Payment not found." });
            return Ok(new { success = true, message = "Payment retrieved.", data = payment });
        }

        // POST api/payment
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data." });

            var (success, message, payment) = await _paymentService.CreatePaymentAsync(dto);
            if (!success) return BadRequest(new { success = false, message });

            // Earn loyalty points
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _loyaltyService.EarnPointsForOrderAsync(userId, dto.OrderId, dto.Amount);

            return Ok(new { success = true, message, data = payment });
        }

        // PATCH api/payment/{id}/status  [Admin only]
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] UpdatePaymentStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid status." });

            var (success, message) = await _paymentService.UpdatePaymentStatusAsync(id, dto);
            if (!success) return NotFound(new { success = false, message });
            return Ok(new { success = true, message });
        }

        // ─── Coupons ──────────────────────────────────────────────────────────

        // GET api/payment/coupons  [Admin]
        [HttpGet("coupons")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCoupons()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return Ok(new { success = true, message = "Coupons retrieved.", data = coupons });
        }

        // GET api/payment/coupons/{id}  [Admin]
        [HttpGet("coupons/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            var coupon = await _couponService.GetCouponByIdAsync(id);
            if (coupon == null) return NotFound(new { success = false, message = "Coupon not found." });
            return Ok(new { success = true, message = "Coupon retrieved.", data = coupon });
        }

        // POST api/payment/coupons  [Admin]
        [HttpPost("coupons")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data." });

            var (success, message, coupon) = await _couponService.CreateCouponAsync(dto);
            if (!success) return BadRequest(new { success = false, message });
            return Ok(new { success = true, message, data = coupon });
        }

        // PUT api/payment/coupons/{id}  [Admin]
        [HttpPut("coupons/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDto dto)
        {
            var (success, message) = await _couponService.UpdateCouponAsync(id, dto);
            if (!success) return NotFound(new { success = false, message });
            return Ok(new { success = true, message });
        }

        // DELETE api/payment/coupons/{id}  [Admin]
        [HttpDelete("coupons/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            var (success, message) = await _couponService.DeleteCouponAsync(id);
            if (!success) return NotFound(new { success = false, message });
            return Ok(new { success = true, message });
        }

        // POST api/payment/coupons/validate  [All authenticated]
        [HttpPost("coupons/validate")]
        public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponDto dto)
        {
            var (valid, message, discount) = await _couponService.ValidateCouponAsync(dto.Code, dto.OrderAmount);
            return Ok(new { success = valid, message, data = new { discount } });
        }

        // ─── Loyalty Points ───────────────────────────────────────────────────

        // GET api/payment/loyalty/balance
        [HttpGet("loyalty/balance")]
        public async Task<IActionResult> GetLoyaltyBalance()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var balance = await _loyaltyService.GetUserPointsBalanceAsync(userId);
            return Ok(new { success = true, message = "Balance retrieved.", data = new { balance } });
        }

        // GET api/payment/loyalty/history
        [HttpGet("loyalty/history")]
        public async Task<IActionResult> GetLoyaltyHistory()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var history = await _loyaltyService.GetUserPointsHistoryAsync(userId);
            return Ok(new { success = true, message = "History retrieved.", data = history });
        }

        // POST api/payment/loyalty/redeem
        [HttpPost("loyalty/redeem")]
        public async Task<IActionResult> RedeemPoints([FromBody] int points)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var (success, message) = await _loyaltyService.RedeemPointsAsync(userId, points);
            if (!success) return BadRequest(new { success = false, message });
            return Ok(new { success = true, message });
        }
    }
}
