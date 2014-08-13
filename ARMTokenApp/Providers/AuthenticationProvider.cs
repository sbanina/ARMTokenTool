namespace ARMTokenApp.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using ARMTokenApp.DataModel;
    using ARMTokenApp.Extensions;
    using ARMTokenApp.Interaction;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;


    internal class AuthenticationProvider
    {
        private EnvironmentParameters Environment { get; set; }

        private Dictionary<TokenCacheKey, string> TokenCache { get; set; }

        public AuthenticationProvider(EnvironmentParameters environment)
        {
            this.Environment = environment;
            this.TokenCache = new Dictionary<TokenCacheKey, string>();
        }

        public AuthenticationResult GetAuthorizationResult(string tenantId = "common", string userId = null)
        {
            userId = !string.IsNullOrEmpty(userId) ? userId : this.Environment.ProvidedUserId;
            AuthenticationResult result = null;
            var thread = new Thread(() =>
            {
                try
                {
                    var context = new AuthenticationContext(
                        authority: this.Environment.AADUri + "/" + tenantId,
                        validateAuthority: true,
                        tokenCacheStore: this.TokenCache);

                    if (!string.IsNullOrEmpty(userId))
                    {
                        result = context.AcquireToken(
                            resource: "https://management.core.windows.net/",
                            clientId: "1950a258-227b-4e31-a9cf-717495945fc2",
                            redirectUri: new Uri("urn:ietf:wg:oauth:2.0:oob"),
                            userId: userId);
                    }
                    else
                    {
                        result = context.AcquireToken(
                            resource: "https://management.core.windows.net/",
                            clientId: "1950a258-227b-4e31-a9cf-717495945fc2",
                            redirectUri: new Uri("urn:ietf:wg:oauth:2.0:oob"),
                            promptBehavior: PromptBehavior.Always);
                    }
                }
                catch (Exception threadEx)
                {
                    Console.WriteLine(threadEx.Message);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return result;
        }

        public async Task<string[]> GetDefaultTenantDomains(string tenantId, string token)
        {
            var tenantDetail = await this.GetTenantDetails(tenantId, token);
            if (tenantDetail != null && tenantDetail.VerifiedDomains != null)
            {
                return tenantDetail.VerifiedDomains
                    .Where(domain => domain.Default)
                    .Select(domain => domain.Name)
                    .ToArray();
            }

            return null;
        }

        public async Task<GraphTenantDetail> GetTenantDetails(string tenantId, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);

                    var url = string.Format(
                        format: "{0}/{1}/tenantDetails/{1}?api-version={2}",
                        arg0: this.Environment.GraphUri,
                        arg1: tenantId,
                        arg2: this.Environment.GraphApiVersion);

                    var response = await httpClient.GetAsync(url);
                    var body = await response.Content.ReadAsStringAsync();

                    return body.FromJson<GraphTenantDetail>();
                }
            }
            catch
            {
                // calling graph is best effort. just return null if failure.
                return null;
            }
        }
    }
}
