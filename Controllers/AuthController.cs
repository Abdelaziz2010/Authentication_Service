using Authentication_Service.Dtos;
using Authentication_Service.Models;
using Authentication_Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Authentication_Service.Controllers
{
    // Controllers/AuthController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly AuthService _authService;

        public AuthController(TokenService tokenService, AuthService authService)
        {
            _tokenService = tokenService;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserRegisterDto request)
        {
            var user = await _authService.Register(request);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var user = await _authService.Login(request);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            await _tokenService.SetRefreshToken(refreshToken, Response);

            return Ok(accessToken);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Invalid refresh token");

            var user = await _tokenService.GetUserFromRefreshToken(refreshToken);

            if (user == null)
                return Unauthorized("Invalid refresh token");

            // Revoke current refresh token
            await _tokenService.RevokeRefreshToken(refreshToken);

            // Generate new tokens
            var newAccessToken = _tokenService.CreateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

            await _tokenService.SetRefreshToken(newRefreshToken, Response);

            return Ok(newAccessToken);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _tokenService.RevokeAllRefreshTokens(userId);

            // Clear refresh token cookie
            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out successfully");
        }

        [Authorize]
        [HttpPost("revoke/{userId}")]
        public async Task<IActionResult> Revoke(int userId)
        {
            await _tokenService.RevokeAllRefreshTokens(userId);
            return Ok($"All tokens revoked for user {userId}");
        }
    }
}
