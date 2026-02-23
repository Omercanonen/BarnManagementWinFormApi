using Business.Security;

namespace Business.Services;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly ISessionContext _session;

    public AuthHeaderHandler(ISessionContext session)
    {
        _session = session;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_session.AccessToken))
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _session.AccessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}