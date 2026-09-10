namespace PharmaOS.API.DTOs;

public class PurchaseInvoiceResponse
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;

    public int PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty;

    public string InvoiceNumber { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;

    public List<PurchaseInvoiceItemResponse> Items { get; set; } = new();
}

public class PurchaseInvoiceItemResponse
{
    public int Id { get; set; }
    public int DrugId { get; set; }
    public string DrugName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly ExpirationDate { get; set; }
    public decimal TotalAmount { get; set; }
}
