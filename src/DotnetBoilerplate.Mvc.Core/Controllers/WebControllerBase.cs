using DotnetBoilerplate.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DotnetBoilerplate.Mvc.Core.Controllers
{
    public abstract class WebControllerBase : Controller
    {
        /// <summary>
        /// Save message into TempData
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="type">Notification type</param>
        /// <param name="message">Message</param>
        [NonAction]
        protected void PrepareTempData(string type, string message)
        {
            var httpContextAccessor = ServiceLocator.Context.GetService<IHttpContextAccessor>();
            var tempData = ServiceLocator.Context.GetService<ITempDataDictionaryFactory>().GetTempData(httpContextAccessor.HttpContext);
            string tempdataKey = $"Notifications.{type}";

            var messageList = tempData.ContainsKey(tempdataKey)
                ? JsonSerializer.Deserialize<List<string>>(tempData[tempdataKey].ToString())
                : new List<string>();

            messageList.Add(message);
            tempData[tempdataKey] = JsonSerializer.Serialize(messageList);
        }

        /// <summary>
        /// Display info notification
        /// </summary>
        /// <param name="message">Message</param>
        [NonAction]
        public void InfoNotification(string message)
        {
            PrepareTempData("Info", message);
        }

        /// <summary>
        /// Display success notification
        /// </summary>
        /// <param name="message">Message</param>
        [NonAction]
        public void SuccessNotification(string message)
        {
            PrepareTempData("Success", message);
        }

        /// <summary>
        /// Display warning notification
        /// </summary>
        /// <param name="message">Message</param>
        [NonAction]
        public void WarningNotification(string message)
        {
            PrepareTempData("Warning", message);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="message">Message</param>
        [NonAction]
        public void ErrorNotification(string message)
        {
            PrepareTempData("Error", message);
        }

        /// <summary>
        /// Display error notification
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        /// <param name="logException">A value indicating whether exception should be logged</param>
        [NonAction]
        public void ErrorNotification(Exception exception, bool logException = true)
        {
            if (logException)
            {
                var logger = ServiceLocator.Context.GetService<ILogger>();
                logger.LogError(exception, exception.Message);
            }

            PrepareTempData("Error", exception.Message);
        }

        [NonAction]
        public EmptyResult Empty()
        {
            return new EmptyResult();
        }

        [NonAction]
        public JsonResult EmptyJson()
        {
            return new JsonResult(null);
        }
    }
}