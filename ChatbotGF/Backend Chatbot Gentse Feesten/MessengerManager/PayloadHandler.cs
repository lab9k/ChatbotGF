using Chatbot_GF.BotData;
using Chatbot_GF.Data;
using Chatbot_GF.MessageBuilder.Factories;
using Chatbot_GF.Model;
using System;
using System.Collections.Generic;
using static Chatbot_GF.BotData.MessengerData;

namespace Chatbot_GF.MessengerManager
{
    public class PayloadHandler : IPayloadHandler
    {
        private ReplyManager rmanager;
        private TempUserData UserLanguage;
        private DataConstants Constants;
        private RemoteDataManager remote;
        private ILocationFactory locationFactory;

        public PayloadHandler(IReplyManager manager, ITempUserData userData, IDataConstants dataConstants, IRemoteDataManager remoteDataManager, ILocationFactory locationFactory)
        {
            rmanager = (ReplyManager)manager;
            UserLanguage = (TempUserData) userData;
            Constants = (DataConstants) dataConstants;
            remote = (RemoteDataManager) remoteDataManager;
            this.locationFactory = locationFactory;
        }

        public void handle(Messaging message)
        {
            try
            {
                long id = message.sender.id;
                PayloadData payload = new PayloadData(message.postback.payload);
                //Console.WriteLine(payload);
                switch (payload.Payload)
                {
                    case "GET_STARTED_PAYLOAD":
                        rmanager.SendWelcomeMessage(id, payload.Language);
                        break;
                    case "SEND_LOCATION_CHOICE":
                        rmanager.SendLocationsChoice(id, payload.Language);
                        break;
                    // tijd keuze extra 
                    case "SEND_DATE_CHOICE":
                        rmanager.SendDateChoice(message.sender.id, payload.Language);
                        break;
                    case "DEVELOPER_DEFINED_DATE_SPECIFIC":
                        rmanager.SendDayOption(message.sender.id, payload.Language);
                        break;
                    case "SEND_LANGUAGE_OPTIONS":
                        rmanager.ChangeLanguage(message.sender.id);
                        break;
                    case "GET_TOILET":
                        string[] co = payload.Value.Split(':');
                        SearchableLocation location = Constants.GetClosestsToilet(double.Parse(co[0]), double.Parse(co[1]));
                        // Console.WriteLine($"Closest location found: {location.PrettyName} ");
                        rmanager.SendGenericMessage(locationFactory.MakeLocationResponse(id, location.Lat, location.Lon, payload.Language));
                        break;
                    case "SET_LANGUAGE":
                        rmanager.SendWelcomeMessage(id, payload.Value);
                        break;
                    case "GET_USER_LOCATION":
                        if(!string.IsNullOrWhiteSpace(payload.Value) && payload.Value == "true")
                        {
                            UserLanguage.Add(id, payload.Language, true);
                        }
                        else
                        {
                            UserLanguage.Add(id, payload.Language, false);
                        }
                        rmanager.SendGetLocationButton(message.sender.id,payload.Language);
                        break;
                    case "DEVELOPER_DEFINED_SEARCHFALSE":
                        rmanager.SendInfoForEnding(message.sender.id, payload.Language);
                        break;
                    case "GET_EVENT_HERE_NOW":
                        rmanager.SendLocationQuery(id, int.Parse(payload.Value), payload.Language);
                        break;
                    case "DEVELOPER_DEFINED_LOCATION":
                        if(payload.Value != "ALL")
                            remote.GetEventsHereNow(id, Constants.GetLocation(payload.Value).Id, Constants.Now,payload.Language);
                        else
                            remote.GetEventsNow(id,payload.Language, Constants.Now);
                        break;
                    case "DEVELOPER_DEFINED_DAY":
                        rmanager.SendTimePeriod(id, payload.Value, payload.Language);
                        break;
                    case "DEVELOPER_DEFINED_COORDINATES":
                        string[] data = payload.Value.Split(':');
                        List<SearchableLocation> locatio = Constants.GetClosestLocation(new Coordinates { lat = double.Parse(data[1]), lon = double.Parse(data[0]) },3);
                       // Console.WriteLine($"Closest location found: {location.PrettyName} ");
                        rmanager.SendLocationResult(id, locatio, payload.Language);
                        break;
                    case "DEVELOPER_DEFINED_DESCRIPTION":
                        rmanager.SendTextMessage(id, payload.Value);
                        break;
                    case "DEVELOPER_DEFINED_HOURS":
                        string[] dat = payload.Value.Split('|');
                        rmanager.SendHoursChoice(id, dat[0], dat[1], payload.Language);
                        break;
                    case "DEVELOPER_DEFINED_HOURS_COMP":
                        string[] da = payload.Value.Split('|');
                        payload.Value = $"{da[1]}{da[0]}:00+02:00";
                        //rmanager.SendTextMessage(id, value);
                        //Console.WriteLine("Datum: " + payload.Value);
                        remote.GetEventsAtTime(id, payload.Value,payload.Language);
                        break;

                    case "DEVELOPER_DEFINED_NEXT":
                        // moet nog normaal gezet worden maar voor test gevallen is het deze tijd
                        int pos = payload.Value.IndexOf("-_-");
                        remote.GetNextEvents(payload.Value.Substring(0, pos), payload.Value.Substring(pos + 3), 3, id, payload.Language);
                        break;
                    case "GET_HELP":
                        rmanager.SendHelpMessage(id);
                        break;
                    case "GET_HELP_PAGE":
                        rmanager.SendTextMessage(id, "Comming Soon (2017)");
                        break;
                    default:
                        //do nothing
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

    }
}
