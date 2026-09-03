using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PharmaOS.API.Data;
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
    public async Task<ActionResult<IEnumerable<PurchaseInvoice>>> GetPurchaseInvoices()
    {
        var invoices = await _context.PurchaseInvoices
            .Include(x => x.Supplier)
            .Include(x => x.Pharmacy)
            .Include(x => x.PurchaseInvoiceItems)
            .ThenInclude(x => x.Drug)
            .ToListAsync();

        return Ok(invoices);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PurchaseInvoice>> GetPurchaseInvoice(int id)
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

        return Ok(invoice);
    }
}
