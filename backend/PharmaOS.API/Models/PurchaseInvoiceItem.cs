namespace PharmaOS.API.Models;

public class PurchaseInvoiceItem
{
    public int Id { get; set; }

    public int PurchaseInvoiceId { get; set; }
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string BatchNumber { get; set; } = string.Empty;

    public DateOnly ExpirationDate { get; set; }

    public decimal TotalAmount { get; set; }
}
