namespace PharmaOS.API.Models;

public class Drug
{
    public int Id { get; set; }

    public string Barcode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string ActiveIngredient { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string Form { get; set; } = string.Empty;

    public string PrescriptionType { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
