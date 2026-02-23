using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
{
    public class BarnInventory
    {
        [Key]
        public int BarnInventoryId { get; set; }

        public int BarnId { get; set; }
        public Barn Barn { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
