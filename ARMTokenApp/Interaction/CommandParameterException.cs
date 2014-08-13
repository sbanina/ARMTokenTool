namespace ARMTokenApp.Interaction
{
    using System;

    internal class CommandParameterException : Exception
    {
        public CommandParameterException(string message)
            : base(message)
        {
        }

        public CommandParameterException(string message, string paramName)
            : base(string.Format("{0} for Parameter : '{1}'", message, paramName))
        {
        }
    }
}
