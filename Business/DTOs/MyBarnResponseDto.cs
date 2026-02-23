namespace Business.DTOs
{
    public sealed class MyBarnResponseDto
    {
        public bool HasBarn { get; set; }

        public int? BarnId { get; set; }
        public string? BarnName { get; set; }
        public decimal? BarnBalance { get; set; }
    }
}
