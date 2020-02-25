using System;
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
using Microsoft.Extensions.Options;

namespace SocialMediaWebHookHandler
{
    public class TwitterService
    {
        private readonly TwitterAppSettings _twitterAppSettings;

        public TwitterService(IOptions<TwitterAppSettings> twitterAppSettings)
        {
            _twitterAppSettings = twitterAppSettings.Value;
        }

        [FunctionName("TwitterService")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (DailyTweetSent())
            {
                return new OkObjectResult("Ignored");
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var publishData = JObject.Parse(requestBody);

            var product = ConvertJObjectToProduct(publishData);

            var success = SendTweet(product);

            return success
                ? (ActionResult)new OkObjectResult("Success")
                : new BadRequestObjectResult("Failure");
        }

        private bool DailyTweetSent()
        {
            // This function will check to see if there is a record for the current date in table storage
            return false;
        }

        private bool SendTweet(Product product)
        {
            var message = $"{product.Title} on special today.\n" +
                          $"{product.Quantity} baked fresh just now. Come and grab one!\n" +
                          "See lordlamington.com for more mouth-watering delicacies.";

            var twitterClient = new TwitterClient(_twitterAppSettings.ConsumerApiKey,
                _twitterAppSettings.ConsumerApiSecretKey,
                _twitterAppSettings.AccessToken, _twitterAppSettings.AccessTokenSecret);

            twitterClient.TweetText(message, string.Empty);

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
