using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_GF.MessengerManager
{
    public interface ITextHandler
    {
        void CheckText(long id, string text);
        string GetResponse(string text);
        string GetPayload(string text);
    }
}
