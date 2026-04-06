using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Winzcart.Application.DTOs.User;
using Winzcart.Application.Interfaces.Services;

namespace Winzcart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IPointsService _pointsService;

    public UserController(IUserService userService, IPointsService pointsService)
    {
        _userService = userService;
        _pointsService = pointsService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var response = await _userService.GetProfileAsync(GetUserId());
        return Ok(response);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        await _userService.UpdateProfileAsync(GetUserId(), request);
        return NoContent();
    }

    [HttpGet("points")]
    public async Task<IActionResult> GetPointsBalance()
    {
        var response = await _pointsService.GetBalanceAsync(GetUserId());
        return Ok(response);
    }

    [HttpGet("points/history")]
    public async Task<IActionResult> GetPointsHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _pointsService.GetHistoryAsync(GetUserId(), page, pageSize);
        return Ok(response);
    }
}
