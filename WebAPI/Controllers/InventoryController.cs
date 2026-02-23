using Business.Abstract;
using Business.Constants;
using Business.DTOs;
using DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly AppDbContext _context;

        public InventoryController(IInventoryService inventoryService, AppDbContext context)
        {
            _inventoryService = inventoryService;
            _context = context;
        }

        private int GetBarnId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return 0;

            var barn = _context.Barns.AsNoTracking().FirstOrDefault(b => b.OwnerUser.Id == userId);
            return barn?.BarnId ?? 0;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetInventory()
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            var items = await _inventoryService.GetInventoryAsync(barnId);
            return Ok(items);
        }

        [HttpPost("sell")]
        public async Task<IActionResult> Sell([FromBody] SellRequestDto request)
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            request.BarnId = barnId;
            request.SoldByUserId = userId;

            bool result = await _inventoryService.SellAsync(request);

            if (!result) return BadRequest(Messages.Error.SellFailed);

            return Ok(new { Message = Messages.Info.SaleSuccess });
        }

        [HttpPost("sell-all")]
        public async Task<IActionResult> SellAll([FromBody] int productId)
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            bool result = await _inventoryService.SellAllAsync(barnId, productId, userId);

            if (!result) return BadRequest(Messages.Warning.NoStockToSell);

            return Ok(new { Message = Messages.Info.AllStockSold });
        }

        [HttpGet("export-history")]
        public async Task<IActionResult> ExportHistory()
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            var json = await _inventoryService.ExportSalesJsonAsync(barnId);

            return Ok(json);
        }
    }
}