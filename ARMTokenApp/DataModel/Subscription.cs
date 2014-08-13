namespace ARMTokenApp.DataModel
{
    using Newtonsoft.Json;

    internal class Subscription
    {
        [JsonProperty]
        public string SubscriptionId { get; set;}

        [JsonProperty]
        public string DisplayName { get; set; }

        [JsonProperty]
        public string State { get; set; }
    }
}
