using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
using PharmaOS.API.DTOs;
using PharmaOS.API.Models;

namespace PharmaOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseInvoicesController : ControllerBase
{
    private readonly PharmaOSDbContext _context;

    public PurchaseInvoicesController(PharmaOSDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PurchaseInvoiceResponse>>> GetPurchaseInvoices()
    {
        var invoices = await _context.PurchaseInvoices
            .Include(x => x.Supplier)
            .Include(x => x.Pharmacy)
            .Include(x => x.PurchaseInvoiceItems)
            .ThenInclude(x => x.Drug)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        var response = invoices.Select(MapToResponse).ToList();

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseInvoiceResponse>> GetPurchaseInvoice(int id)
    {
        var invoice = await _context.PurchaseInvoices
            .Include(x => x.Supplier)
            .Include(x => x.Pharmacy)
            .Include(x => x.PurchaseInvoiceItems)
            .ThenInclude(x => x.Drug)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (invoice == null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(invoice));
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseInvoiceResponse>> CreatePurchaseInvoice(
        CreatePurchaseInvoiceRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
        {
            return BadRequest("Alış faturasında en az bir ürün bulunmalıdır.");
        }

        if (request.Items.Any(x => x.Quantity <= 0))
        {
            return BadRequest("Ürün miktarı sıfırdan büyük olmalıdır.");
        }

        if (request.Items.Any(x => x.UnitPrice < 0))
        {
            return BadRequest("Birim fiyat negatif olamaz.");
        }

        var supplierExists = await _context.Suppliers
            .AnyAsync(x => x.Id == request.SupplierId);

        if (!supplierExists)
        {
            return BadRequest("Belirtilen tedarikçi bulunamadı.");
        }

        var pharmacyExists = await _context.Pharmacies
            .AnyAsync(x => x.Id == request.PharmacyId);

        if (!pharmacyExists)
        {
            return BadRequest("Belirtilen eczane bulunamadı.");
        }

        var drugIds = request.Items
            .Select(x => x.DrugId)
            .Distinct()
            .ToList();

        var existingDrugIds = await _context.Drugs
            .Where(x => drugIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync();

        var missingDrugIds = drugIds
            .Except(existingDrugIds)
            .ToList();

        if (missingDrugIds.Count > 0)
        {
            return BadRequest("Faturadaki ürünlerden bazıları bulunamadı.");
        }

        var invoice = new PurchaseInvoice
        {
            SupplierId = request.SupplierId,
            PharmacyId = request.PharmacyId,
            InvoiceNumber = request.InvoiceNumber,
            InvoiceDate = request.InvoiceDate,
            TotalAmount = request.Items.Sum(x => x.Quantity * x.UnitPrice),
            Status = "Draft"
        };

        foreach (var item in request.Items)
        {
            invoice.PurchaseInvoiceItems.Add(new PurchaseInvoiceItem
            {
                DrugId = item.DrugId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                BatchNumber = item.BatchNumber,
                ExpirationDate = item.ExpirationDate,
                TotalAmount = item.Quantity * item.UnitPrice
            });
        }

        _context.PurchaseInvoices.Add(invoice);

        await _context.SaveChangesAsync();

        var createdInvoice = await _context.PurchaseInvoices
            .Include(x => x.Supplier)
            .Include(x => x.Pharmacy)
            .Include(x => x.PurchaseInvoiceItems)
            .ThenInclude(x => x.Drug)
            .FirstAsync(x => x.Id == invoice.Id);

        var response = MapToResponse(createdInvoice);

        return CreatedAtAction(
            nameof(GetPurchaseInvoice),
            new { id = invoice.Id },
            response);
    }
        [HttpPost("{id}/receive")]
    public async Task<ActionResult<PurchaseInvoiceResponse>> ReceivePurchaseInvoice(int id)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var invoice = await _context.PurchaseInvoices
                .Include(x => x.PurchaseInvoiceItems)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                return NotFound("Alış faturası bulunamadı.");
            }

            if (invoice.Status != "Draft")
            {
                return BadRequest("Bu alış faturası daha önce stoğa işlenmiş veya işlem için uygun değil.");
            }

            foreach (var item in invoice.PurchaseInvoiceItems)
            {
                var inventoryItem = await _context.InventoryItems
                    .FirstOrDefaultAsync(x =>
                        x.PharmacyId == invoice.PharmacyId &&
                        x.DrugId == item.DrugId &&
                        x.BatchNumber == item.BatchNumber &&
                        x.ExpirationDate == item.ExpirationDate &&
                        x.PurchasePrice == item.UnitPrice);

                if (inventoryItem == null)
                {
                    inventoryItem = new InventoryItem
                    {
                        PharmacyId = invoice.PharmacyId,
                        DrugId = item.DrugId,
                        Quantity = item.Quantity,
                        BatchNumber = item.BatchNumber,
                        ExpirationDate = item.ExpirationDate,
                        PurchasePrice = item.UnitPrice,
                        SalePrice = await _context.InventoryItems
    .Where(x =>
        x.PharmacyId == invoice.PharmacyId &&
        x.DrugId == item.DrugId)
    .OrderByDescending(x => x.CreatedAt)
    .Select(x => x.SalePrice)
    .FirstOrDefaultAsync()
                    };

                    _context.InventoryItems.Add(inventoryItem);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    inventoryItem.Quantity += item.Quantity;
                }

                var stockMovement = new StockMovement
                {
                    PharmacyId = invoice.PharmacyId,
                    DrugId = item.DrugId,
                    InventoryItemId = inventoryItem.Id,
                    MovementType = "Purchase",
                    Quantity = item.Quantity,
                    UnitCost = item.UnitPrice,
                    ReferenceType = "PurchaseInvoice",
                    ReferenceId = invoice.Id,
                    Note = $"Alış faturası: {invoice.InvoiceNumber}"
                };

                _context.StockMovements.Add(stockMovement);
            }

            invoice.Status = "Completed";

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var completedInvoice = await _context.PurchaseInvoices
                .Include(x => x.Supplier)
                .Include(x => x.Pharmacy)
                .Include(x => x.PurchaseInvoiceItems)
                .ThenInclude(x => x.Drug)
                .FirstAsync(x => x.Id == invoice.Id);

            return Ok(MapToResponse(completedInvoice));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static PurchaseInvoiceResponse MapToResponse(PurchaseInvoice invoice)
    {
        return new PurchaseInvoiceResponse
        {
            Id = invoice.Id,
            SupplierId = invoice.SupplierId,
            SupplierName = invoice.Supplier?.Name ?? string.Empty,
            PharmacyId = invoice.PharmacyId,
            PharmacyName = invoice.Pharmacy?.Name ?? string.Empty,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status,

            Items = invoice.PurchaseInvoiceItems
                .Select(item => new PurchaseInvoiceItemResponse
                {
                    Id = item.Id,
                    DrugId = item.DrugId,
                    DrugName = item.Drug?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    BatchNumber = item.BatchNumber,
                    ExpirationDate = item.ExpirationDate,
                    TotalAmount = item.TotalAmount
                })
                .ToList()
        };
    }
}
