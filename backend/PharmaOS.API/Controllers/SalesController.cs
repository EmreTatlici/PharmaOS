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
            SoldAt = x.CreatedAt,

            IsUndone = _context.StockMovements.Any(undo =>
                undo.MovementType == "SaleUndo" &&
                undo.ReferenceType == "SaleUndo" &&
                undo.ReferenceId == x.Id)
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
        (x.MovementType == "Sale" ||
         x.MovementType == "SaleUndo") &&
        x.CreatedAt >= startOfDayUtc &&
        x.CreatedAt < endOfDayUtc)
    .ToListAsync();
        var response = new DailySalesResponse
        {
            Date = today,

TotalSales =
    sales
        .Where(x => x.MovementType == "Sale")
        .Sum(x => x.Quantity)
    -
    sales
        .Where(x => x.MovementType == "SaleUndo")
        .Sum(x => x.Quantity),

PrescriptionSales =
    sales
        .Where(x =>
            x.MovementType == "Sale" &&
            x.SaleType == "Prescription")
        .Sum(x => x.Quantity)
    -
    sales
        .Where(x =>
            x.MovementType == "SaleUndo" &&
            x.SaleType == "Prescription")
        .Sum(x => x.Quantity),

RetailSales =
    sales
        .Where(x =>
            x.MovementType == "Sale" &&
            x.SaleType == "Retail")
        .Sum(x => x.Quantity)
    -
    sales
        .Where(x =>
            x.MovementType == "SaleUndo" &&
            x.SaleType == "Retail")
        .Sum(x => x.Quantity),
TotalAmount =
    sales.Sum(x =>
        x.MovementType == "Sale"
            ? x.Quantity * (x.UnitSalePrice ?? 0)
            : -x.Quantity * (x.UnitSalePrice ?? 0)),

CashAmount =
    sales
        .Where(x => x.PaymentType == "Cash")
        .Sum(x =>
            x.MovementType == "Sale"
                ? x.Quantity * (x.UnitSalePrice ?? 0)
                : -x.Quantity * (x.UnitSalePrice ?? 0)),

CardAmount =
    sales
        .Where(x => x.PaymentType == "Card")
        .Sum(x =>
            x.MovementType == "Sale"
                ? x.Quantity * (x.UnitSalePrice ?? 0)
                : -x.Quantity * (x.UnitSalePrice ?? 0))
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
        var turkeyTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

        var turkeyNow =
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                turkeyTimeZone);

        var today = DateOnly.FromDateTime(turkeyNow);

        var todayClosingExists = await _context.DailyClosings
            .AnyAsync(x =>
                x.PharmacyId == request.PharmacyId &&
                x.BusinessDate == today);

        if (todayClosingExists)
        {
            return BadRequest(
                "Bugünün kasa kapanışı yapılmış. Yeni satış oluşturulamaz.");
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
[HttpPost("{movementId}/undo")]
public async Task<ActionResult> UndoSale(
    int movementId,
    [FromQuery] int pharmacyId)
{
    var originalSale = await _context.StockMovements
        .Include(x => x.InventoryItem)
        .Include(x => x.Drug)
        .FirstOrDefaultAsync(x =>
            x.Id == movementId &&
            x.PharmacyId == pharmacyId);

    if (originalSale == null)
    {
        return NotFound("Satış kaydı bulunamadı.");
    }
    var saleBusinessDate = DateOnly.FromDateTime(
    TimeZoneInfo.ConvertTimeFromUtc(
        originalSale.CreatedAt,
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul")));

var saleDayClosed = await _context.DailyClosings
    .AnyAsync(x =>
        x.PharmacyId == pharmacyId &&
        x.BusinessDate == saleBusinessDate);

if (saleDayClosed)
{
    return BadRequest(
        "Bu satışın ait olduğu günün kasası kapanmış. Satış geri alınamaz.");
}

    if (originalSale.MovementType != "Sale")
    {
        return BadRequest("Bu hareket bir satış kaydı değil.");
    }

    var alreadyUndone = await _context.StockMovements
        .AnyAsync(x =>
            x.MovementType == "SaleUndo" &&
            x.ReferenceType == "SaleUndo" &&
            x.ReferenceId == originalSale.Id);

    if (alreadyUndone)
    {
        return BadRequest("Bu satış daha önce geri alınmış.");
    }

    if (originalSale.InventoryItem == null)
    {
        return BadRequest("Satışın bağlı olduğu stok lotu bulunamadı.");
    }

    await using var transaction =
        await _context.Database.BeginTransactionAsync();

    originalSale.InventoryItem.Quantity += originalSale.Quantity;

    var undoMovement = new StockMovement
    {
        PharmacyId = originalSale.PharmacyId,
        DrugId = originalSale.DrugId,
        InventoryItemId = originalSale.InventoryItemId,
        MovementType = "SaleUndo",
        Quantity = originalSale.Quantity,
        UnitCost = originalSale.UnitCost,
        UnitSalePrice = originalSale.UnitSalePrice,
        PaymentType = originalSale.PaymentType,
        SaleType = originalSale.SaleType,
        ReferenceType = "SaleUndo",
        ReferenceId = originalSale.Id,
        Note = $"Satış geri alındı - Orijinal satış: #{originalSale.Id}"
    };

    _context.StockMovements.Add(undoMovement);

    await _context.SaveChangesAsync();

    await transaction.CommitAsync();

    return Ok(new
    {
        message = "Satış geri alındı.",
        originalMovementId = originalSale.Id,
        restoredQuantity = originalSale.Quantity,
        restoredStock = originalSale.InventoryItem.Quantity,
        reversedAmount =
            originalSale.Quantity * (originalSale.UnitSalePrice ?? 0),
        paymentType = originalSale.PaymentType,
        saleType = originalSale.SaleType,
        drug = originalSale.Drug.Name,
        batchNumber = originalSale.InventoryItem.BatchNumber
    });
}
}
