using Chatbot_GF.Client;
using Chatbot_GF.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestEase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Chatbot_GF.BotData.MessengerData;

namespace Chatbot_GF.Data
{
    public class DataConstants : IDataConstants
    {
        public int numberLocations;
        private IConfigurationRoot LocationsStore;
        private IConfigurationRoot MessagesStore;
        private IConfigurationRoot QueryStore;
        private IConfigurationRoot ConfigStore;

        public DataConstants()
        {
            MessagesStore = init("messages.json");
            QueryStore = init("queries.json");
            ConfigStore = init("config.json");
        }

        

        public List<SearchableLocation> Locations
        {
            get {
                if (locations == null)
                    initLocations();
                return locations;
            }
        }
        
        private List<SearchableLocation> locations;
        private List<SearchableLocation> toilets;
        private readonly int TOILET_COUNT = 171;

        public List<SearchableLocation> Toilets {
            get {
                if (toilets == null)
                    initToilets();
                return toilets;
            }
        }

        public IMessengerApi GetMessengerApi()
        {
            return RestClient.For<IMessengerApi>(GetConfig("apiUrl", "production"));
        }


        private void initToilets()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("locations.json");

            IConfiguration toiletStore = builder.Build();
            toilets = new List<SearchableLocation>();

            for (int i = 0; i < TOILET_COUNT; i++)
            {
                    
                //Console.WriteLine("Toilet toegevoegd");
                toilets.Add(new SearchableLocation { Lon = double.Parse(toiletStore[$"toilets:{i}:{0}"]), Lat = double.Parse(toiletStore[$"toilets:{i}:{1}"]) });
            }
        }

        public SearchableLocation GetClosestsToilet(double Lon, double Lat)
        {
            return GetClosestLocation(Toilets, new Coordinates { lon = Lon, lat = Lat });
        }

        public SearchableLocation GetLocationBySearchTag(string tag)
        {
            foreach(SearchableLocation loc in Locations)
            {
                if(loc.Search != null)
                {
                    if (loc.Search.Contains(tag.ToLower()))
                    {
                        return loc;
                    }
                }
            }
            return null;
        }

        public string GetQuery(string name)
        {
            return QueryStore[name];
        }

        public DateTime Now
        {
            get { return DateTime.Now.AddDays(5).AddHours(6); }
        }

        private IConfigurationRoot init(string json)
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile(json);
            return builder.Build();
        }


        private void initLocations()
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("locations.json");
            LocationsStore = builder.Build();
            locations = new List<SearchableLocation>();
            numberLocations = int.Parse(LocationsStore["LocationsCount"]);
            for (int i = 0; i < numberLocations; i++)
            {
                locations.Add(new SearchableLocation
                {
                    Name = LocationsStore[$"locations:{i}:Name"],
                    PrettyName = LocationsStore[$"locations:{i}:PrettyName"],
                    Id = LocationsStore[$"locations:{i}:Id"],
                    Lat = double.Parse(LocationsStore[$"locations:{i}:Lat"]),
                    Lon = double.Parse(LocationsStore[$"locations:{i}:Lon"])
                });
                locations[i].Search = new List<string>();
                if (!string.IsNullOrWhiteSpace(LocationsStore[$"locations:{i}:SearchCount"]))
                {
                    int count = int.Parse(LocationsStore[$"locations:{i}:SearchCount"]);
                    for (int j=0; j < count; j++)
                    {
                        string tag = LocationsStore[$"locations:{i}:Search:{j}"];
                        //Console.WriteLine("Tag found: " + tag);
                        locations[i].Search.Add(tag);
                    }
                }
                    
            }
        }

        public string GetMessage(string name, string locale)
        {
            return MessagesStore[$"messages:{name}:{locale}"];
        }

        public string GetConfig(string type, string name)
        {
            return ConfigStore[$"config:{type}:{name}"];
        }

        public SearchableLocation GetLocation(string name){
            if (locations == null)
                initLocations();
            foreach(SearchableLocation loc in locations)
            { 
                if (loc.Id == name || loc.Name.Contains(name) || loc.PrettyName.Contains(name))
                {
                    return loc;
                }
            }
            return null;
        }

        public List<SearchableLocation> GetClosestLocation(Coordinates coors, int count)
        {
            List<SearchableLocation> closests = new List<SearchableLocation>();
            List<SearchableLocation> locations = new List<SearchableLocation>(Locations); //shallow clone
            for(int i = 0; i < count; i++)
            {
                SearchableLocation close = GetClosestLocation(locations, coors);
                locations.Remove(close);
                closests.Add(close);
            }
            return closests;
        }

        public SearchableLocation GetClosestLocation(List<SearchableLocation> locations,Coordinates coors)
        {
            SearchableLocation closests = locations[0];
            double dx = Locations[0].Lon - coors.lon;
            double dy = Locations[0].Lat - coors.lat;
            double shortestDistance = Math.Sqrt(dx * dx + dy * dy);
            for(int i = 1; i < locations.Count; i++)
            {
                dx = locations[i].Lon - coors.lon;
                dy = locations[i].Lat - coors.lat;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                if(distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closests = locations[i];
                }
            }
            return closests;
        }
    }
}
