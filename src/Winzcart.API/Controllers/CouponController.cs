using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Winzcart.Application.DTOs.Coupon;
using Winzcart.Application.Interfaces.Services;

namespace Winzcart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableCoupons()
    {
        var response = await _couponService.GetAvailableCouponsAsync();
        return Ok(response);
    }

    [HttpPost("redeem")]
    public async Task<IActionResult> RedeemCoupon([FromBody] RedeemCouponRequest request)
    {
        var response = await _couponService.RedeemCouponAsync(GetUserId(), request);
        return Ok(response);
    }

    [HttpGet("my-coupons")]
    public async Task<IActionResult> GetMyCoupons()
    {
        var response = await _couponService.GetUserCouponsAsync(GetUserId());
        return Ok(response);
    }
}
