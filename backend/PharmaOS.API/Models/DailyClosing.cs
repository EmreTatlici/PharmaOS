namespace PharmaOS.API.Models;

public class DailyClosing
{
    public int Id { get; set; }

    public int PharmacyId { get; set; }

    public Pharmacy Pharmacy { get; set; } = null!;

    public DateOnly BusinessDate { get; set; }

    public int TotalSales { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal CashAmount { get; set; }

    public decimal CardAmount { get; set; }
    public decimal OpeningCashAmount { get; set; }

    public decimal ClosingCashAmount { get; set; }

    public decimal CashDifference { get; set; }
    public DateTime ClosedAt { get; set; }

    public string ClosedBy { get; set; } = "System";
}
