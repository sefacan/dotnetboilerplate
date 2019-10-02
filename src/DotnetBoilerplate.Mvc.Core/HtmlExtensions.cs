using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;

namespace DotnetBoilerplate.Mvc
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// Generate antiforgery token for ajax modals
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string GenerateAntiforgeryToken(this IHtmlHelper html)
        {
            var antiforgery = (IAntiforgery)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IAntiforgery));
            var tokenSet = antiforgery.GetAndStoreTokens(html.ViewContext.HttpContext);

            return tokenSet.RequestToken;
        }

        /// <summary>
        /// Convert IHtmlContent to string
        /// </summary>
        /// <param name="htmlContent">HTML content</param>
        /// <returns>Result</returns>
        public static string RenderHtmlContent(this IHtmlContent htmlContent)
        {
            using (var writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, HtmlEncoder.Default);
                var htmlOutput = writer.ToString();

                return htmlOutput;
            }
        }

        /// <summary>
        /// Convert IHtmlContent to string
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <returns>String</returns>
        public static string ToHtmlString(this IHtmlContent tag)
        {
            using (var writer = new StringWriter())
            {
                tag.WriteTo(writer, HtmlEncoder.Default);

                return writer.ToString();
            }
        }
    }
}