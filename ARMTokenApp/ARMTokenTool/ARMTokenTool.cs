namespace ARMTokenApp
{
    using System;
    using System.Net;
    using ARMTokenApp.Extensions;
    using ARMTokenApp.Interaction;
    using ARMTokenApp.Providers;

    class ARMTokenTool
    {
        static void Main(string[] args)
        {
            ARMTokenTool.ConfigureServicePointManager();
            ARMTokenTool.RunTool(environment: EnvironmentParameters.InitializeEnvironment(args));
        }

        private static void RunTool(EnvironmentParameters environment)
        {
            var resourceManagerDataProvider = new ResourceManagerDataProvider(environment: environment);

            string authorizationHeader = null;
            if (environment.ProvidedSubscriptionId.IsGuid())
            {
                var tenantId = resourceManagerDataProvider
                    .GetSubscriptionTenantId(subscriptionId: environment.ProvidedSubscriptionId)
                    .Result;

                authorizationHeader = new AuthenticationProvider(environment: environment)
                    .GetAuthorizationResult(tenantId: tenantId)
                    .CreateAuthorizationHeader();
            }
            else
            {
                var tenants = resourceManagerDataProvider.GetTenantsWithSubscriptions().Result;
                authorizationHeader = OutputHelpers.PromptForTokenSelection(tenants);
            }

            OutputHelpers.OutputToken(outputMethod: environment.OutputMethod, token: authorizationHeader);
        }

        private static void ConfigureServicePointManager()
        {
            ServicePointManager.DefaultConnectionLimit = Environment.ProcessorCount * 48;
            ServicePointManager.MaxServicePointIdleTime = 90000;
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.Expect100Continue = false;
        }
    }
}
