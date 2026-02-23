namespace Business.DTOs
{
    public class WorkerDto
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public int IntervalSeconds { get; set; }
        public int Level { get; set; }
        public decimal UpgradeCost { get; set; }
        public decimal SellPrice { get; set; }   
        public bool CanUpgrade { get; set; }
    }
}
