namespace DotnetBoilerplate.Http
{
    public interface IRetryPolicyConfig
    {
        int RetryCount { get; set; }
    }
}