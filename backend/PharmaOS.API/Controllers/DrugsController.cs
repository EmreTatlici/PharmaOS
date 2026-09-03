using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.Models;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DrugsController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public DrugsController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Drug>>> GetDrugs()
    {
        var drugs = await _context.Drugs
            .OrderBy(x => x.Name)
            .ToListAsync();

        return Ok(drugs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Drug>> GetDrug(int id)
    {
        var drug = await _context.Drugs
            .FirstOrDefaultAsync(x => x.Id == id);

        if (drug == null)
        {
            return NotFound();
        }

        return Ok(drug);
    }

    [HttpPost]
    public async Task<ActionResult<Drug>> CreateDrug(Drug drug)
    {
        _context.Drugs.Add(drug);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDrug), new { id = drug.Id }, drug);
    }
}
