namespace PharmaOS.API.DTOs;

public class CreateDailyClosingRequest
{
    public int PharmacyId { get; set; }

    public decimal ClosingCashAmount { get; set; }
}
