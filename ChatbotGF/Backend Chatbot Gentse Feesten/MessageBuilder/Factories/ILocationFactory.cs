using Chatbot_GF.MessageBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_GF.MessageBuilder.Factories
{
    public interface ILocationFactory
    {
        GenericMessage MakeLocationButton(long id, string lang);
        GenericMessage MakeLocationResponse(long id, double lat, double lon, string lang);
    }
}
