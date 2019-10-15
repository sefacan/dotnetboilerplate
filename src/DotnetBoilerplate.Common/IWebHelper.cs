using Microsoft.AspNetCore.Http;

namespace DotnetBoilerplate.Common
{
    public interface IWebHelper
    {
        bool IsSecureConnection { get; }
        string UrlReferrer { get; }
        string CurrentIpAddress { get; }
        string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false);
        string GetHost(bool useSsl);
        string GetLocation(bool? useSsl = null);
        string GetRawUrl(HttpRequest request);
    }
}