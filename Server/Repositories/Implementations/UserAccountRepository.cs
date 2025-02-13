using BaseLibrary.DTOs;
using BaseLibrary.DTOs.BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Server.Repositories.Contracts;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Server.Repositories.Implementations
{
    public class UserAccountRepository : IUserAccount
    {
       
            private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOptions<JwtSection> config;
        private readonly AppDbContext appDbContext;

        public UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext)
        {
            this.config = config;
            this.appDbContext = appDbContext;
        }

        // ... existing constructor and other methods ...
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user is null) return new GeneralResponse(false, "Entity is empty");

            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null) return new GeneralResponse(false, "User Already registered");


            //Save user
            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                FullName = user.FullName,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
                //ConfirmPassword = user.ConfirmPassword

            });

            //Check, create and assign role
            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));
            if (checkAdminRole is null)
            {
                var createAdminRole = await AddToDatabase(new SystemRoles() { Name = Constants.Admin });
                await AddToDatabase(new UserRoles() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
                return new GeneralResponse(true, "AdminAccount created Successfully!");
            }
            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
            SystemRoles response = new();
            if (checkUserRole is null)
            {
                response = await AddToDatabase(new SystemRoles() { Name = Constants.User });
                await AddToDatabase(new UserRoles() { RoleId = response.Id, UserId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRoles() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
            }
            return new GeneralResponse(true, "UserAccount Successfully created");

        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            if (user is null) return new LoginResponse(false, "Model is empty");
            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null) return new LoginResponse(false, "User not found");

            //Verify the user password
            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
                return new LoginResponse(false, "Email/Password not valid");

            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null) return new LoginResponse(false, "user role not found");

            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getUserRole is null) return new LoginResponse(false, "user role not found");

            string jwtToken = GenerateToken(applicationUser, getRoleName!.Name!);
            string refreshToken = GenerateRefreshToken();

            //Save the Refresh token to the database
            var findUser = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                //checks if the user is logged in else refresh the token
                findUser!.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken, UserId = applicationUser.Id });
            }
            return new LoginResponse(true, "Login successfully", jwtToken, refreshToken);

        }
        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!)
            };
            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private async Task<UserRoles> FindUserRole(int userId) => await appDbContext.UserRoles.FirstOrDefaultAsync(_ => _.UserId == userId);
        private async Task<SystemRoles> FindRoleName(int roleId) => await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Id == roleId);
        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        private async Task<ApplicationUser> FindUserByEmail(string email) =>
            await appDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Email!.ToLower()!.Equals(email!.ToLower()));

        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }

        //public Task<RefreshTokenResponse> RefreshTokenAsync(RefreshToken token)
        //{
        //    throw new NotImplementedException();
        //}
        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null) return new LoginResponse(false, "Token is Empty");

            var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.Token!.Equals(token.Token));
            if (findToken is null) return new LoginResponse(false, "Refresh token is required");

            //get user details
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Id == findToken.UserId);
            if (user is null) return new LoginResponse(false, "Refresh token wasn't created due to user not found");

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.RoleId);
            string jwtToken = GenerateToken(user, roleName.Name!);
            string refreshToken = GenerateRefreshToken();

            //Sign-in before a Refresh token can be granted to the user
            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == user.Id);
            if (updateRefreshToken is null) return new LoginResponse(false, "Please sign in before a Refresh Token can be assigned to user");

            //stateless jwt
            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();
            return new LoginResponse(true, "Token successfully granted", jwtToken, refreshToken);
        }

        public Task<GeneralResponse> ForgotPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> ResetPasswordAsync(ResetPassword model)
        {
            throw new NotImplementedException();
        }

      

            public async Task<List<UserDTO>> GetAllUsersAsync()
            {
                try
                {
                    var users = await _userManager.Users.ToListAsync();
                    return users.Select(user => new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        
                    }).ToList();
                }
                catch (Exception)
                {
                    // Log the exception
                    return new List<UserDTO>();
                }
            }

            public async Task<UserDTO> GetUserByIdAsync(string userId)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                        return null;

                    return new UserDTO
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                       
                    };
                }
                catch (Exception)
                {
                    // Log the exception
                    return null;
                }
            }

        Task<List<UserDTO>> IUserAccount.GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        Task<UserDTO> IUserAccount.GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }

        //Task<GeneralResponse> IUserAccount.RefreshTokenAsync(RefreshToken token)
        //{
        //    throw new NotImplementedException();
        //}
    }

