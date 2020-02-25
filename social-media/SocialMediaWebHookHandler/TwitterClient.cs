using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SocialMediaWebHookHandler
{
    public class TwitterClient
    {
        private readonly string _consumerKey;
        private readonly string _accessToken;
        private readonly HMACSHA1 _sigHasher;
        private readonly DateTime _epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Twitter endpoint for sending tweets
        /// </summary>
        private readonly string _twitterTextApi;
        /// <summary>
        /// Twitter endpoint for uploading images
        /// </summary>
        private readonly string _twitterImageApi;
        /// <summary>
        /// Current tweet limit
        /// </summary>
        private readonly int _limit;

        public TwitterClient(string consumerKey, string consumerKeySecret, string accessToken, string accessTokenSecret, int limit = 280)
        {
            _twitterTextApi = "https://api.twitter.com/1.1/statuses/update.json";
            _twitterImageApi = "https://upload.twitter.com/1.1/media/upload.json";

            _consumerKey = consumerKey;
            _accessToken = accessToken;
            _limit = limit;

            _sigHasher = new HMACSHA1(
                new ASCIIEncoding().GetBytes($"{consumerKeySecret}&{accessTokenSecret}")
            );
        }

        /// <summary>
        /// Publish a post with image
        /// </summary>
        /// <returns>result</returns>
        /// <param name="post">post to publish</param>
        /// <param name="pathToImage">image to attach</param>
        public string PublishToTwitter(string post, string pathToImage)
        {
            try
            {
                // first, upload the image
                var rezImage = Task.Run(async () =>
                {
                    var response = await TweetImage(pathToImage);
                    return response;
                });
                var rezImageJson = JObject.Parse(rezImage.Result.Item2);

                if (rezImage.Result.Item1 != 200)
                {
                    try
                    {
                        return $"Error uploading image to Twitter. {rezImageJson["errors"][0]["message"].Value<string>()}";
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unknown error uploading image to Twitter", ex);
                    }
                }
                var mediaId = rezImageJson["media_id_string"].Value<string>();

                // second, send the text with the uploaded image
                var rezText = Task.Run(async () =>
                {
                    var response = await TweetText(CutTweetToLimit(post), mediaId);
                    return response;
                });
                var rezTextJson = JObject.Parse(rezText.Result.Item2);

                if (rezText.Result.Item1 != 200)
                {
                    try // return error from JSON
                    {
                        return $"Error sending post to Twitter. {rezTextJson["errors"][0]["message"].Value<string>()}";
                    }
                    catch (Exception) // return unknown error
                    {
                        // log exception somewhere
                        return "Unknown error sending post to Twitter";
                    }
                }

                return "OK";
            }
            catch (Exception)
            {
                // log exception somewhere
                return "Unknown error publishing to Twitter";
            }
        }

        /// <summary>
        /// Send a tweet with some image attached
        /// </summary>
        /// <returns>HTTP StatusCode and response</returns>
        /// <param name="text">Text</param>
        /// <param name="mediaId">Media ID for the uploaded image. Pass empty string, if you want to send just text</param>
        public Task<Tuple<int, string>> TweetText(string text, string mediaId)
        {
            var textData = new Dictionary<string, string> {
                { "status", text },
                { "trim_user", "1" },
                { "media_ids", mediaId}
            };

            return SendText(_twitterTextApi, textData);
        }

        /// <summary>
        /// Upload some image to Twitter
        /// </summary>
        /// <returns>HTTP StatusCode and response</returns>
        /// <param name="pathToImage">Path to the image to send</param>
        public Task<Tuple<int, string>> TweetImage(string pathToImage)
        {
            byte[] imageData = System.IO.File.ReadAllBytes(pathToImage);
            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");

            var multipartContent = new MultipartFormDataContent {{imageContent, "media"}};

            return SendImage(_twitterImageApi, multipartContent);
        }

        private async Task<Tuple<int, string>> SendText(string url, Dictionary<string, string> textData)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", PrepareOAuth(url, textData, "POST"));

            var httpResponse = await httpClient.PostAsync(url, new FormUrlEncodedContent(textData));
            var httpContent = await httpResponse.Content.ReadAsStringAsync();

            return new Tuple<int, string>((int)httpResponse.StatusCode, httpContent);
        }

        private async Task<Tuple<int, string>> SendImage(string url, MultipartFormDataContent multipartContent)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Authorization", PrepareOAuth(url, null, "POST"));

            var httpResponse = await httpClient.PostAsync(url, multipartContent);
            var httpContent = await httpResponse.Content.ReadAsStringAsync();

            return new Tuple<int, string>((int)httpResponse.StatusCode, httpContent);
        }

        #region Some OAuth magic

        private string PrepareOAuth(string url, Dictionary<string, string> data, string httpMethod)
        {
            // seconds passed since 1/1/1970
            var timestamp = (int)((DateTime.UtcNow - _epochUtc).TotalSeconds);

            // Add all the OAuth headers we'll need to use when constructing the hash
            Dictionary<string, string> oAuthData = new Dictionary<string, string>
            {
                {"oauth_consumer_key", _consumerKey},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", timestamp.ToString()},
                {"oauth_nonce", Guid.NewGuid().ToString()},
                {"oauth_token", _accessToken},
                {"oauth_version", "1.0"}
            };

            if (data != null) // add text data too, because it is a part of the signature
            {
                foreach (var item in data)
                {
                    oAuthData.Add(item.Key, item.Value);
                }
            }

            // Generate the OAuth signature and add it to our payload
            oAuthData.Add("oauth_signature", GenerateSignature(url, oAuthData, httpMethod));

            // Build the OAuth HTTP Header from the data
            return GenerateOAuthHeader(oAuthData);
        }

        /// <summary>
        /// Generate an OAuth signature from OAuth header values
        /// </summary>
        private string GenerateSignature(string url, Dictionary<string, string> data, string httpMethod)
        {
            var sigString = string.Join(
                "&",
                data
                    .Union(data)
                    .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}")
                    .OrderBy(s => s)
            );

            var fullSigData = $"{httpMethod}&{Uri.EscapeDataString(url)}&{Uri.EscapeDataString(sigString)}";

            return Convert.ToBase64String(_sigHasher.ComputeHash(new ASCIIEncoding().GetBytes(fullSigData)));
        }

        /// <summary>
        /// Generate the raw OAuth HTML header from the values (including signature)
        /// </summary>
        private string GenerateOAuthHeader(Dictionary<string, string> data)
        {
            return
                $"OAuth {string.Join(", ", data.Where(kvp => kvp.Key.StartsWith("oauth_")).Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}=\"{Uri.EscapeDataString(kvp.Value)}\"").OrderBy(s => s))}";
        }
        #endregion

        /// <summary>
        /// Cuts the tweet text to fit the limit
        /// </summary>
        /// <returns>Tweet text cut to specified limit</returns>
        /// <param name="tweet">Original tweet text</param>
        private string CutTweetToLimit(string tweet)
        {
            while (tweet.Length >= _limit)
            {
                tweet = tweet.Substring(0, tweet.LastIndexOf(" ", StringComparison.Ordinal));
            }
            return tweet;
        }
    }
}
