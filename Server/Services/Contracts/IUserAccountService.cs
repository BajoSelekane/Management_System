
using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace Server.Services.Contracts
{
    public interface IUserAccountService
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshToken token);

    }
}
