using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Chatbot_GF.BotData.MessengerData;

namespace Chatbot_GF.MessengerManager
{
    public interface IMessageHandler
    {
        void CheckForKnowText(Messaging message);
        Messaging MessageRecognized(Messaging message);
    }
}
