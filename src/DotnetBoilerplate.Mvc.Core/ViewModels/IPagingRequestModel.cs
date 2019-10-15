namespace DotnetBoilerplate.Mvc.Core.ViewModels
{
    public interface IPagingRequestModel
    {
        /// <summary>
        /// Gets a page number
        /// </summary>
        int Page { get; }

        /// <summary>
        /// Gets a page size
        /// </summary>
        int PageSize { get; }
    }
}