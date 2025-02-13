using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary.DTOs;
using BaseLibrary.DTOs.BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace Server.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<GeneralResponse> ForgotPasswordAsync(string email);
        Task<GeneralResponse> ResetPasswordAsync(ResetPassword model);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(string userId);

    }
}
