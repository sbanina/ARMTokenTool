namespace ARMTokenApp.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ARMTokenApp.Extensions;

    internal class CommandLineArgumentsParser
    {
        /// <summary>
        /// Gets or sets the parameter values.
        /// </summary>
        private ILookup<string, string> ParamValues { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentsParser" /> class.
        /// </summary>
        /// <param name="commandParameters">The command parameters.</param>
        public CommandLineArgumentsParser(string[] commandParameters)
        {
            this.ParamValues = this.ParseParameters(commandParameters);
        }

        /// <summary>
        /// Checks if all required parameters exist.
        /// </summary>
        /// <param name="requiredParameters">The required parameters.</param>
        public bool CheckIfAllRequiredParametersExists(params string[] requiredParameters)
        {
            return requiredParameters.All(param => this.ParamValues.Contains(param));
        }

        /// <summary>
        /// Gets the string parameter from command parameters
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="defaultValue">The default value.</param>
        internal string GetStringParameter(string parameterName, string defaultValue = null)
        {
            if (this.ParamValues.Contains(parameterName))
            {
                if (this.ParamValues[parameterName].Count() == 1 && !string.IsNullOrWhiteSpace(this.ParamValues[parameterName].Single()))
                {
                    return this.ParamValues[parameterName].Single();
                }
            }
            else if (defaultValue != null)
            {
                return defaultValue;
            }

            throw new CommandParameterException(message: "Invalid or multiple parameters found.", paramName: parameterName);
        }

        /// <summary>
        /// Gets integer parameter from command parameters
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="defaultValue">The default value.</param>
        internal int GetIntParameter(string parameterName, int? defaultValue = null)
        {
            if (defaultValue.HasValue && !this.ParamValues.Contains(parameterName))
            {
                return defaultValue.Value;
            }

            int paramInt;
            if (int.TryParse(this.GetStringParameter(parameterName), out paramInt))
            {
                return paramInt;
            }

            throw new CommandParameterException(message: "Parameter not in int format.", paramName: parameterName);
        }

        /// <summary>
        /// Gets bool parameter from command parameters
        /// </summary>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="defaultValue">The default value.</param>
        internal bool GetBoolParameter(string parameterName, bool? defaultValue = null)
        {
            if (defaultValue.HasValue && !this.ParamValues.Contains(parameterName))
            {
                return defaultValue.Value;
            }

            bool paramBool;
            if (bool.TryParse(this.GetStringParameter(parameterName), out paramBool))
            {
                return paramBool;
            }

            throw new CommandParameterException(message: "Parameter not in bool format.", paramName: parameterName);
        }

        /// <summary>
        /// Gets enum parameter from command parameters
        /// </summary>
        /// <typeparam name="TEnum">The type of enum to be parsed.</typeparam>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="defaultValue">The default value.</param>
        internal TEnum GetEnumParameter<TEnum>(string parameterName, TEnum? defaultValue = null) where TEnum : struct
        {
            if (defaultValue.HasValue && !this.ParamValues.Contains(parameterName))
            {
                return defaultValue.Value;
            }

            string paramEnumString = this.GetStringParameter(parameterName);
            TEnum paramEnum;
            if (Enum.TryParse(value: paramEnumString, ignoreCase: true, result: out paramEnum))
            {
                return paramEnum;
            }

            string possibleValues = Enum.GetNames(typeof(TEnum)).ConcatStrings(",");

            throw new CommandParameterException(
                message: string.Format(
                    format: "Invalid value '{0}'. Possible values parameter '{1}'",
                    arg0: paramEnumString, 
                    arg1: possibleValues),
                paramName: parameterName);
        }

        /// <summary>
        /// Parses parameters and generates the lookup.
        /// </summary>
        /// <param name="commandParameters">The command parameters.</param>
        private ILookup<string, string> ParseParameters(string[] commandParameters)
        {
            List<KeyValuePair<string, string>> paramValues = new List<KeyValuePair<string, string>>();
            if (commandParameters.Count() % 2 != 0)
            {
                throw new CommandParameterException("All parameters should have a value.");
            }

            for (int i = 0; i < commandParameters.Length; i += 2)
            {
                var paramName = commandParameters[i];
                var paramValue = commandParameters[i + 1];
                paramValues.Add(new KeyValuePair<string, string>(paramName, paramValue));
            }

            return paramValues.ToLookup(pair => pair.Key, pair => pair.Value, StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
