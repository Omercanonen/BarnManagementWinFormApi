using Business.Abstract;
using Business.Constants;
using Business.DTOs;
using DataAccess.Context; // AppDbContext için gerekli
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _productionService;
        private readonly AppDbContext _context;

        public ProductionController(IProductionService productionService, AppDbContext context)
        {
            _productionService = productionService;
            _context = context;
        }

        private int GetBarnId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return 0;

            var barn = _context.Barns
                             .AsNoTracking()
                             .FirstOrDefault(b => b.OwnerUser.Id == userId); 

            return barn?.BarnId ?? 0;
        }

        [HttpGet("potential")]
        public async Task<IActionResult> GetPotential()
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            var result = await _productionService.GetProductionPotentialAsync(barnId);
            return Ok(result);
        }

        [HttpGet("pending")]
        public IActionResult GetPendingProducts()
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            var result = _productionService.GetAccumulatedProducts(barnId);
            return Ok(result);
        }

        [HttpPost("collect")]
        public async Task<IActionResult> Collect([FromBody] List<AccumulatedProductDto> itemsToCollect)
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            await _productionService.CollectManualProductsAsync(barnId, itemsToCollect);
            return Ok(new { Message = Messages.Info.ProductionCollectCompleted });
        }
    }
}