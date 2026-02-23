namespace Business.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public string UserName { get; set; } = null!;
    }
}
