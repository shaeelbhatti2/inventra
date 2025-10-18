using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventra.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Inventra.Application.Auth;

public sealed class AuthUser
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public UserRole Role { get; init; }
}

public sealed class LoginRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; init; }
    public AuthUser User { get; init; } = null!;
}

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken ct);
}

public sealed class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(AuthUser user, out DateTimeOffset expiresAt)
    {
        var secret = _configuration["JWT_SECRET"]
            ?? _configuration["Jwt:Secret"]
            ?? "dev-secret-key-minimum-32-characters-long";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        expiresAt = DateTimeOffset.UtcNow.AddHours(8);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("org", user.OrganizationId.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "inventra",
            audience: "inventra",
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public sealed class InMemoryAuthService : IAuthService
{
    private static readonly List<(AuthUser User, string Password)> Users = new()
    {
        (new AuthUser
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            OrganizationId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Email = "owner@demo.local",
            Role = UserRole.Owner
        }, "demo1234")
    };

    private readonly JwtTokenService _tokens;

    public InMemoryAuthService(JwtTokenService tokens) => _tokens = tokens;

    public Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var match = Users.FirstOrDefault(x =>
            x.User.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase) &&
            x.Password == request.Password);

        if (match.User is null)
        {
            return Task.FromResult<LoginResponse?>(null);
        }

        var token = _tokens.GenerateToken(match.User, out var expiresAt);
        return Task.FromResult<LoginResponse?>(new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = match.User
        });
    }

    public Task<AuthUser?> GetUserByEmailAsync(string email, CancellationToken ct)
    {
        var match = Users.FirstOrDefault(x => x.User.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(match.User);
    }
}
