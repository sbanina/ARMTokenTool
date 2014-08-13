namespace ARMTokenApp.DataModel
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class Tenant
    {
        [JsonProperty]
        public string TenantId { get; set; }

        [JsonIgnore]
        public List<string> TenantDomains { get; set; }

        [JsonIgnore]
        public string AuthenticationToken { get; set; }

        [JsonIgnore]
        public List<Subscription> Subscriptions { get; set; }

        public Tenant()
        {
            this.Subscriptions = new List<Subscription>();
            this.TenantDomains = new List<string>();
        }
    }
}
