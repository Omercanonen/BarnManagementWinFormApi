using Business.Abstract;
using Business.Constants;
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
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerService _workerService;
        private readonly AppDbContext _context;

        public WorkerController(IWorkerService workerService, AppDbContext context)
        {
            _workerService = workerService;
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
        public async Task<IActionResult> List()
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);
            return Ok(await _workerService.GetWorkersAsync(barnId));
        }

        [HttpPost("buy")]
        public async Task<IActionResult> Buy()
        {
            int barnId = GetBarnId();
            if (barnId == 0) return BadRequest(Messages.Error.BarnNotFound);

            try
            {
                await _workerService.BuyWorkerAsync(barnId, 0);
                return Ok(new { Message = Messages.Info.WorkerPurchased });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("upgrade/{id}")]
        public async Task<IActionResult> Upgrade(int id)
        {
            try
            {
                await _workerService.UpgradeWorkerAsync(id, 0);
                return Ok(new { Message = Messages.Info.WorkerUpgraded });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("sell/{id}")]
        public async Task<IActionResult> Sell(int id)
        {
            await _workerService.SellWorkerAsync(id, 0);
            return Ok(new { Message = Messages.Info.WorkerSold });
        }
    }
}