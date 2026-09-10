namespace PharmaOS.API.DTOs;

public class SaleHistoryResponse
{
    public int MovementId { get; set; }

    public int DrugId { get; set; }

    public string DrugName { get; set; } = string.Empty;

    public string Barcode { get; set; } = string.Empty;

    public string BatchNumber { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitSalePrice { get; set; }

    public decimal TotalAmount { get; set; }
public string? PaymentType { get; set; }
    public DateTime SoldAt { get; set; }
}
