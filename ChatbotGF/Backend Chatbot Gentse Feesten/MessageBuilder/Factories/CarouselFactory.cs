using Chatbot_GF.Data;
using Chatbot_GF.MessageBuilder.Model;
using Chatbot_GF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Chatbot_GF.MessageBuilder.Factories
{

    /// <summary>
    /// Creates a Messenger reply object, must be parsed to JSON and posted back to Messenger.
    /// </summary>
    public class CarouselFactory : ICarouselFactory
    {
        private DataConstants Constants;

        public CarouselFactory(IDataConstants Constants )
        {
            this.Constants = (DataConstants) Constants;
        }

        public GenericMessage makeCarousel(long id, List<Event> events, string lang)
        {
            if (events.Count > 10)
            {
                events = events.GetRange(0, 10);
            }

            List<Element> elements = new List<Element>();
            foreach (var eve in events)
            {
                List<IButton> buttons = new List<IButton>();
                DefaultAction defaultAction = new DefaultAction("web_url", MakeUrl(eve.name.nl), true);
                if (!string.IsNullOrWhiteSpace(eve.description?.nl))
                {
                    if (eve.description.nl.Length < 640)
                    {
                        buttons.Add(new ButtonPayload(Constants.GetMessage("What_Is_It", lang), "postback", "DEVELOPER_DEFINED_DESCRIPTION°" + eve.description.nl + "°" + lang));
                    } else
                    {
                        buttons.Add(new ButtonPayload(Constants.GetMessage("What_Is_It", lang), "postback", "DEVELOPER_DEFINED_DESCRIPTION°" + eve.description.nl.Substring(0, 635) + "..." + "°" + lang));
                    }
                }

                buttons.Add(new ButtonPayload(Constants.GetMessage("NEXT", lang), "postback", "DEVELOPER_DEFINED_NEXT°" + eve.location + "-_-" + eve.startDate + "°" + lang));

                var image = eve.image;
                if (string.IsNullOrEmpty(image))
                {
                    image = "https://chatbot.lab9k.gent/images/default0.png";
                }

                string dates = " ";
                string juli = $"{Constants.GetMessage("MONTH", lang)} ";
                if (eve.startDate.ToString().Equals(eve.endDate.ToString()))
                {
                    string[] helpStart = eve.startDate.ToString().Split('T');
                    string[] daySt = helpStart[0].Split('-');
                    string[] hourSt = helpStart[1].Split(':');
                    dates += daySt[2];
                    dates += juli;
                    dates += hourSt[0];
                    dates += ":";
                    dates += hourSt[1];
                    string[] helpEnd = eve.endDate.ToString().Split('T');
                    string[] hourEnd = helpEnd[1].Split(':');
                    dates += " - ";
                    dates += hourEnd[0];
                    dates += ":";
                    dates += hourEnd[1];
                    //dates = $" | {start.Day} juli {start.ToString("HH:mm")} - {end.ToString("HH:mm")}";
                }
                else
                {
                    string[] helpStart = eve.startDate.ToString().Split('T');
                    string[] daySt = helpStart[0].Split('-');
                    string[] hourSt = helpStart[1].Split(':');
                    dates += daySt[2];
                    dates += juli;
                    dates += hourSt[0];
                    dates += ":";
                    dates += hourSt[1];

                    string[] helpEnd = eve.endDate.ToString().Split('T');
                    string[] dayEnd = helpEnd[0].Split('-');
                    string[] hourEnd = helpEnd[1].Split(':');
                    dates += " - ";
                    dates += dayEnd[2];
                    dates += juli;
                    dates += hourEnd[0];
                    dates += ":";
                    dates += hourEnd[1];
                }
                buttons.Add(new ButtonShare());
                string free = ((eve.isAccessibleForFree ?? false) == true) ? Constants.GetMessage("FREE", lang) : Constants.GetMessage("NOTFREE", lang);
                string wheelie = "";
                if (!(eve.isWheelchairUnfriendly ?? true))
                {
                    wheelie = " | ♿";
                }
                string loc = (Constants.GetLocation(eve.location)?.PrettyName ?? (eve.locationName ?? "???"));
                if (loc.Length > 21)
                {
                    loc = loc.Substring(0, 20) + "...";
                }
                string subtitle =  loc + " | " + dates + " | " + free + wheelie;
                elements.Add(new Element(eve.name.nl, image, subtitle, buttons, defaultAction));
            }
            IPayload payload = new PayloadMessage("generic", elements, true, "horizontal");
            Attachment attachment = new Attachment("template", payload);
            return new GenericMessage(id, attachment);
        }

        public static string MakeUrl(string name)
        {
            return "https://gentsefeesten.stad.gent/nl/search?search=" + name;
        }

    }
}
