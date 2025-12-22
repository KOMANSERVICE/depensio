namespace depensio.Domain.Enums;

public enum PurchaseStatus
{
    /// <summary>
    /// Brouillon
    /// </summary>
    Draft = 1,
    /// <summary>
    /// En attente de validation
    /// </summary>
    PendingApproval = 2,
    /// <summary>
    /// Approuve (DEFAULT - retrocompatibilite)
    /// </summary>
    Approved = 3,
    /// <summary>
    /// Rejete
    /// </summary>
    Rejected = 4,
    /// <summary>
    /// Annule
    /// </summary>
    Cancelled = 5
}
