# Social Media Implementation

To implement the social media integration, Twitter in this case, you can spin up an Azure function which listens to a webhook call from Umbraco Heartcore.

1. First, [register your App on Twitter](https://developer.twitter.com/en/apps) 
2. Use the [code from this location](https://github.com/emmanueltissera/umbraco-heartcore-omnichannel/tree/master/social-media) and build an Azure function
3. Copy the keys and tokens from the Twitter App to the Azure configuration
4. You can use the [sample webhook call](https://github.com/emmanueltissera/umbraco-heartcore-omnichannel/blob/master/social-media/SocialMediaWebHookHandler/sample-webhook-call.json) to test out the Azure function locally
5. Once you are happy it's working deploy the function to Azure from Visual Studio
6. Setup a [webhook on Umbraco Heartcore](https://our.umbraco.com/documentation/Umbraco-Heartcore/Getting-Started-Cloud/Webhooks/) to call the endpoint on your Azure function