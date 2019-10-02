namespace DotnetBoilerplate.Data.Core
{
    public abstract class BaseEntity : BaseEntityTypedId<int>
    {
    }

    public abstract class BaseEntityTypedId<TId>
    {
        public TId Id { get; set; }
    }
}