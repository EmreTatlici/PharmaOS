namespace PharmaOS.API.DTOs;

public class AvailableLotResponse
{
    public int InventoryItemId { get; set; }

    public string BatchNumber { get; set; } = string.Empty;

    public DateOnly ExpirationDate { get; set; }

    public int Quantity { get; set; }

    public decimal SalePrice { get; set; }

    public bool IsRecommended { get; set; }
}
