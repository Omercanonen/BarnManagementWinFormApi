namespace Business.DTOs.Barn
{
    public class ActiveBarnListItemDto
    {
        public int Id { get; set; }
        public string BarnName { get; set; } = default!;
        public string Owner { get; set; } = default!;
        public string? Location { get; set; }
        public decimal Balance { get; set; }
        public string Capacity { get; set; } = default!;
    }
}
