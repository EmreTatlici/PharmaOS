namespace PharmaOS.API.Models;

public class Pharmacy
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
