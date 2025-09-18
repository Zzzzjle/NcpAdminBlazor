using System.Security.Claims;
using System.Text.Json;

namespace NcpAdminBlazor.Client.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(this string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs == null) return claims;
            if (keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles))
            {
                var rolesString = roles.ToString();
                switch (string.IsNullOrEmpty(rolesString))
                {
                    case false when rolesString != null && rolesString.Trim().StartsWith('['):
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);
                        if (parsedRoles != null)
                        {
                            foreach (var parsedRole in parsedRoles)
                            {
                                if (!string.IsNullOrEmpty(parsedRole))
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                                }
                            }
                        }

                        break;
                    }
                    case false:
                        if (rolesString != null) claims.Add(new Claim(ClaimTypes.Role, rolesString));
                        break;
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
            return claims;
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
}
