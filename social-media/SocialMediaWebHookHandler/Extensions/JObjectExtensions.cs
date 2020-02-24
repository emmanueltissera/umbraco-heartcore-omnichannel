using System;
using Newtonsoft.Json.Linq;

namespace SocialMediaWebHookHandler.Extensions
{
    public static class JObjectExtensions
    {
        public static string GetStringValue(this JObject jObject, string field, string language = "$invariant")
        {
            return jObject.SelectToken(field).SelectToken(language).ToString();
        }

        public static decimal GetDecimalValue(this JObject jObject, string field, string language = "$invariant")
        {
            return decimal.Parse(jObject.GetStringValue(field, language));
        }

        public static int GetIntValue(this JObject jObject, string field, string language = "$invariant")
        {
            return int.Parse(jObject.GetStringValue(field, language));
        }

        public static bool GetBoolValue(this JObject jObject, string field, string language = "$invariant")
        {
            return Convert.ToBoolean( jObject.GetIntValue(field, language));
        }
    }
}
