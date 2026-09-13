namespace PharmaOS.API.Models;

public class StockMovement
{
    public int Id { get; set; }

    public int PharmacyId { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;

    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;

    public int? InventoryItemId { get; set; }
    public InventoryItem? InventoryItem { get; set; }

    public string MovementType { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal? UnitCost { get; set; }

    public decimal? UnitSalePrice { get; set; }
    
    public string? PaymentType { get; set; }
public string? SaleType { get; set; }    
    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    public int? CreatedByUserId { get; set; }
    public User? CreatedByUser { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
