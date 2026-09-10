namespace PharmaOS.API.DTOs;

public class CreatePurchaseInvoiceRequest
{
    public int SupplierId { get; set; }

    public int PharmacyId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateOnly InvoiceDate { get; set; }

    public List<CreatePurchaseInvoiceItemRequest> Items { get; set; } = new();
}

public class CreatePurchaseInvoiceItemRequest
{
    public int DrugId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public string BatchNumber { get; set; } = string.Empty;

    public DateOnly ExpirationDate { get; set; }
}
