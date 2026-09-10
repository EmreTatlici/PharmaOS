namespace PharmaOS.API.DTOs;

public class FEFOSuggestionResponse
{
    public int DrugId { get; set; }
    public string DrugName { get; set; } = string.Empty;

    public int PharmacyId { get; set; }

    public string RecommendedBatchNumber { get; set; } = string.Empty;
    public DateOnly RecommendedExpirationDate { get; set; }

    public int AvailableQuantity { get; set; }

    public bool HasAlternativeLots { get; set; }
}
