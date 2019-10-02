namespace DotnetBoilerplate.Mvc.Core.ViewModels
{
    public abstract class BaseViewModel
    {
    }

    public abstract class BaseEntityViewModel : BaseEntityViewModel<int>
    {
    }

    public abstract class BaseEntityViewModel<TId>
    {
        public virtual TId Id { get; set; }
    }
}