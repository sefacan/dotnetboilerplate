using DotnetBoilerplate.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Net;

namespace DotnetBoilerplate.Common
{
    public class WebHelper : IWebHelper, IScopedLifetime
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WebHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Check whether current HTTP request is available
        /// </summary>
        /// <returns>True if available; otherwise false</returns>
        protected bool IsRequestAvailable
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                    return false;

                if (_httpContextAccessor.HttpContext.Request == null)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>True if it's secured, otherwise false</returns>
        public bool IsSecureConnection
        {
            get
            {
                if (!IsRequestAvailable)
                    return false;

                return _httpContextAccessor.HttpContext.Request.IsHttps;
            }
        }

        /// <summary>
        /// Get URL referrer if exists
        /// </summary>
        /// <returns>URL referrer</returns>
        public string UrlReferrer
        {
            get
            {
                if (!IsRequestAvailable)
                    return string.Empty;

                //URL referrer is null in some case (for example, in IE 8)
                return _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
            }
        }

        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns>String of IP address</returns>
        public string CurrentIpAddress
        {
            get
            {
                if (!IsRequestAvailable)
                    return string.Empty;

                var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
                string result = string.Empty;

                //"TryParse" doesn't support IPv4 with port number
                if (IPAddress.TryParse(ipAddress.ToString(), out var ip))
                    //IP address is valid 
                    result = ip.ToString();
                else if (ipAddress != null)
                    //remove port
                    result = ipAddress.ToString().Split(':')[0];

                if (result == "::1" || string.IsNullOrEmpty(result))
                    result = "127.0.0.1";

                return result;
            }
        }

        /// <summary>
        /// Gets this page URL
        /// </summary>
        /// <param name="includeQueryString">Value indicating whether to include query strings</param>
        /// <param name="useSsl">Value indicating whether to get SSL secured page URL. Pass null to determine automatically</param>
        /// <param name="lowercaseUrl">Value indicating whether to lowercase URL</param>
        /// <returns>Page URL</returns>
        public string GetThisPageUrl(bool includeQueryString, bool? useSsl = null, bool lowercaseUrl = false)
        {
            if (!IsRequestAvailable)
                return string.Empty;

            //get location
            var location = GetLocation(useSsl ?? IsSecureConnection);

            //add local path to the URL
            var pageUrl = $"{location.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.Path}";

            //add query string to the URL
            if (includeQueryString)
                pageUrl = $"{pageUrl}{_httpContextAccessor.HttpContext.Request.QueryString}";

            //whether to convert the URL to lower case
            if (lowercaseUrl)
                pageUrl = pageUrl.ToLowerInvariant();

            return pageUrl;
        }

        /// <summary>
        /// Gets host location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL</param>
        /// <returns>host location</returns>
        public string GetHost(bool useSsl)
        {
            if (!IsRequestAvailable)
                return string.Empty;

            //try to get host from the request HOST header
            var hostHeader = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Host];
            if (StringValues.IsNullOrEmpty(hostHeader))
                return string.Empty;

            //add scheme to the URL
            var host = $"{(useSsl ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}{Uri.SchemeDelimiter}{hostHeader[0]}";

            //ensure that host is ended with slash
            host = $"{host.TrimEnd('/')}/";

            return host;
        }

        /// <summary>
        /// Gets location
        /// </summary>
        /// <param name="useSsl">Whether to get SSL secured URL; pass null to determine automatically</param>
        /// <returns>Location</returns>
        public string GetLocation(bool? useSsl = null)
        {
            var location = string.Empty;

            //get host
            var host = GetHost(useSsl ?? IsSecureConnection);
            if (!string.IsNullOrEmpty(host))
            {
                //add application path base if exists
                location = IsRequestAvailable ? $"{host.TrimEnd('/')}{_httpContextAccessor.HttpContext.Request.PathBase}" : host;
            }

            //ensure that URL is ended with slash
            location = $"{location.TrimEnd('/')}/";

            return location;
        }

        /// <summary>
        /// Get the raw path and full query of request
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Raw URL</returns>
        public string GetRawUrl(HttpRequest request)
        {
            //first try to get the raw target from request feature
            //note: value has not been UrlDecoded
            var rawUrl = request.HttpContext.Features.Get<IHttpRequestFeature>()?.RawTarget;

            //or compose raw URL manually
            if (string.IsNullOrEmpty(rawUrl))
                rawUrl = $"{request.PathBase}{request.Path}{request.QueryString}";

            return rawUrl;
        }
    }
}