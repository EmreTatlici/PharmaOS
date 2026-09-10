using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.Models;
using PharmaOS.API.DTOs;

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
    public async Task<ActionResult<IEnumerable<InventoryItemResponse>>> GetInventoryItems()
    {
        var items = await _context.InventoryItems
            .Include(x => x.Drug)
            .Include(x => x.Pharmacy)
            .OrderBy(x => x.Drug!.Name)
            .Select(x => new InventoryItemResponse
            {
                Id = x.Id,
                PharmacyId = x.PharmacyId,
                PharmacyName = x.Pharmacy!.Name,
                DrugId = x.DrugId,
                DrugName = x.Drug!.Name,
                Barcode = x.Drug.Barcode,
                Quantity = x.Quantity,
                BatchNumber = x.BatchNumber,
                ExpirationDate = x.ExpirationDate,
                PurchasePrice = x.PurchasePrice,
                SalePrice = x.SalePrice,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(items);
    }
    [HttpGet("fefo-suggestion")]
    public async Task<ActionResult<FEFOSuggestionResponse>> GetFefoSuggestion(
        [FromQuery] int pharmacyId,
        [FromQuery] int drugId)
    {
        var lots = await _context.InventoryItems
            .Include(x => x.Drug)
            .Where(x =>
                x.PharmacyId == pharmacyId &&
                x.DrugId == drugId &&
                x.Quantity > 0)
            .OrderBy(x => x.ExpirationDate)
            .ThenBy(x => x.CreatedAt)
            .ToListAsync();

        if (lots.Count == 0)
        {
            return NotFound("Bu ilaç için satılabilir stok bulunamadı.");
        }

        var recommended = lots.First();

        return Ok(new FEFOSuggestionResponse
        {
            DrugId = recommended.DrugId,
            DrugName = recommended.Drug!.Name,
            PharmacyId = recommended.PharmacyId,
            RecommendedBatchNumber = recommended.BatchNumber,
            RecommendedExpirationDate = recommended.ExpirationDate,
            AvailableQuantity = recommended.Quantity,
            HasAlternativeLots = lots.Count > 1
        });
    }
[HttpGet("available-lots")]
public async Task<ActionResult<IEnumerable<AvailableLotResponse>>> GetAvailableLots(
    [FromQuery] int pharmacyId,
    [FromQuery] int drugId)
{
    var inventoryLots = await _context.InventoryItems
        .Where(x =>
            x.PharmacyId == pharmacyId &&
            x.DrugId == drugId &&
            x.Quantity > 0)
        .OrderBy(x => x.ExpirationDate)
        .ThenBy(x => x.CreatedAt)
        .ToListAsync();

    if (inventoryLots.Count == 0)
    {
        return NotFound("Bu ilaç için satılabilir lot bulunamadı.");
    }

    var lots = inventoryLots
        .Select((x, index) => new AvailableLotResponse
        {
            InventoryItemId = x.Id,
            BatchNumber = x.BatchNumber,
            ExpirationDate = x.ExpirationDate,
            Quantity = x.Quantity,
            SalePrice = x.SalePrice,
            IsRecommended = index == 0
        })
        .ToList();

    return Ok(lots);
}
    
    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItemResponse>> GetInventoryItem(int id)
    {
        var item = await _context.InventoryItems
            .Include(x => x.Drug)
            .Include(x => x.Pharmacy)
            .Where(x => x.Id == id)
            .Select(x => new InventoryItemResponse
            {
                Id = x.Id,
                PharmacyId = x.PharmacyId,
                PharmacyName = x.Pharmacy!.Name,
                DrugId = x.DrugId,
                DrugName = x.Drug!.Name,
                Barcode = x.Drug.Barcode,
                Quantity = x.Quantity,
                BatchNumber = x.BatchNumber,
                ExpirationDate = x.ExpirationDate,
                PurchasePrice = x.PurchasePrice,
                SalePrice = x.SalePrice,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (item == null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItemResponse>> CreateInventoryItem(InventoryItem inventoryItem)
    {
        if (!await _context.Drugs.AnyAsync(x => x.Id == inventoryItem.DrugId))
        {
            return BadRequest("Belirtilen ilaç bulunamadı.");
        }

        if (!await _context.Pharmacies.AnyAsync(x => x.Id == inventoryItem.PharmacyId))
        {
            return BadRequest("Belirtilen eczane bulunamadı.");
        }

        inventoryItem.Drug = null!;
        inventoryItem.Pharmacy = null!;

        _context.InventoryItems.Add(inventoryItem);
        await _context.SaveChangesAsync();

        var createdItem = await _context.InventoryItems
            .Include(x => x.Drug)
            .Include(x => x.Pharmacy)
            .FirstAsync(x => x.Id == inventoryItem.Id);

        var response = new InventoryItemResponse
        {
            Id = createdItem.Id,
            PharmacyId = createdItem.PharmacyId,
            PharmacyName = createdItem.Pharmacy!.Name,
            DrugId = createdItem.DrugId,
            DrugName = createdItem.Drug!.Name,
            Barcode = createdItem.Drug.Barcode,
            Quantity = createdItem.Quantity,
            BatchNumber = createdItem.BatchNumber,
            ExpirationDate = createdItem.ExpirationDate,
            PurchasePrice = createdItem.PurchasePrice,
            SalePrice = createdItem.SalePrice,
            CreatedAt = createdItem.CreatedAt
        };

        return CreatedAtAction(nameof(GetInventoryItem), new { id = createdItem.Id }, response);
    }
}
