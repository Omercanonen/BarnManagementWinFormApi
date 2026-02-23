using Business.DTOs;

namespace Business.Abstract.ApiServices
{
    public interface IUserApiService
    {
        Task<UserMeResponseDto> MeAsync(CancellationToken ct = default);
    }
}
