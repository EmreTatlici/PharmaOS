using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.Models;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PharmaciesController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public PharmaciesController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pharmacy>>> GetPharmacies()
    {
        var pharmacies = await _context.Pharmacies
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(pharmacies);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Pharmacy>> GetPharmacy(int id)
    {
        var pharmacy = await _context.Pharmacies
            .FirstOrDefaultAsync(x => x.Id == id);

        if (pharmacy == null)
        {
            return NotFound();
        }

        return Ok(pharmacy);
    }

    [HttpPost]
    public async Task<ActionResult<Pharmacy>> CreatePharmacy(Pharmacy pharmacy)
    {
        _context.Pharmacies.Add(pharmacy);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPharmacy),
            new { id = pharmacy.Id },
            pharmacy);
    }
}
