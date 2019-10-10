using System.Collections.Generic;

namespace DotnetBoilerplate.Mvc.Core.ViewModels
{
    public abstract class BasePagedListViewModel<T> : BaseViewModel, IPagedModel<T> where T : BaseViewModel
    {
        /// <summary>
        /// Gets or sets data records
        /// </summary>
        public IEnumerable<T> Data { get; set; }

        /// <summary>
        /// Gets or sets total records number
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets an extra data
        /// </summary>
        public object ExtraData { get; set; }

        /// <summary>
        /// Gets or sets an errors
        /// </summary>
        public object Errors { get; set; }
    }
}