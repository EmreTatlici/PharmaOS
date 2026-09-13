using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.DTOs;
using PharmaOS.API.Models;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public SalesController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SaleHistoryResponse>>> GetSales()
    {
        var sales = await _context.StockMovements
            .Include(x => x.Drug)
            .Include(x => x.InventoryItem)
            .Where(x => x.MovementType == "Sale")
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new SaleHistoryResponse
            {
                MovementId = x.Id,
                DrugId = x.DrugId,
                DrugName = x.Drug.Name,
                Barcode = x.Drug.Barcode,
                BatchNumber = x.InventoryItem != null
                    ? x.InventoryItem.BatchNumber
                    : "",
                Quantity = x.Quantity,
                UnitSalePrice = x.UnitSalePrice ?? 0,
                TotalAmount = x.Quantity * (x.UnitSalePrice ?? 0),
                PaymentType = x.PaymentType,
                SaleType = x.SaleType,
                SoldAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(sales);
    }

    [HttpGet("daily")]
    public async Task<ActionResult<DailySalesResponse>> GetDailySales()
    {
        var turkeyTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

        var turkeyNow =
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                turkeyTimeZone);

        var today = DateOnly.FromDateTime(turkeyNow);

        var startOfDayTurkey =
            today.ToDateTime(TimeOnly.MinValue);

        var endOfDayTurkey =
            today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var startOfDayUtc =
            TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(
                    startOfDayTurkey,
                    DateTimeKind.Unspecified),
                turkeyTimeZone);

        var endOfDayUtc =
            TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(
                    endOfDayTurkey,
                    DateTimeKind.Unspecified),
                turkeyTimeZone);

        var sales = await _context.StockMovements
            .Where(x =>
                x.MovementType == "Sale" &&
                x.CreatedAt >= startOfDayUtc &&
                x.CreatedAt < endOfDayUtc)
            .ToListAsync();

        var response = new DailySalesResponse
        {
            Date = today,

            TotalSales = sales.Count,

            PrescriptionSales =
                sales.Count(x => x.SaleType == "Prescription"),

            RetailSales =
                sales.Count(x => x.SaleType == "Retail"),

            TotalAmount =
                sales.Sum(x =>
                    x.Quantity * (x.UnitSalePrice ?? 0)),

            CashAmount =
                sales
                    .Where(x => x.PaymentType == "Cash")
                    .Sum(x =>
                        x.Quantity * (x.UnitSalePrice ?? 0)),

            CardAmount =
                sales
                    .Where(x => x.PaymentType == "Card")
                    .Sum(x =>
                        x.Quantity * (x.UnitSalePrice ?? 0))
        };

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult> CreateSale(
        CreateSaleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PaymentType))
        {
            return BadRequest("Ödeme tipi belirtilmelidir.");
        }

        if (string.IsNullOrWhiteSpace(request.SaleType))
        {
            return BadRequest("Satış türü belirtilmelidir.");
        }

        if (request.SaleType != "Retail" &&
            request.SaleType != "Prescription")
        {
            return BadRequest("Geçersiz satış türü.");
        }



        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var inventoryItem = await _context.InventoryItems
            .Include(x => x.Drug)
            .FirstOrDefaultAsync(x =>
                x.Id == request.InventoryItemId &&
                x.PharmacyId == request.PharmacyId);

        if (inventoryItem == null)
        {
            return NotFound("Seçilen lot bulunamadı.");
        }

        if (request.Quantity <= 0)
        {
            return BadRequest(
                "Satış miktarı sıfırdan büyük olmalıdır.");
        }

        if (inventoryItem.Quantity < request.Quantity)
        {
            return BadRequest("Yetersiz stok.");
        }

        // FEFO check (only warning, never changes the selected lot)
        var recommendedLot = await _context.InventoryItems
            .Where(x =>
                x.PharmacyId == request.PharmacyId &&
                x.DrugId == inventoryItem.DrugId &&
                x.Quantity > 0)
            .OrderBy(x => x.ExpirationDate)
            .ThenBy(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        inventoryItem.Quantity -= request.Quantity;

        var movement = new StockMovement
        {
            PharmacyId = inventoryItem.PharmacyId,
            DrugId = inventoryItem.DrugId,
            InventoryItemId = inventoryItem.Id,
            MovementType = "Sale",
            Quantity = request.Quantity,
            UnitCost = inventoryItem.PurchasePrice,
            UnitSalePrice = inventoryItem.SalePrice,
            PaymentType = request.PaymentType,
            SaleType = request.SaleType,
            ReferenceType = "Sale",
            ReferenceId = inventoryItem.Id,
            Note = $"Satış - Lot: {inventoryItem.BatchNumber}"
        };

        _context.StockMovements.Add(movement);

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok(new
        {
            message = "Satış tamamlandı.",
            remainingStock = inventoryItem.Quantity,
            soldLot = inventoryItem.BatchNumber,
            drug = inventoryItem.Drug!.Name,

            fefoWarning =
                recommendedLot != null &&
                recommendedLot.Id != inventoryItem.Id
                    ? new
                    {
                        message =
                            "Rafta daha eski SKT'li lot bulunuyor.",
                        recommendedBatch =
                            recommendedLot.BatchNumber,
                        recommendedExpirationDate =
                            recommendedLot.ExpirationDate,
                        availableQuantity =
                            recommendedLot.Quantity
                    }
                    : null
        });
    }
}
