using Microsoft.AspNetCore.Identity;

namespace depensio.Domain.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ReferralCode { get; set; } = string.Empty;
    //public Guid? ReferredBy { get; set; }
    //public DateTime CreatedAt { get; set; }
    //public DateTime UpdatedAt { get; set; }

    //public ICollection<UsersBoutique> UsersBoutiques { get; set; }
    //public ICollection<Referral> Referrals { get; set; }
}
