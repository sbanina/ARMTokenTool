namespace ARMTokenApp.Interaction
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using ARMTokenApp.DataModel;
    using ARMTokenApp.Extensions;

    static class OutputHelpers
    {
        public static TEnum PromptForEnumInput<TEnum>(string prompt, TEnum defaultValue, TEnum[] options) where TEnum : struct
        {
            for (var index = 0; index < options.Length; index++)
            {
                var optionDetails = string.Format(
                    format: "[{0}]: {1} {2}",
                    arg0: index,
                    arg1: options[index],
                    arg2: defaultValue.Equals(options[index]) ? "(Default)" : null);

                Console.WriteLine(optionDetails);
            }

            var chosenIndex = -1;
            while (chosenIndex < 0 || chosenIndex >= options.Length)
            {
                Console.Write(prompt);
                var inputString = Console.ReadLine();

                if (string.IsNullOrEmpty(inputString))
                {
                    return defaultValue;
                }

                chosenIndex = int.Parse(inputString);
            }

            return options[chosenIndex];
        }

        public static string PromptForStringInput(string prompt)
        {
            Console.Write(prompt);
            var inputString = Console.ReadLine();

            return inputString;
        }

        public static string PromptForTokenSelection(Tenant[] tenants)
        {
            if (!tenants.Any())
            {
                Console.WriteLine("No tenants for this user in ARM. Press any key to terminate...");
                Console.ReadKey();
                Environment.Exit(0);
            }

            var tenantIndex = 0;
            foreach (var tenant in tenants)
            {
                Console.WriteLine(string.Format(
                    format: "[{0}]: Tenant '{1}' {2}contains:",
                    arg0: tenantIndex++,
                    arg1: tenant.TenantId,
                    arg2: tenant.TenantDomains == null ? string.Empty : string.Format("({0}) ", tenant.TenantDomains.ConcatStrings(", "))));

                if (!tenant.Subscriptions.Any())
                {
                    Console.WriteLine("No subscriptions".Indent(numberOfSpaces: 2) + Environment.NewLine);
                    continue;
                }

                foreach (var subscription in tenant.Subscriptions)
                {
                    var subscriptionDetail = string.Format(
                        format: "Subscription '{0}' ({1})",
                        arg0: subscription.SubscriptionId,
                        arg1: subscription.DisplayName);

                    Console.WriteLine(subscriptionDetail.Indent(numberOfSpaces: 2));
                }

                Console.WriteLine();
            }

            var chosenIndex = -1;
            while (chosenIndex < 0 || chosenIndex >= tenantIndex)
            {
                Console.Write("Choose tenant by index: ");
                chosenIndex = int.Parse(Console.ReadLine());
            }

            return tenants
                .Select(tenant => tenant.AuthenticationToken)
                .Skip(chosenIndex)
                .FirstOrDefault();
        }

        public static void OutputToken(OutputMethod outputMethod, string token)
        {
            switch (outputMethod)
            {
                case OutputMethod.Clipboard:
                    OutputHelpers.CopyToClipboard(text: token);
                    return;

                case OutputMethod.TempFile:
                    OutputHelpers.WriteToTempFile(text: token);
                    return;

                case OutputMethod.Print:
                default:
                    Console.WriteLine(token);
                    return;
            }
        }

        private static void CopyToClipboard(string text)
        {
            var thread = new Thread(() => Clipboard.SetText(text: text, format: TextDataFormat.Text));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private static void WriteToTempFile(string text)
        {
            var tempFilePath = Path.GetTempFileName();
            File.WriteAllText(path: tempFilePath, contents: text);
            Process.Start(fileName: "notepad.exe", arguments: tempFilePath).WaitForInputIdle();
            File.Delete(path: tempFilePath);
        }
    }
}
