using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.Models;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DailyClosingsController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public DailyClosingsController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<DailyClosing>> CreateDailyClosing(
        [FromQuery] int pharmacyId)
    {
        var pharmacyExists = await _context.Pharmacies
            .AnyAsync(x => x.Id == pharmacyId && x.IsActive);

        if (!pharmacyExists)
        {
            return BadRequest("Geçerli bir eczane bulunamadı.");
        }

        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
        var turkeyNow = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            turkeyTimeZone);

        var today = DateOnly.FromDateTime(turkeyNow);

        var existingClosing = await _context.DailyClosings
            .FirstOrDefaultAsync(x =>
                x.PharmacyId == pharmacyId &&
                x.BusinessDate == today);

        if (existingClosing != null)
        {
            return BadRequest("Bu eczanenin bugünkü kasa kapanışı zaten yapılmış.");
        }

        var startOfDayTurkey = today.ToDateTime(TimeOnly.MinValue);
        var endOfDayTurkey = today.AddDays(1).ToDateTime(TimeOnly.MinValue);

        var startOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                startOfDayTurkey,
                DateTimeKind.Unspecified),
            turkeyTimeZone);

        var endOfDayUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                endOfDayTurkey,
                DateTimeKind.Unspecified),
            turkeyTimeZone);

        var sales = await _context.StockMovements
            .Where(x =>
                x.PharmacyId == pharmacyId &&
                x.CreatedAt >= startOfDayUtc &&
                x.CreatedAt < endOfDayUtc &&
                (x.MovementType == "Sale" ||
                 x.MovementType == "SaleReturn"))
            .ToListAsync();

        var totalSales = sales
            .Where(x => x.MovementType == "Sale")
            .Sum(x => x.Quantity);

        var totalAmount = sales.Sum(x =>
            x.MovementType == "Sale"
                ? x.Quantity * (x.UnitSalePrice ?? 0)
                : -x.Quantity * (x.UnitSalePrice ?? 0));

        var cashAmount = sales.Sum(x =>
            x.PaymentType == "Cash"
                ? x.MovementType == "Sale"
                    ? x.Quantity * (x.UnitSalePrice ?? 0)
                    : -x.Quantity * (x.UnitSalePrice ?? 0)
                : 0);

        var cardAmount = sales.Sum(x =>
            x.PaymentType == "Card"
                ? x.MovementType == "Sale"
                    ? x.Quantity * (x.UnitSalePrice ?? 0)
                    : -x.Quantity * (x.UnitSalePrice ?? 0)
                : 0);

        var closing = new DailyClosing
        {
            PharmacyId = pharmacyId,
            BusinessDate = today,
            TotalSales = totalSales,
            TotalAmount = totalAmount,
            CashAmount = cashAmount,
            CardAmount = cardAmount,
            ClosedAt = DateTime.UtcNow,
            ClosedBy = "System"
        };

        _context.DailyClosings.Add(closing);

        await _context.SaveChangesAsync();

        return Ok(closing);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DailyClosing>>> GetDailyClosings(
        [FromQuery] int pharmacyId)
    {
        var closings = await _context.DailyClosings
            .Where(x => x.PharmacyId == pharmacyId)
            .OrderByDescending(x => x.BusinessDate)
            .ToListAsync();

        return Ok(closings);
    }
}
