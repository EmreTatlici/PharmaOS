namespace PharmaOS.API.DTOs;

public class CreateSaleRequest
{
    public int PharmacyId { get; set; }

    public int InventoryItemId { get; set; }

    public int Quantity { get; set; }

    public int? PatientId { get; set; }
    public string? PaymentType { get; set; }
    public string? SaleType { get; set; }
}

