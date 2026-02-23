using Business.DTOs.Auth;

namespace Business.Abstract.ApiServices
{
    public interface IAuthApiService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
    }
}
