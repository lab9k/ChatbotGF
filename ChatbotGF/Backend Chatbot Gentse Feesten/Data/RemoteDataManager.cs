﻿using Chatbot_GF.Client;
using Chatbot_GF.Controllers;
using Chatbot_GF.MessageBuilder.Factories;
using Chatbot_GF.MessageBuilder.Model;
using Chatbot_GF.MessengerManager;
using Chatbot_GF.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace Chatbot_GF.Data
{
    public class RemoteDataManager : IRemoteDataManager { 

        private SparqlRemoteEndpoint endpoint;  
        private ReplyManager rm;
        private DataConstants constants;
        private ICarouselFactory carouselFactory;

        public RemoteDataManager(IDataConstants constants,IReplyManager rManager, ICarouselFactory carouselFactory)
        {
            endpoint = new SparqlRemoteEndpoint(new Uri("https://stad.gent/sparql"), "http://stad.gent/gentse-feesten/");
            rm =(ReplyManager) rManager;
            this.constants = (DataConstants) constants;
            this.carouselFactory = carouselFactory;
        }

       

        public void GetEventsHereNow(long id,string location,DateTime now,string language)
        {
            string formattedTime = now.ToString("yyyy-MM-ddTHH:mm:sszzz");
            string locationfilter = "str(?location) = \"" + location + "\"";
            string startdatefilter = "?startdate < \"" + formattedTime + "\" ^^ xsd:dateTime";
            string enddatefilter = "?enddate > \"" + formattedTime + "\" ^^ xsd:dateTime";
            string query = constants.GetQuery("base") + string.Format(constants.GetQuery("EventsNowHere"), locationfilter, startdatefilter, enddatefilter);
            endpoint.QueryWithResultSet(query, new SparqlResultsCallback(callback), new CallbackData { Id = id, Language = language });
        }

        public void GetEventsAtTime(long id, string date,string language)
        {
            string startdatefilter = "?startdate < \"" + date + "\" ^^ xsd:dateTime";
            string enddatefilter = "?enddate > \"" + date + "\" ^^ xsd:dateTime";
            List<SearchableLocation> locations = constants.Locations;
            string locationFilters = "str(?location) = \"" + locations[0].Id + "\"";
            for (int i = 1; i < locations.Count; i++)
            {
                locationFilters += " || str(?location) = \"" + locations[i].Id + "\"";
            }
            string query = constants.GetQuery("base") + string.Format(constants.GetQuery("EventsNowHere"), locationFilters, startdatefilter, enddatefilter);
            endpoint.QueryWithResultSet(query, new SparqlResultsCallback(callback), new CallbackData {Id = id, Language = language });
        }

        public void GetEventsNow(long id, string lang, DateTime time)
        {
            string formattedTime = time.ToString("yyyy-MM-ddTHH:mm:sszzz");
            string startdatefilter = "?startdate < \"" + formattedTime + "\" ^^ xsd:dateTime";
            string enddatefilter = "?enddate > \"" + formattedTime + "\" ^^ xsd:dateTime";
            //filter over defined locations only
            List<SearchableLocation> locations = constants.Locations;
            string locationFilters = "str(?location) = \"" + locations[0].Id + "\"";
            for(int i = 1; i<locations.Count; i++)
            {
                locationFilters += " || str(?location) = \"" + locations[i].Id + "\"";
            }
            string query = constants.GetQuery("base") + string.Format(constants.GetQuery("EventsNowHere"),locationFilters,startdatefilter,enddatefilter);
            endpoint.QueryWithResultSet(query, new SparqlResultsCallback(callback), new CallbackData {Id = id, Language = lang });
        }
        public  void GetNextEvents(string locationurl,string date, int count, long id,string lang)
        {
            string query = constants.GetQuery("base") + string.Format(constants.GetQuery("NextEventsOnLocation"), locationurl, date, count);
            endpoint.QueryWithResultSet(query, new SparqlResultsCallback(callback), new CallbackData { Id = id, Language = lang });
        }
        public void GetEventByName(string locationName, long id, string lang)
        {
            try
            {
                string formattedTime = constants.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                string query = constants.GetQuery("base") + string.Format(constants.GetQuery("SearchByName"), locationName.ToLower(), formattedTime);
                endpoint.QueryWithResultSet(query, new SparqlResultsCallback(callback), new CallbackData { Id = id, Language = lang });
            }catch(Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }


        public void callback(SparqlResultSet results, Object u)
        {
            try
            {
                List<Event> events = new List<Event>();
                IMessengerApi api = RestClientBuilder.GetMessengerApi();
                
                if (results.Count > 0 && u is CallbackData)
                {
                    CallbackData user = (CallbackData)u;
                    foreach (SparqlResult res in results)
                    {
                        try
                        {
                            Event e = ResultParser.GetEvent(res);
                            events.Add(e);
                        } catch (Exception ex)
                        {
                            System.Console.WriteLine(ex);
                        }
                    }
                    rm.SendTextMessage(user.Id, constants.GetMessage("Found", user.Language));
                    String result = api.SendMessageToUser(carouselFactory.makeCarousel(user.Id, events,user.Language)).Result;
                }
                else if(u is CallbackData)
                {
                    CallbackData user = (CallbackData)u;
                    rm.SendNoEventFound(user.Id, user.Language);
                    rm.SendConfirmation(user.Id, user.Language);
                }
                else if (u is VDS.RDF.AsyncError)
                {
                    VDS.RDF.AsyncError error = (VDS.RDF.AsyncError)u;
                    CallbackData user = (CallbackData)error.State;
                    string hmess = constants.GetMessage("Error", user.Language);
                    rm.SendTextMessage(user.Id, hmess);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        public class CallbackData
        {
            public string Language { get; set; }
            public long Id { get; set; }
        }
    }
}
