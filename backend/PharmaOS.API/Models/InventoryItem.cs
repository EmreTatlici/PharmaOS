namespace PharmaOS.API.Models;

public class InventoryItem
{
    public int Id { get; set; }

    public int PharmacyId { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;

    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;

    public int Quantity { get; set; }

    public string BatchNumber { get; set; } = string.Empty;

    public DateOnly ExpirationDate { get; set; }

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
