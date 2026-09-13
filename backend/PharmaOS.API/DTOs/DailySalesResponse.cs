namespace PharmaOS.API.DTOs;

public class DailySalesResponse
{
    public DateOnly Date { get; set; }

    public int TotalSales { get; set; }
    public int PrescriptionSales { get; set; }

    public int RetailSales { get; set; }
    public decimal TotalAmount { get; set; }

    public decimal CashAmount { get; set; }

    public decimal CardAmount { get; set; }
}

