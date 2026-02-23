using Microsoft.AspNetCore.Identity;

namespace Entities.Concrete
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true;
        public Barn? Barn { get; set; }

    }
}
