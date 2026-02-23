using Business.DTOs;
using Business.DTOs.Barn;
using DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class BarnController : ControllerBase
{
    private readonly AppDbContext _db;

    public BarnController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("my")]
    public async Task<ActionResult<MyBarnResponseDto>> GetMyBarn(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();


        var barn = await _db.Barns
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.OwnerUser.Id == userId, ct);

        if (barn == null)
        {
            return Ok(new MyBarnResponseDto
            {
                HasBarn = false,
                BarnId = null,
                BarnName = null,
                BarnBalance = null
            });
        }

        return Ok(new MyBarnResponseDto
        {
            HasBarn = true,
            BarnId = barn.BarnId,
            BarnName = barn.BarnName,
            BarnBalance = barn.BarnBalance
        });
    }

    [HttpGet("all-active")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ActiveBarnListItemDto>>> GetAllActive(CancellationToken ct)
    {
        var barns = await _db.Barns
            .AsNoTracking()
            .Include(b => b.OwnerUser)
            .Where(b => b.IsActive)
            .Select(b => new ActiveBarnListItemDto
            {
                Id = b.BarnId,
                BarnName = b.BarnName,
                Owner = b.OwnerUser.UserName!,
                Location = b.BarnLocation,
                Balance = b.BarnBalance,
                Capacity = (b.BarnCapacity.ToString() + "/" + b.BarnMaxCapacity.ToString())
            })
            .ToListAsync(ct);

        return Ok(barns);
    }
}