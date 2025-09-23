using System.Text.Json;

namespace JwtDemo.Apps;

[App(icon: Icons.Key, title: "JWT Demo")]
public class JwtDemoApp : ViewBase
{
    // Sample secret key for demonstration (in real apps, use proper secrets management)
    private const string SecretKey = "MySecretKeyForJWTDemonstration123!";
    
    // JSON serializer options for pretty printing
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public override object? Build()
    {
        // State for user inputs
        var username = this.UseState<string>("");
        var email = this.UseState<string>("");
        var role = this.UseState<string>("user");
        var tokenToValidate = this.UseState<string>("");
        var result = this.UseState<string>("");

        // Generate JWT token
        void GenerateToken()
        {
            try
            {
                var token = JwtBuilder.Create()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(SecretKey)
                    .AddClaim("sub", username.Value) // Subject (username)
                    .AddClaim("email", email.Value)
                    .AddClaim("role", role.Value)
                    .AddClaim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds()) // Issued at
                    .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds()) // Expires in 1 hour
                    .AddClaim("iss", "JwtDemo") // Issuer
                    .AddClaim("aud", "JwtDemoUsers") // Audience
                    .Encode();

                tokenToValidate.Set(token);
                result.Set($"Token generated successfully!\n\nToken: {token}");
            }
            catch (Exception ex)
            {
                result.Set($"Error generating token: {ex.Message}");
            }
        }

        // Validate JWT token
        void ValidateToken()
        {
            try
            {
                if (string.IsNullOrEmpty(tokenToValidate.Value))
                {
                    result.Set("Please enter a token to validate.");
                    return;
                }

                var json = JwtBuilder.Create()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(SecretKey)
                    .MustVerifySignature()
                    .Decode(tokenToValidate.Value);
                
                var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                var formattedPayload = JsonSerializer.Serialize(payload, JsonOptions);
                result.Set($"Token is valid!\n\nDecoded payload:\n{formattedPayload}");
            }
            catch (Exception ex)
            {
                result.Set($"Token validation failed: {ex.Message}");
            }
        }

        // Decode without validation (to see token contents even if signature is invalid)
        void DecodeToken()
        {
            try
            {
                if (string.IsNullOrEmpty(tokenToValidate.Value))
                {
                    result.Set("Please enter a token to decode.");
                    return;
                }

                var json = JwtBuilder.Create()
                    .DoNotVerifySignature()
                    .Decode(tokenToValidate.Value);
                
                var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                var formattedPayload = JsonSerializer.Serialize(payload, JsonOptions);
                result.Set($"Token decoded (without verification):\n\n{formattedPayload}");
            }
            catch (Exception ex)
            {
                result.Set($"Token decoding failed: {ex.Message}");
            }
        }

        // Extract specific claims
        void ExtractClaims()
        {
            try
            {
                if (string.IsNullOrEmpty(tokenToValidate.Value))
                {
                    result.Set("Please enter a token to extract claims from.");
                    return;
                }

                var json = JwtBuilder.Create()
                    .DoNotVerifySignature()
                    .Decode(tokenToValidate.Value);
                
                var payload = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                var claims = new
                {
                    Subject = payload.ContainsKey("sub") ? payload["sub"].GetString() : "N/A",
                    Email = payload.ContainsKey("email") ? payload["email"].GetString() : "N/A",
                    Role = payload.ContainsKey("role") ? payload["role"].GetString() : "N/A",
                    IssuedAt = payload.ContainsKey("iat") ? DateTimeOffset.FromUnixTimeSeconds(payload["iat"].GetInt64()).ToString() : "N/A",
                    ExpiresAt = payload.ContainsKey("exp") ? DateTimeOffset.FromUnixTimeSeconds(payload["exp"].GetInt64()).ToString() : "N/A",
                    Issuer = payload.ContainsKey("iss") ? payload["iss"].GetString() : "N/A",
                    Audience = payload.ContainsKey("aud") ? payload["aud"].GetString() : "N/A"
                };

                var formattedClaims = JsonSerializer.Serialize(claims, JsonOptions);
                result.Set($"Extracted Claims:\n\n{formattedClaims}");
            }
            catch (Exception ex)
            {
                result.Set($"Claims extraction failed: {ex.Message}");
            }
        }

        // Clear all fields
        void ClearAll()
        {
            username.Set("");
            email.Set("");
            role.Set("user");
            tokenToValidate.Set("");
            result.Set("");
        }

        // Build the UI
        return Layout.Vertical().Gap(4).Padding(4)
            | Text.H1("JWT (JSON Web Token) Demo")
            | Text.Block("This demo showcases the JWT.NET library for creating, validating, and working with JSON Web Tokens.")
            
            | new Card(
                Layout.Vertical().Gap(3).Padding(3)
                | Text.H2("🔑 Generate JWT Token")
                | Text.Small("Fill in the user details below to generate a JWT token:")
                | Layout.Horizontal().Gap(2)
                    | username.ToInput(placeholder: "Username (e.g., john_doe)")
                    | email.ToInput(placeholder: "Email (e.g., john@example.com)")
                | Layout.Horizontal().Gap(2)
                    | role.ToSelectInput(new[]
                    {
                        new Option<string>("User", "user"),
                        new Option<string>("Admin", "admin"),
                        new Option<string>("Moderator", "moderator")
                    }, placeholder: "Select Role")
                    | new Button("Generate Token", GenerateToken)
                        .Disabled(string.IsNullOrWhiteSpace(username.Value) || string.IsNullOrWhiteSpace(email.Value))
            )

            | new Card(
                Layout.Vertical().Gap(3).Padding(3)
                | Text.H2("🔍 Token Operations")
                | Text.Small("Paste a JWT token below to validate, decode, or extract claims:")
                | tokenToValidate.ToInput(placeholder: "Paste JWT token here...")
                | new WrapLayout(new[]
                {
                    new Button("Validate Token", ValidateToken),
                    new Button("Decode Token", DecodeToken),
                    new Button("Extract Claims", ExtractClaims),
                    new Button("Clear All", ClearAll)
                }, gap: 2)
            )

            | new Card(
                Layout.Vertical().Gap(3).Padding(3)
                | Text.H2("📋 Results")
                | new Code(result.Value?.Length > 0 ? result.Value : "Results will appear here...", Languages.Json)
                    .ShowLineNumbers()
                    .ShowCopyButton()
            )

            | new Card(
                Layout.Vertical().Gap(2).Padding(3)
                | Text.H2("📚 About JWT")
                | Text.Block("JSON Web Tokens (JWT) are a compact, URL-safe means of representing claims to be transferred between two parties. JWTs can be signed using a secret or a public/private key pair.")
                | Text.Small("Key Features demonstrated:")
                | Layout.Vertical().Gap(1)
                    | Text.Small("• Token Generation with custom claims")
                    | Text.Small("• Token Validation with signature verification")
                    | Text.Small("• Token Decoding without verification")
                    | Text.Small("• Claims extraction for specific use cases")
                    | Text.Small("• HMAC SHA-256 algorithm for signing")
                | Text.Markdown("Learn more about JWT at [jwt.io](https://jwt.io) and the library at [GitHub](https://github.com/jwt-dotnet/jwt)")
            );
    }
}