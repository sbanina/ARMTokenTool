namespace ARMTokenApp.Extensions
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization.Formatters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    public static class JsonExtensions
    {
        public static T FromJson<T>(this string json)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter> 
                { 
                    new StringEnumConverter { CamelCaseText = false },
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal },
                },
            };

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public static string ToJson(this object objectToConvert)
        {
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
            };

            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(objectToConvert, Formatting.Indented, settings);
        }
    }
}
