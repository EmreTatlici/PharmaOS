using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.Models;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryItemsController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public InventoryItemsController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetInventoryItems()
    {
        var items = await _context.InventoryItems
            .Include(x => x.Drug)
            .Include(x => x.Pharmacy)
            .OrderBy(x => x.Drug!.Name)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItem>> GetInventoryItem(int id)
    {
        var item = await _context.InventoryItems
            .Include(x => x.Drug)
            .Include(x => x.Pharmacy)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> CreateInventoryItem(
        InventoryItem inventoryItem)
    {
        var drugExists = await _context.Drugs
            .AnyAsync(x => x.Id == inventoryItem.DrugId);

        if (!drugExists)
        {
            return BadRequest("Belirtilen ilaç bulunamadı.");
        }

        var pharmacyExists = await _context.Pharmacies
            .AnyAsync(x => x.Id == inventoryItem.PharmacyId);

        if (!pharmacyExists)
        {
            return BadRequest("Belirtilen eczane bulunamadı.");
        }

        inventoryItem.Drug = null!;
        inventoryItem.Pharmacy = null!;

        _context.InventoryItems.Add(inventoryItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetInventoryItem),
            new { id = inventoryItem.Id },
            inventoryItem);
    }
}
