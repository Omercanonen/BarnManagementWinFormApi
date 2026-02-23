using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Concrete
{
    public class Barn
    {
        [Key]
        public int BarnId { get; set; }
        public string BarnName { get; set; } = null!;
        public string? BarnLocation { get; set; }
        public string? BarnPersonInCharge { get; set; }

        public int BarnMaxCapacity { get; set; }
        public int BarnCapacity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BarnBalance { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public string OwnerUserId { get; set; } = null!;
        [ForeignKey("OwnerUserId")]
        public ApplicationUser OwnerUser { get; set; } = null!;

        public ICollection<Animal> Animals { get; set; } = new List<Animal>();
        public ICollection<BarnInventory> Inventory { get; set; } = new List<BarnInventory>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();

    }
}
