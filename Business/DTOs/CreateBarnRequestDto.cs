namespace Business.DTOs
{
    public sealed class CreateBarnRequestDto
    {
        public string BarnName { get; set; } = "";
        public int Capacity { get; set; }
    }
}
