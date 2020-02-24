using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SocialMediaWebHookHandler.Models;
using SocialMediaWebHookHandler.Extensions;
using System;
using CoreTweet;

namespace SocialMediaWebHookHandler
{
    public static class TwitterService
    {
        [FunctionName("TwitterService")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (DailyTweetSent())
            {
                return (ActionResult) new OkObjectResult("Ignored");
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var publishData = JObject.Parse(requestBody);

            var product = ConvertJObjectToProduct(publishData);

            var success = SendTweet(product);

            return success
                ? (ActionResult)new OkObjectResult("Success")
                : new BadRequestObjectResult("Failure");
        }

        private static bool DailyTweetSent()
        {
            // This function will check to see if there is a record for the current date in table storage
            return true;
        }

        private static bool SendTweet(Product product)
        {
            var session = OAuth.Authorize(Environment.GetEnvironmentVariable("TwitterApp:ConsumerKey"),
                Environment.GetEnvironmentVariable("TwitterApp:ConsumerSecret"));
            var tokens = session.GetTokens(Environment.GetEnvironmentVariable("TwitterApp:PinCode"));

            var message = $"{product.Title} on special today.\n"+
                            "{product.Quantity} baked fresh just now. Come and grab one!\n" +
                            "See lordlamington.com for more mouth-watering delicacies.";

            tokens.Statuses.Update(status => message);

            RecordDailyTweetSentAction();

            return true;
        }

        private static void RecordDailyTweetSentAction()
        {
            // This method will record the current date in table storage to indicate that the daily tweet has been sent
        }

        private static Product ConvertJObjectToProduct(JObject publishData)
        {
            return new Product()
            {
                Title = publishData.GetStringValue("title"),
                Description = publishData.GetStringValue("description"),
                ImageUrl = publishData.GetStringValue("image"),
                Price = publishData.GetDecimalValue("price"),
                Quantity = publishData.GetIntValue("quantity"),
                IsOnSpecialToday = publishData.GetBoolValue("isOnSpecialToday")
            };
        }
    }
}
