namespace PharmaOS.API.Models;

public class PurchaseInvoice
{
    public int Id { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;

    public int PharmacyId { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateOnly InvoiceDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Draft";
    
    public ICollection<PurchaseInvoiceItem> PurchaseInvoiceItems { get; set; } = new List<PurchaseInvoiceItem>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
