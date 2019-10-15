namespace DotnetBoilerplate.Mvc.Core.ViewModels
{
    public abstract class BaseViewModel
    {
    }

    public abstract class BaseEntityViewModel : BaseViewModel
    {
        public virtual int Id { get; set; }
    }
}