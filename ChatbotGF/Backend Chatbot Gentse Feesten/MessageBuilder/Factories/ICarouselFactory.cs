using Chatbot_GF.MessageBuilder.Model;
using Chatbot_GF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_GF.MessageBuilder.Factories
{
    public interface ICarouselFactory
    {
        GenericMessage makeCarousel(long id, List<Event> events, string lang);
    }
}
