namespace PharmaOS.API.DTOs;

public class StockMovementResponse
{
    public int Id { get; set; }

    public int PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty;

    public int DrugId { get; set; }
    public string DrugName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;

    public int? InventoryItemId { get; set; }

    public string MovementType { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public decimal? UnitCost { get; set; }
    public decimal? UnitSalePrice { get; set; }

    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }

    public int? CreatedByUserId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
}
