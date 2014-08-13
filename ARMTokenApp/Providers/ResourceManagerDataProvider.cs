namespace ARMTokenApp.Providers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using ARMTokenApp.DataModel;
    using ARMTokenApp.Extensions;
    using ARMTokenApp.Interaction;

    internal class ResourceManagerDataProvider
    {
        private EnvironmentParameters Environment { get; set; }

        public ResourceManagerDataProvider(EnvironmentParameters environment)
        {
            this.Environment = environment;
        }

        public async Task<Tenant[]> GetTenantsWithSubscriptions()
        {
            var authenticationProvider = new AuthenticationProvider(environment: this.Environment);

            using (var tenantClient = new HttpClient())
            {
                var authResult = authenticationProvider.GetAuthorizationResult();
                if (authResult == null)
                {
                    // sometimes the prompt fails when switching between LiveId/OrgId subsequent runs. Single retry here to help.
                    authResult = authenticationProvider.GetAuthorizationResult();
                }

                tenantClient.DefaultRequestHeaders.Add("Authorization", authResult.CreateAuthorizationHeader());

                var tenants = await this.GetTenants(tenantClient);

                foreach (var tenant in tenants)
                {
                    var tenantAuthResult = authenticationProvider.GetAuthorizationResult(
                        tenantId: tenant.TenantId,
                        userId: authResult.UserInfo.UserId);

                    tenant.AuthenticationToken = tenantAuthResult.CreateAuthorizationHeader();

                    var tenantDomains = await authenticationProvider.GetDefaultTenantDomains(tenant.TenantId, tenant.AuthenticationToken);
                    tenant.TenantDomains.AddRange(tenantDomains ?? Enumerable.Empty<string>());

                    using (var subscriptionClient = new HttpClient())
                    {
                        subscriptionClient.DefaultRequestHeaders.Add("Authorization", tenant.AuthenticationToken);
                        tenant.Subscriptions.AddRange(await this.GetSubscriptions(subscriptionClient));
                    }
                }

                return tenants;
            }
        }

        public async Task<Tenant[]> GetTenants(HttpClient httpClient)
        {
            var url = string.Format(
                format: "{0}/tenants?api-version={1}",
                arg0: this.Environment.ResourceManagerUri,
                arg1: this.Environment.ResourceManagerApiVersion);

            var response = await httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            return body.FromJson<ResponseWithContinuation<Tenant[]>>().Value;
        }

        public async Task<Subscription[]> GetSubscriptions(HttpClient httpClient)
        {
            var url = string.Format(
                format: "{0}/subscriptions?api-version=2014-01-01",
                arg0: this.Environment.ResourceManagerUri,
                arg1: this.Environment.ResourceManagerApiVersion);

            var response = await httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            return body.FromJson<ResponseWithContinuation<Subscription[]>>().Value;
        }

        public Task<HttpResponseMessage> GetSubscription(HttpClient httpClient, string subscriptionId)
        {
            var url = string.Format(
                format: "{0}/subscriptions/{1}?api-version={2}",
                arg0: this.Environment.ResourceManagerUri,
                arg1: subscriptionId,
                arg2: this.Environment.ResourceManagerApiVersion);

            return httpClient.GetAsync(url);
        }

        public async Task<string> GetSubscriptionTenantId(string subscriptionId)
        {
            AuthenticationHeaderValue wwwAuthenticateHeader;

            using (var httpClient = new HttpClient())
            {
                // Attempting an ARM request without JWT will return auth discovery header
                var subscriptionResponse = await this.GetSubscription(httpClient, subscriptionId);
                if (subscriptionResponse.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw new InvalidOperationException("Failure requesting subscription tenant id from ARM.");
                }

                wwwAuthenticateHeader = subscriptionResponse.Headers.WwwAuthenticate.Single();
            }

            var tokens = wwwAuthenticateHeader.Parameter.Split('=', ',');
            var authUrl = tokens.ElementAt(1).Trim('"');
            var tenantId = new Uri(authUrl).AbsolutePath.Trim('/');

            return tenantId;
        }
    }
}
