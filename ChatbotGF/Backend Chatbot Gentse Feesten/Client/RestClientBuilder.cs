using RestEase;

namespace Chatbot_GF.Client
{
    public class RestClientBuilder
    {
        private readonly static string production = "https://graph.facebook.com/v2.6/me/messages?access_token=EAADbmmTTQZBkBAGCYtymjKzMGGTr817rNVgsqNMAFxxVZCkrvKN5dkJfj88rhy3onuVwCAziCWPB1sBl3Jf5C6FujRZC1g6lRaRk1yW0M5EQvSQiKLFtkbNAYSqFpRZAsuBDqUXYpQz2K5PwZCopyzC5skFa1e7LOUhEZAdelk2QZDZD";
        private readonly static string testing = "https://graph.facebook.com/v2.6/me/messages?access_token=EAALIVzw9ni4BAGdIcwWxyS1Xrr0fnnNZBtgtB7S0fo0kTNa24ZA9VdDJdUg0yP20ett1IGNt9NOZBsSb9cQqZAffYu1lYfZBxBD3ObNfCnhpx8qNGRLLDjd707dUzUk4W3jZAI65WPaEWwZA7BrXzmRurrz28qkOC0ehFc8qWkrDP3XQwv8GAid";
        
        public static IMessengerApi GetMessengerApi()
        {
            return RestClient.For<IMessengerApi>(production);
        }
    }
}
