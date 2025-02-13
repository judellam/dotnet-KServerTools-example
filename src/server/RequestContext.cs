using KServerTools.Common;

namespace server;

/// <summary>
/// Represents a request context.
/// </summary>
/// <remarks>
/// Usually gets put in middleware for setup then to be used throughout the request.
/// </remarks>
public class RequestContext : IRequestContext {
    public Guid RequestId { get; set; }
    public string? UserAgent { get; set; }

    public void Setup(HttpContext context)
    {
        if (context == null || context.Request == null) {
            return;
        }
        RequestId = Guid.NewGuid();
        context.Request.Headers.TryGetValue("User-Agent", out var userAgent);
        UserAgent = userAgent;
    }
}