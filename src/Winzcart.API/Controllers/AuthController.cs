using Microsoft.AspNetCore.Mvc;
using Winzcart.Application.DTOs.Auth;
using Winzcart.Application.Interfaces.Services;

namespace Winzcart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMerchantService _merchantService;

    public AuthController(IUserService userService, IMerchantService merchantService)
    {
        _userService = userService;
        _merchantService = merchantService;
    }

    [HttpPost("register/customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterRequest request)
    {
        var response = await _userService.RegisterCustomerAsync(request);
        return Ok(response);
    }

    [HttpPost("register/merchant")]
    public async Task<IActionResult> RegisterMerchant([FromBody] MerchantRegisterRequest request)
    {
        var response = await _merchantService.RegisterMerchantAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userService.LoginAsync(request);
        return Ok(response);
    }
}
