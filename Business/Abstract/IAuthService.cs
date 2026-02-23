using Business.DTOs.Auth;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    }
}
