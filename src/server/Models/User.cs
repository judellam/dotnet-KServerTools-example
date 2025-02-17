using System.Text.Json.Serialization;

namespace server.Models;

/// <summary>
/// External model for user login
/// </summary>
public record User(
    [property:JsonPropertyName("userName")] string Username, 
    [property:JsonPropertyName("password")] string Password) {
    [property:JsonPropertyName("email")] public string? Email { get; set; } = null;
}

/// <summary>
/// External model for user login response
/// </summary>
public record UserLoginResponse(
    [property:JsonPropertyName("jwt-token")] string Token) {
    [property:JsonPropertyName("success")] public bool Success => !string.IsNullOrEmpty(Token);
}

/// <summary>
/// External model for user registration
/// </summary>
public record RegisterUser(
    [property:JsonPropertyName("userName")] string Username, 
    [property:JsonPropertyName("password")] string Password,
    [property:JsonPropertyName("email")] string Email);