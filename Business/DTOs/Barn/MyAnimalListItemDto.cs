namespace Business.DTOs.Barn
{
    public class MyAnimalListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Species { get; set; } = default!;
        public string Gender { get; set; } = default!;
        public string Age { get; set; } = default!;
    }
}
