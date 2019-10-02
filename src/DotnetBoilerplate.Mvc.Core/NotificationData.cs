namespace DotnetBoilerplate.Mvc
{
    public class NotificationData
    {
        /// <summary>
        /// Message type (success/warning/error)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Message text
        /// </summary>
        public string Message { get; set; }
    }
}