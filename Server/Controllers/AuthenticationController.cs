using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BaseLibrary.DTOs;
using Microsoft.Identity.Client;
using BaseLibrary.Entities;
using Server.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IUserAccount _accountInterface;

    public AuthenticationController(IUserAccount accountInterface)
    {
        _accountInterface = accountInterface ?? throw new ArgumentNullException(nameof(accountInterface));
    }

   

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] Register user)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Invalid registration data", Errors = ModelState });

        var result = await _accountInterface.CreateAsync(user);
        return Ok(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignInAsync([FromBody] Login user)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Invalid login credentials", Errors = ModelState });

        var result = await _accountInterface.SignInAsync(user);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountInterface.ForgotPasswordAsync(model.Email);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountInterface.ResetPasswordAsync(model);
        return Ok(result);
    }

    [HttpGet("getAllUsers")]
    [Authorize(Roles = "Admin")] // Optional: Add authorization if needed
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _accountInterface.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("getUserByUserId/{userId}")]
        [Authorize] // Optional: Add authorization if needed
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _accountInterface.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            return Ok(user);
        }



    [HttpPost("refresh-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshToken token)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { Message = "Invalid refresh token", Errors = ModelState });

        var result = await _accountInterface.RefreshTokenAsync(token);
        return Ok(result);
    }
}
}
// [Route("api/[controller]")]

    // [ApiController]
    // public class AuthenticationController(IUserAccount accountInterface) : ControllerBase
    // {


    //     [HttpPost("register")]
    //     public async Task<IActionResult> CreateAsync(Register user)
    //     {
    //         if (user == null) return BadRequest("Model is created");
    //         var result = await accountInterface.CreateAsync(user);
    //         return Ok(result);
    //     }
    //     [HttpPost("login")]
    //     public async Task<IActionResult> SignInAsync(Login user)
    //     {
    //         if (user == null) return BadRequest("Model is Empty");
    //         var result = await accountInterface.SignInAsync(user);
    //         return Ok(result);
    //     }
    //     [HttpPost("refresh-token")]
    //     public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
    //     {
    //         if (token == null) return BadRequest("Token is Empty");
    //         var result = await accountInterface.RefreshTokenAsync(token);
    //         return Ok(result);
    //     }
    // }
