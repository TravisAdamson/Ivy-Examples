# JWT Demo

Interactive Ivy demo application showcasing the use of [JWT.NET](https://github.com/jwt-dotnet/jwt) library for JSON Web Token operations.

## Features

This demo showcases the core features of JWT.NET library:

- **Token Generation**: Create JWT tokens with custom claims including username, email, role, and standard claims (iat, exp, iss, aud)
- **Token Validation**: Verify JWT tokens with signature validation using HMAC SHA-256
- **Token Decoding**: Decode JWT tokens without verification to inspect contents
- **Claims Extraction**: Extract specific claims from JWT tokens for application use
- **Interactive UI**: User-friendly interface for learning JWT concepts

## Usage

1. **Generate a Token**:
   - Enter a username and email
   - Select a role (User/Admin/Moderator)
   - Click "Generate Token" to create a JWT with your claims

2. **Validate a Token**:
   - Paste a JWT token in the token operations section
   - Click "Validate Token" to verify the signature and decode the payload

3. **Decode a Token**:
   - Use "Decode Token" to see token contents without signature verification
   - Useful for inspecting expired or invalid tokens

4. **Extract Claims**:
   - Click "Extract Claims" to get formatted view of standard JWT claims
   - Shows subject, email, role, issued time, expiration time, issuer, and audience

## Built With

- [JWT.NET](https://github.com/jwt-dotnet/jwt) - JWT library for .NET
- [Ivy](https://github.com/Ivy-Interactive/Ivy) - Interactive web application framework

## JWT Security Notes

- This demo uses a hardcoded secret key for demonstration purposes
- In production applications, always use proper secrets management
- Store secrets securely (environment variables, Azure Key Vault, etc.)
- Use strong, randomly generated keys
- Consider using asymmetric algorithms (RS256) for distributed systems

## Run

```bash
dotnet watch
```

## Deploy

```bash
ivy deploy
```

## Learn More

- [JWT.io](https://jwt.io) - JWT introduction and debugger
- [RFC 7519](https://tools.ietf.org/html/rfc7519) - JWT specification
- [JWT.NET Documentation](https://github.com/jwt-dotnet/jwt) - Library documentation