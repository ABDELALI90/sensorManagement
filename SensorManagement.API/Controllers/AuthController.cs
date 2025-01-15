using Microsoft.AspNetCore.Mvc;
using SensorManagement.Application.Services;

[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(JwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // In a real application, validate the username and password
        if (request.Username == "admin" && request.Password == "password") // Replace with real validation
        {
            var token = _jwtTokenService.GenerateToken(request.Username);
            return Ok(new { Token = token });
        }
        return Unauthorized();
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
