using Chatbot_GF.Data;
using RestEase;

namespace Chatbot_GF.Client
{
    public class RestClientBuilder
    {

        
        public static IMessengerApi GetMessengerApi()
        {
            DataConstants Constants = new DataConstants();
            return RestClient.For<IMessengerApi>(Constants.GetConfig("apiUrl", "production"));
        }
    }
}
