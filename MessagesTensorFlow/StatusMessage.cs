using Newtonsoft.Json;

namespace MessagesTensorFlow
{
    public class StatusMessage
    {
        public string message_uuid { get; set; }
        public string timestamp { get; set; }
        public string status { get; set; }
        public ToObj to { get; set; }
        public FromObj from { get; set; }
        public ErrorObj error { get; set; }
        public UsageObj usage { get; set; }
        public string client_ref { get; set; }

        public class ToObj
        {
            [JsonProperty("type")]
            public string type { get; set; }
            [JsonProperty("id")]
            public string id { get; set; }
            [JsonProperty("number")]
            public string number { get; set; }
        }

        public class FromObj
        {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("number")]
            public string number { get; set; }

        }
        public class ErrorObj
        {
            public int code { get; set; }
            public string reason { get; set; }
        }
        public class UsageObj
        {
            public string currency { get; set; }
            public string price { get; set; }
        }

    }
}
