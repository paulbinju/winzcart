using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Winzcart.Application.DTOs.Coupon;
using Winzcart.Application.Interfaces.Services;

namespace Winzcart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Merchant")]
public class MerchantController : ControllerBase
{
    private readonly IMerchantService _merchantService;
    private readonly ICouponService _couponService;

    public MerchantController(IMerchantService merchantService, ICouponService couponService)
    {
        _merchantService = merchantService;
        _couponService = couponService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var response = await _merchantService.GetProfileAsync(GetUserId());
        return Ok(response);
    }

    [HttpPost("coupons")]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponRequest request)
    {
        var response = await _couponService.CreateCouponAsync(GetUserId(), request);
        return CreatedAtAction(nameof(GetMerchantCoupons), null, response);
    }

    [HttpGet("coupons")]
    public async Task<IActionResult> GetMerchantCoupons()
    {
        var response = await _couponService.GetMerchantCouponsAsync(GetUserId());
        return Ok(response);
    }

    [HttpPost("coupons/validate")]
    public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponRequest request)
    {
        var response = await _couponService.ValidateCouponAsync(GetUserId(), request);
        return Ok(response);
    }
}
