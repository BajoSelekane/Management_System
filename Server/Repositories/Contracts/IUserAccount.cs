using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace Server.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
        Task<LoginResponse> SignInAsync(Login user);
    }
}
