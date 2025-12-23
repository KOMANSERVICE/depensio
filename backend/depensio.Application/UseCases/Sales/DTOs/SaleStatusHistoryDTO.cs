namespace depensio.Application.UseCases.Sales.DTOs;

/// <summary>
/// DTO for sale status history entry
/// </summary>
public record SaleStatusHistoryDTO
{
    /// <summary>
    /// The unique identifier of the history entry
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// The date and time when the status change occurred
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// The user who made the status change
    /// </summary>
    public string ChangedBy { get; init; } = string.Empty;

    /// <summary>
    /// The previous status (null if initial creation)
    /// </summary>
    public string? FromStatus { get; init; }

    /// <summary>
    /// The new status after the change
    /// </summary>
    public string ToStatus { get; init; } = string.Empty;

    /// <summary>
    /// Optional comment about the status change
    /// </summary>
    public string? Comment { get; init; }
}
