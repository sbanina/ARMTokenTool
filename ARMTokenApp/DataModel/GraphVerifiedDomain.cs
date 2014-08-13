namespace ARMTokenApp.DataModel
{
    using Newtonsoft.Json;

    internal class GraphVerifiedDomain
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public bool Default { get; set; }
    }
}
