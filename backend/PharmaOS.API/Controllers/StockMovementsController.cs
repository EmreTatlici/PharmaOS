using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.DTOs;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockMovementsController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public StockMovementsController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockMovementResponse>>> GetStockMovements()
    {
        var movements = await _context.StockMovements
            .Include(x => x.Drug)
            .Include(x => x.Pharmacy)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new StockMovementResponse
            {
                Id = x.Id,

                PharmacyId = x.PharmacyId,
                PharmacyName = x.Pharmacy.Name,

                DrugId = x.DrugId,
                DrugName = x.Drug.Name,
                Barcode = x.Drug.Barcode,

                InventoryItemId = x.InventoryItemId,

                MovementType = x.MovementType,
                Quantity = x.Quantity,

                UnitCost = x.UnitCost,
                UnitSalePrice = x.UnitSalePrice,

                ReferenceType = x.ReferenceType,
                ReferenceId = x.ReferenceId,

                CreatedByUserId = x.CreatedByUserId,

                Note = x.Note,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(movements);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StockMovementResponse>> GetStockMovement(int id)
    {
        var movement = await _context.StockMovements
            .Include(x => x.Drug)
            .Include(x => x.Pharmacy)
            .Where(x => x.Id == id)
            .Select(x => new StockMovementResponse
            {
                Id = x.Id,

                PharmacyId = x.PharmacyId,
                PharmacyName = x.Pharmacy.Name,

                DrugId = x.DrugId,
                DrugName = x.Drug.Name,
                Barcode = x.Drug.Barcode,

                InventoryItemId = x.InventoryItemId,

                MovementType = x.MovementType,
                Quantity = x.Quantity,

                UnitCost = x.UnitCost,
                UnitSalePrice = x.UnitSalePrice,

                ReferenceType = x.ReferenceType,
                ReferenceId = x.ReferenceId,

                CreatedByUserId = x.CreatedByUserId,

                Note = x.Note,
                CreatedAt = x.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (movement == null)
        {
            return NotFound();
        }

        return Ok(movement);
    }
}
