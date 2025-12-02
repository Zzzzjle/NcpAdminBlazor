using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace NcpAdminBlazor.Client.Providers;

// PURPOSE: Bridges token service with Blazor authentication
public sealed class TokenAuthenticationStateProvider : AuthenticationStateProvider, IDisposable
{
    private readonly IAuthTokenService _tokenService;
    private readonly ILogger<TokenAuthenticationStateProvider> _logger;

    public TokenAuthenticationStateProvider(
        IAuthTokenService tokenService,
        ILogger<TokenAuthenticationStateProvider> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
        _tokenService.TokensCleared += OnTokensCleared;
    }

    // Cache authentication state to avoid repeated token parsing
    private AuthenticationState? _cachedAuthState;
    private DateTime _cacheExpiry = DateTime.MinValue;

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Return cached state if still valid
            if (_cachedAuthState != null && _cacheExpiry > DateTime.UtcNow)
            {
                return _cachedAuthState;
            }

            var snapshot = await _tokenService.GetSnapshotAsync();
            var token = snapshot.AccessToken;

            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogDebug("No token found, returning anonymous user");
                return CreateAnonymousState();
            }

            var claims = ParseClaimsFromJwt(token);

            // Validate token expiration from claims
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");
            if (expClaim != null)
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));

                // Proactive refresh if expiring soon
                if (expTime < DateTimeOffset.UtcNow.AddMinutes(5))
                {
                    _logger.LogDebug("Token expiring soon, attempting refresh");
                    _ = Task.Run(async () => await _tokenService.TryRefreshTokenAsync());
                }
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            // Cache for 1 minute to reduce parsing overhead
            _cachedAuthState = new AuthenticationState(user);
            _cacheExpiry = DateTime.UtcNow.AddMinutes(1);

            _logger.LogDebug("User authenticated: {UserName}",
                user.Identity?.Name ?? "Unknown");

            return _cachedAuthState;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authentication state");
            return CreateAnonymousState();
        }
    }

    public async Task LoginAsync(TokenStorageSnapshot snapshot)
    {
        try
        {
            await _tokenService.SetTokensAsync(snapshot);

            // Clear cache to force re-evaluation
            _cachedAuthState = null;
            _cacheExpiry = DateTime.MinValue;

            // WHY: Notify all components that authentication state changed
            // HOW: Triggers re-rendering of AuthorizeView and authentication checks
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            _logger.LogInformation("User logged in successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            throw new InvalidOperationException("登录流程失败，请稍后重试。", ex);
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _tokenService.ClearTokensAsync();

            _logger.LogInformation("User logged out successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            throw new InvalidOperationException("注销流程失败，请稍后重试。", ex);
        }
    }

    private void OnTokensCleared(object? sender, EventArgs e)
    {
        _cachedAuthState = null;
        _cacheExpiry = DateTime.MinValue;
        NotifyAuthenticationStateChanged(Task.FromResult(CreateAnonymousState()));
    }

    public void Dispose()
    {
        _tokenService.TokensCleared -= OnTokensCleared;
    }

    private static AuthenticationState CreateAnonymousState()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        return new AuthenticationState(anonymous);
    }

    private List<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null)
            {
                return [];
            }

            var claims = new List<Claim>();

            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement element)
                {
                    if (element.ValueKind == JsonValueKind.Array)
                    {
                        // Handle array claims (e.g., roles)
                        foreach (var item in element.EnumerateArray())
                        {
                            claims.Add(new Claim(kvp.Key, item.GetString() ?? ""));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(kvp.Key, element.ToString()));
                    }
                }
                else
                {
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
                }
            }

            // Map JWT standard claims to .NET claim types
            MapStandardClaims(claims);

            return claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing JWT claims");
            return [];
        }
    }

    private static void MapStandardClaims(List<Claim> claims)
    {
        // WHY: Map JWT standard claims to .NET Framework claim types
        // HOW: This enables standard .NET authorization to work with JWT claims
        var mappings = new Dictionary<string, string>
        {
            { "sub", ClaimTypes.NameIdentifier },
            { "name", ClaimTypes.Name },
            { "email", ClaimTypes.Email },
            { "role", ClaimTypes.Role },
            { "roles", ClaimTypes.Role }
        };

        foreach (var mapping in mappings)
        {
            var claim = claims.FirstOrDefault(c => c.Type == mapping.Key);
            if (claim != null)
            {
                claims.Add(new Claim(mapping.Value, claim.Value));
            }
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}