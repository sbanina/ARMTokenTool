namespace ARMTokenApp.DataModel
{
    using Newtonsoft.Json;

    internal class GraphTenantDetail
    {
        [JsonProperty]
        public string ObjectId { get; set; }

        [JsonProperty]
        public string DisplayName { get; set; }

        [JsonProperty]
        public GraphVerifiedDomain[] VerifiedDomains { get; set; }
    }
}
