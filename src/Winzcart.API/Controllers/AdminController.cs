using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Winzcart.Application.DTOs.Admin;
using Winzcart.Application.DTOs.Bill;
using Winzcart.Application.Interfaces.Services;

namespace Winzcart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IBillService _billService;
    private readonly IWinnerService _winnerService;

    public AdminController(
        IAdminService adminService,
        IBillService billService,
        IWinnerService winnerService)
    {
        _adminService = adminService;
        _billService = billService;
        _winnerService = winnerService;
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        var response = await _adminService.GetAnalyticsAsync();
        return Ok(response);
    }

    [HttpGet("bills")]
    public async Task<IActionResult> GetAllBills([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var response = await _billService.GetAllBillsAsync(page, pageSize);
        return Ok(response);
    }

    [HttpPut("bills/{id}/approve")]
    public async Task<IActionResult> ApproveOrRejectBill(Guid id, [FromBody] ApproveBillRequest request)
    {
        var response = await _billService.ApproveOrRejectBillAsync(id, request);
        return Ok(response);
    }

    [HttpGet("winners")]
    public async Task<IActionResult> GetWinners([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _winnerService.GetWinnersAsync(page, pageSize);
        return Ok(response);
    }

    [HttpPost("winners/select")]
    public async Task<IActionResult> SelectWinner([FromBody] SelectWinnerRequest request)
    {
        var response = await _winnerService.SelectWinnerAsync(request);
        return Ok(response);
    }
}
