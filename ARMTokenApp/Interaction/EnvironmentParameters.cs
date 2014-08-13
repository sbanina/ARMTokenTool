namespace ARMTokenApp.Interaction
{
    using System;
    using System.Linq;
    using System.Text;

    enum ARMEnvironment
    {
        Unknown = 0,
        Next = 1,
        Current = 2,
        Dogfood = 3,
        Production = 4,
    }

    enum InputMethod
    {
        Unknown = 0,
        Arguments = 1,
        Interactive = 2,
    }

    enum OutputMethod
    {
        Unknown = 0,
        Print = 1,
        Clipboard = 2,
        TempFile = 3,
    }

    internal class EnvironmentParameters
    {
        public string AADUri { get; private set; }

        public string GraphUri { get; private set; }

        public string GraphApiVersion { get; private set; }

        public string ResourceManagerUri { get; private set; }

        public string ResourceManagerApiVersion { get; private set; }

        public string ProvidedSubscriptionId { get; private set; }

        public string ProvidedUserId { get; private set; }

        public OutputMethod OutputMethod { get; private set; }

        private EnvironmentParameters(ARMEnvironment environment, OutputMethod outputMethod, string subscriptionId = null, string userId = null)
        {
            this.AADUri = EnvironmentParameters.GetAADUri(environment: environment);
            this.GraphUri = EnvironmentParameters.GetGraphUri(environment: environment);
            this.GraphApiVersion = "2013-04-05";
            this.ResourceManagerUri = EnvironmentParameters.GetResourceManagerUri(environment: environment);
            this.ResourceManagerApiVersion = "2014-01-01";
            this.ProvidedSubscriptionId = subscriptionId;
            this.ProvidedUserId = userId;
            this.OutputMethod = outputMethod;
        }

        public static string GetUsageInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Example usage: ArmTokenApp.exe -env Production -outputMethod TempFile [-subscriptionId 81eea2b6-df17-43a8-bb2b-a4a217d4de0d] [-userId me@live.com]");
            sb.AppendLine();
            sb.AppendLine("Example usage: ArmTokenApp.exe -inputMethod Interactive");
            sb.AppendLine();
            sb.AppendLine("Arguments:");
            sb.AppendLine("    -env: Target environment can be Next, Current, Dogfood, Production");
            sb.AppendLine("    [-outputMethod]: Token output method can be Print, TempFile, Clipboard");
            sb.AppendLine("         Print: Prints token to standard out. This is the default.");
            sb.AppendLine("         TempFile: Writes token to temp file to open in Notepad. Temp file is deleted immediately.");
            sb.AppendLine("         Clipboard: Copies the token directly to the clipboard");
            sb.AppendLine("    [-inputMethod]: Program input method can be Interactive, Arguments");
            sb.AppendLine("         Arguments: Only looks at command line. This is the default.");
            sb.AppendLine("         Interactive: Prompts for any missing input arguments");
            sb.AppendLine("    [-subscriptionId]: Specify subscription to avoid choosing");
            sb.AppendLine("    [-userId]: Specify user email to avoid typing or bypass AAD auto sign-in");
            return sb.ToString();
        }

        public static EnvironmentParameters InitializeEnvironment(params string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine(EnvironmentParameters.GetUsageInfo());
                Environment.Exit(0);
            }

            var commandParser = new CommandLineArgumentsParser(args);
            var inputMethod = commandParser.GetEnumParameter<InputMethod>(parameterName: "-inputMethod", defaultValue: InputMethod.Arguments);
            var environment = commandParser.GetEnumParameter<ARMEnvironment>(parameterName: "-env", defaultValue: ARMEnvironment.Unknown);
            var outputMethod = commandParser.GetEnumParameter<OutputMethod>(parameterName: "-outputMethod", defaultValue: OutputMethod.Unknown);
            var subscriptionId = commandParser.GetStringParameter(parameterName: "-subscriptionId", defaultValue: string.Empty);
            var userId = commandParser.GetStringParameter(parameterName: "-userId", defaultValue: string.Empty);

            if (inputMethod == InputMethod.Interactive)
            {
                if (environment == ARMEnvironment.Unknown)
                {
                    environment = OutputHelpers.PromptForEnumInput(
                        prompt: "Choose environment by index: ",
                        defaultValue: ARMEnvironment.Production,
                        options: new ARMEnvironment[] { ARMEnvironment.Next, ARMEnvironment.Current, ARMEnvironment.Dogfood, ARMEnvironment.Production });
                }

                if (outputMethod == OutputMethod.Unknown)
                {
                    outputMethod = OutputHelpers.PromptForEnumInput(
                        prompt: "Choose output method by index: ",
                        defaultValue: OutputMethod.TempFile,
                        options: new OutputMethod[] { OutputMethod.Print, OutputMethod.TempFile, OutputMethod.Clipboard });
                }

                if (string.IsNullOrEmpty(subscriptionId))
                {
                    subscriptionId = OutputHelpers.PromptForStringInput("Enter subscription Id (Optional): ");
                }

                if (string.IsNullOrEmpty(userId))
                {
                    userId = OutputHelpers.PromptForStringInput("Enter user email (Optional): ");
                }
            }

            if (environment == ARMEnvironment.Unknown)
            {
                throw new ArgumentException("-env parameter must be specified. Valid values: Next, Current, Dogfood, Production");
            }

            return new EnvironmentParameters(
                environment: environment,
                outputMethod: outputMethod,
                subscriptionId: subscriptionId,
                userId: userId);
        }

        private static string GetResourceManagerUri(ARMEnvironment environment)
        {
            switch (environment)
            {
                case ARMEnvironment.Next:
                    return "https://api-next.resources.windows-int.net";

                case ARMEnvironment.Current:
                    return "https://api-current.resources.windows-int.net";

                case ARMEnvironment.Dogfood:
                    return "https://api-dogfood.resources.windows-int.net";

                case ARMEnvironment.Production:
                    return "https://management.azure.com";

                default:
                    throw new ArgumentException(message: string.Format("Unknown environment: '{0}'", environment));
            }
        }

        private static string GetAADUri(ARMEnvironment environment)
        {
            switch (environment)
            {
                case ARMEnvironment.Next:
                case ARMEnvironment.Current:
                case ARMEnvironment.Dogfood:
                    return "https://login.windows-ppe.net";

                case ARMEnvironment.Production:
                    return "https://login.windows.net";

                default:
                    throw new ArgumentException(message: string.Format("Unknown environment: '{0}'", environment));
            }
        }

        private static string GetGraphUri(ARMEnvironment environment)
        {
            switch (environment)
            {
                case ARMEnvironment.Next:
                case ARMEnvironment.Current:
                case ARMEnvironment.Dogfood:
                    return "https://graph.ppe.windows.net";

                case ARMEnvironment.Production:
                    return "https://graph.windows.net";

                default:
                    throw new ArgumentException(message: string.Format("Unknown environment: '{0}'", environment));
            }
        }
    }
}
