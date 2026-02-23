using AutoMapper;
using Business.Constants;
using Business.DTOs;
using Business.DTOs.Barn;
using DataAccess.Context;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AnimalController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public AnimalController(AppDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
    {
        _db = db;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpGet("species")]
    public async Task<ActionResult<List<AnimalSpeciesDto>>> GetSpecies(CancellationToken ct)
    {
        var species = await _db.AnimalSpecies
            .AsNoTracking()
            .Where(s => s.IsActive)
            .Select(s => new AnimalSpeciesDto
            {
                Id = s.AnimalSpeciesId,
                Name = s.AnimalSpeciesName,
                Price = s.AnimalSpeciesPurchasePrice
            })
            .ToListAsync(ct);

        return Ok(species);
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> PurchaseAnimal([FromBody] PurchaseAnimalDto dto, CancellationToken ct)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Unauthorized();

        var barn = await _db.Barns.FirstOrDefaultAsync(b => b.OwnerUserId == userId, ct);

        if (barn == null)
            return BadRequest(Messages.Error.BarnNotFound);


        var species = await _db.AnimalSpecies.FindAsync(new object[] { dto.AnimalSpeciesId }, ct);
        if (species == null || !species.IsActive)
            return BadRequest(Messages.Error.InvalidSpecies);

        if (barn.BarnCapacity >= barn.BarnMaxCapacity)
            return BadRequest(Messages.Error.BarnCapacityFull);

        if (barn.BarnBalance < species.AnimalSpeciesPurchasePrice)
            return BadRequest(Messages.Error.InsufficientBalance);

        try
        {
            var animal = _mapper.Map<Animal>(dto);

            animal.BarnId = barn.BarnId;
            animal.AnimalSpeciesId = species.AnimalSpeciesId;

            var purchaseLog = new Purchase
            {
                BarnId = barn.BarnId,
                AnimalSpeciesId = species.AnimalSpeciesId,
                Quantity = 1,
                UnitPrice = species.AnimalSpeciesPurchasePrice,
                TotalCost = species.AnimalSpeciesPurchasePrice,
                PurchasedByUserId = userId,
                PurchaseDate = DateTime.UtcNow
            };

            barn.BarnBalance -= species.AnimalSpeciesPurchasePrice;
            barn.BarnCapacity++;

            _db.Animals.Add(animal);
            _db.Purchases.Add(purchaseLog);
            _db.Barns.Update(barn);

            await _db.SaveChangesAsync(ct);

            return Ok(new { Message = Messages.Info.PurchaseSuccess, NewBalance = barn.BarnBalance });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"{Messages.Error.GeneralError} Details: {ex.Message}");
        }
    }



    [HttpGet("my")]
    public async Task<ActionResult<List<MyAnimalListItemDto>>> GetMyAnimals(CancellationToken ct)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var animals = await _db.Animals
            .AsNoTracking()
            .Include(a => a.AnimalSpecies)
            .Include(a => a.Barn)
            .Where(a => a.IsActive && a.Barn.OwnerUserId == user.Id)
            .Select(a => new
            {
                a.AnimalId,
                a.AnimalName,
                SpeciesName = a.AnimalSpecies.AnimalSpeciesName,
                a.AnimalGender,
                a.AgeMonth
            })
            .ToListAsync(ct);

        var result = animals.Select(a =>
        {
            var totalMonths = a.AgeMonth < 0 ? 0 : a.AgeMonth;
            var years = totalMonths / 12;
            var months = totalMonths % 12;

            string ageText =
                (years > 0 && months > 0) ? $"{years} Year {months} Month" :
                (years > 0) ? $"{years} Year" :
                $"{months} Month";

            return new MyAnimalListItemDto
            {
                Id = a.AnimalId,
                Name = a.AnimalName,
                Species = a.SpeciesName,
                Gender = a.AnimalGender,
                Age = ageText
            };
        }).ToList();

        return Ok(result);
    }
}