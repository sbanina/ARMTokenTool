namespace ARMTokenApp.DataModel
{
    using System.Collections;
    using Newtonsoft.Json;

    internal class ResponseWithContinuation<T> where T : IEnumerable
    {
        [JsonProperty]
        public T Value { get; set; }

        [JsonProperty]
        public string NextLink { get; set; }
    }
}
