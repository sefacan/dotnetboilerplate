using System.Collections.Generic;
using System.Linq;

namespace DotnetBoilerplate.Mvc.Core.ViewModels
{
    public abstract class BaseSearchViewModel : BaseViewModel, IPagingRequestModel
    {
        public BaseSearchViewModel()
        {
            //set the default values
            PageSize = 10;
            Sort = new List<SearchSortViewModel>();
        }

        /// <summary>
        /// Gets the sort fields
        /// </summary>
        public IList<SearchSortViewModel> Sort { get; set; }

        /// <summary>
        /// Gets the model property name sort by column
        /// </summary>
        public string SortFieldName => Sort.FirstOrDefault()?.Field;

        /// <summary>
        /// Gets the sort by ascending or descending
        /// </summary>
        public string SortBy => Sort.FirstOrDefault()?.Dir;

        /// <summary>
        /// Gets a page number
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets a page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a comma-separated list of available page sizes
        /// </summary>
        public string AvailablePageSizes { get; set; }

        /// <summary>
        /// Set grid page parameters
        /// </summary>
        public void SetGridPageSize()
        {
            SetGridPageSize(50, "50, 100, 250, 500, 1000");
        }

        /// <summary>
        /// Set grid page parameters
        /// </summary>
        /// <param name="pageSize">Page size; pass null to use default value</param>
        /// <param name="availablePageSizes">Available page sizes; pass null to use default value</param>
        public void SetGridPageSize(int pageSize, string availablePageSizes = null)
        {
            PageIndex = 1;
            PageSize = pageSize;
            AvailablePageSizes = availablePageSizes;
        }

        public class SearchSortViewModel
        {
            public string Field { get; set; }
            public string Dir { get; set; }
        }
    }
}