namespace Business.Security;

public interface ISessionContext
{
    string? AccessToken { get; set; }
    DateTime? Expiration { get; set; }
    string? UserName { get; set; }
}

public sealed class SessionContext : ISessionContext
{
    public string? AccessToken { get; set; }
    public DateTime? Expiration { get; set; }
    public string? UserName { get; set; }
}