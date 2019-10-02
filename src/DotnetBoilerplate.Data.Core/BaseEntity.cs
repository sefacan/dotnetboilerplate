namespace DotnetBoilerplate.Data.Core
{
    public abstract class BaseEntity : BaseEntity<int>
    {
    }

    public abstract class BaseEntity<TId>
    {
        public virtual TId Id { get; set; }
    }
}