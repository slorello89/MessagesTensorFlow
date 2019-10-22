using Newtonsoft.Json;

namespace MessagesTensorFlow
{
    public class InboundMessage
    {
        [JsonProperty("message_uuid")]
        public string message_uuid { get; set; }

        [JsonProperty("to")]
        public ToObj to { get; set; }

        [JsonProperty("from")]
        public FromObj from { get; set; }

        [JsonProperty("timestamp")]
        public string timestamp { get; set; }

        [JsonProperty("message")]
        public MessageObj message { get; set; }
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
        public class MessageObj
        {
            [JsonProperty("content")]
            public Content content { get; set; }
        }
        public class Content
        {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("text")]
            public string text { get; set; }

            [JsonProperty("image")]
            public Image image { get; set; }

            [JsonProperty("audio")]
            public Audio audio { get; set; }

            [JsonProperty("video")]
            public Video video { get; set; }

            [JsonProperty("file")]
            public File file { get; set; }

            [JsonProperty("location")]
            public Location location { get; set; }
        }

        public class Image
        {
            [JsonProperty("url")]
            public string url { get; set; }

            [JsonProperty("caption")]
            public string caption { get; set; }
        }
        public class Audio
        {
            [JsonProperty("url")]
            public string url { get; set; }
        }
        public class Video
        {
            [JsonProperty("url")]
            public string url { get; set; }

            [JsonProperty("caption")]
            public string caption { get; set; }
        }
        public class File
        {
            [JsonProperty("url")]
            public string url { get; set; }

            [JsonProperty("caption")]
            public string caption { get; set; }
        }
        public class Location
        {
            [JsonProperty("lat")]
            public string lat { get; set; }

            [JsonProperty("long")]
            public string Longitude { get; set; }

            [JsonProperty("url")]
            public string url { get; set; }

            [JsonProperty("address")]
            public string address { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }
        }


    }
}
