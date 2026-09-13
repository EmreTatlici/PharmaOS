namespace PharmaOS.API.DTOs;

public class CreateStockPolicyRequest
{
    public int PharmacyId { get; set; }

    public int DrugId { get; set; }

    public int CriticalStockLevel { get; set; }

    public int TargetStockLevel { get; set; }
}
