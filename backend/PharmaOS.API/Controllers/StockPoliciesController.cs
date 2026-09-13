using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.DTOs;
using PharmaOS.API.Models;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockPoliciesController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public StockPoliciesController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetStockPolicies(
        [FromQuery] int pharmacyId)
    {
        var policies = await _context.StockPolicies
            .Include(x => x.Drug)
            .Where(x =>
                x.PharmacyId == pharmacyId &&
                x.IsActive)
            .Select(x => new
            {
                x.Id,
                x.PharmacyId,
                x.DrugId,
                DrugName = x.Drug.Name,
                Barcode = x.Drug.Barcode,
                x.CriticalStockLevel,
                x.TargetStockLevel,
                x.IsActive,

                CurrentStock = _context.InventoryItems
                    .Where(item =>
                        item.PharmacyId == pharmacyId &&
                        item.DrugId == x.DrugId)
                    .Sum(item => item.Quantity)
            })
            .OrderBy(x => x.DrugName)
            .ToListAsync();
        var result = policies.Select(x => new
{
    x.Id,
    x.PharmacyId,
    x.DrugId,
    x.DrugName,
    x.Barcode,
    x.CriticalStockLevel,
    x.TargetStockLevel,
    x.IsActive,
    x.CurrentStock,
    IsCritical = x.CurrentStock < x.CriticalStockLevel,
    SuggestedOrderQuantity = x.CurrentStock < x.TargetStockLevel
        ? x.TargetStockLevel - x.CurrentStock
        : 0
}).ToList();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateStockPolicy(
        [FromBody] CreateStockPolicyRequest request)
    {
        if (request.CriticalStockLevel < 0)
        {
            return BadRequest(
                "Kritik stok seviyesi negatif olamaz.");
        }

        if (request.TargetStockLevel < request.CriticalStockLevel)
        {
            return BadRequest(
                "Hedef stok seviyesi kritik stok seviyesinden düşük olamaz.");
        }

        var pharmacyExists = await _context.Pharmacies
            .AnyAsync(x =>
                x.Id == request.PharmacyId &&
                x.IsActive);

        if (!pharmacyExists)
        {
            return BadRequest(
                "Geçerli bir eczane bulunamadı.");
        }

        var drugExists = await _context.Drugs
            .AnyAsync(x =>
                x.Id == request.DrugId &&
                x.IsActive);

        if (!drugExists)
        {
            return BadRequest(
                "Geçerli bir ilaç bulunamadı.");
        }

        var existingPolicy = await _context.StockPolicies
            .FirstOrDefaultAsync(x =>
                x.PharmacyId == request.PharmacyId &&
                x.DrugId == request.DrugId);

        if (existingPolicy != null)
        {
            return BadRequest(
                "Bu ilaç için bu eczanede zaten bir stok politikası bulunuyor.");
        }

        var policy = new StockPolicy
        {
            PharmacyId = request.PharmacyId,
            DrugId = request.DrugId,
            CriticalStockLevel = request.CriticalStockLevel,
            TargetStockLevel = request.TargetStockLevel,
            IsActive = true
        };

        _context.StockPolicies.Add(policy);

        await _context.SaveChangesAsync();

        return Ok(policy);
    }
}
