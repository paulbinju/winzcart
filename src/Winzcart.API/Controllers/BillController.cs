using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Winzcart.Application.DTOs.Bill;
using Winzcart.Application.Interfaces.Services;

namespace Winzcart.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillController : ControllerBase
{
    private readonly IBillService _billService;
    private readonly IImageStorageService _imageStorageService;

    public BillController(IBillService billService, IImageStorageService imageStorageService)
    {
        _billService = billService;
        _imageStorageService = imageStorageService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost("upload")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> UploadBill([FromForm] IFormFile file, [FromForm] string? merchantName, [FromForm] decimal? amount, [FromForm] DateTime? date)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        // Upload to mock storage
        using var stream = file.OpenReadStream();
        string imageUrl = await _imageStorageService.UploadAsync(stream, file.FileName, file.ContentType);

        var request = new UploadBillRequest
        {
            ImageUrl = imageUrl,
            MerchantName = merchantName,
            TotalAmount = amount,
            BillDate = date
        };

        var response = await _billService.UploadBillAsync(GetUserId(), request);
        return Ok(response);
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyBills([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await _billService.GetUserBillsAsync(GetUserId(), page, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Customer,Admin")]
    public async Task<IActionResult> GetBill(Guid id)
    {
        var response = await _billService.GetBillByIdAsync(id);
        
        // Authorization check: if customer, must be their bill
        if (User.IsInRole("Customer") && response.UserId != GetUserId())
        {
            return Forbid();
        }

        return Ok(response);
    }
}
