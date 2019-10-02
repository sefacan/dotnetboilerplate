using FluentValidation;

namespace DotnetBoilerplate.Mvc.Core.Validations
{
    public abstract class BaseValidator<TModel> : AbstractValidator<TModel> where TModel : class
    {
    }
}