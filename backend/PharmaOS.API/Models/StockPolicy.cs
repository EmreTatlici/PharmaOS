namespace PharmaOS.API.Models;

public class StockPolicy
{
    public int Id { get; set; }

    public int PharmacyId { get; set; }
    public Pharmacy Pharmacy { get; set; } = null!;

    public int DrugId { get; set; }
    public Drug Drug { get; set; } = null!;

    public int CriticalStockLevel { get; set; }

    public int TargetStockLevel { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

