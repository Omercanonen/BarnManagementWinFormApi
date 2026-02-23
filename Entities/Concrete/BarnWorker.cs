using System.ComponentModel.DataAnnotations;

namespace Entities.Concrete
{
    public class BarnWorker
    {
        [Key]
        public int BarnWorkerId { get; set; }

        public int BarnId { get; set; }
        public Barn Barn { get; set; }

        public int BarnWorkerCapacity { get; set; } = 5;
        public int BarnWorkerIntervalSecond {get; set; } = 60;
        public DateTime BarnWorkerNextCollectionTime { get; set; }
        public int BarnWorkerLevel { get; set; } = 1;
        public bool IsActive { get; set; } = true;

    }
}
