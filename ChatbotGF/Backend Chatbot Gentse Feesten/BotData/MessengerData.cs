using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Chatbot_GF.BotData
{

    // deze klasse wordt gebruikt bij het ophallen van een bericht(json omzetten naar objecten en velden)
    public class MessengerData
    {
        [JsonProperty(PropertyName = "object")]
        public String Object { get; set; }
        public List<Entry> entry { get; set; }
        
        public class Sender
        {
            public long id { get; set; }
        }

        public class Recipient
        {
            public long id { get; set; }
        }

        public class Messaging
        {
            public long timestamp { get; set; }
            public Sender sender { get; set; }
            public Recipient recipient { get; set; }
            public Message message { get; set; }
            public Postback postback { get; set; }

            public class QuickReply
            {
                public string payload { get; set; }
            }

            public class Message
            {
                public string mid { get; set; }
                public string text { get; set; }
                public QuickReply quick_reply { get; set; }
                public List<Attachment> attachments { get; set; }
            }
        }

        public class Entry
        {
            public string id { get; set; }
            public long time { get; set; }
            public List<Messaging> messaging { get; set; }
        }

        public class Postback
        {
            public string payload { get; set; }
        }

        public class Coordinates
        {
            public double lat { get; set; }
            [JsonProperty(PropertyName = "long")]
            public double lon { get; set; }
        }

        public class Payload
        {
            public Coordinates coordinates { get; set; }
        }

        public class Attachment
        {
            public string title { get; set; }
            public string url { get; set; }
            public string type { get; set; }
            public Payload payload { get; set; }
        }
    }
}