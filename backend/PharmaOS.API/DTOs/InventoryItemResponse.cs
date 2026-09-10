namespace PharmaOS.API.DTOs;

public class InventoryItemResponse
{
    public int Id { get; set; }

    public int PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty;

    public int DrugId { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string BatchNumber { get; set; } = string.Empty;
    public DateOnly ExpirationDate { get; set; }

    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }

    public DateTime CreatedAt { get; set; }
}
