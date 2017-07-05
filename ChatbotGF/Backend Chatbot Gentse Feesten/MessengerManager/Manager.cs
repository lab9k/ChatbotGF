﻿using Chatbot_GF.Data;
using Chatbot_GF.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_GF.MessengerManager
{
    public enum SearchState { START, PROGRESS, READYTO}
   

    public class Manager
    {
        public static Dictionary<String, String> locations_WithURL;
        private ReplyManager reply;
        public Dictionary<long, User> activeUsers;
        private RemoteDataManager dataDAO;
        private string[] locaties = { "BAUDELOHOF", "BEVERHOUTPLEINPLACEMUSETTE", "SINTJACOBS", "CENTRUM", "STADSHAL", "EMILE BRAUNPLEIN", "LUISTERPLEIN", "GROENTENMARKT", "KORENLEI-GRASLEI", "KORENMARKT", "SINTBAAFSPLEIN", "STVEERLEPLEIN", "VLASMARKT", "VRIJDAGMARKT", "WILLEM DE BEERSTEEG"};
        private string[] urls =
        {
            "https://gentsefeesten.stad.gent/api/v1/location/ffc36b40-f30a-4f4c-9512-ee1367c45fc3",
            "https://gentsefeesten.stad.gent/api/v1/location/a9b88475-05d5-4d52-8787-764f75827599",
            "https://gentsefeesten.stad.gent/api/v1/location/2917fe2d-8597-46ab-be01-a5e0a97447c2",
            "https://gentsefeesten.stad.gent/api/v1/location/f792034c-a8e5-4260-ab6b-2adeecf759a6",
            "https://gentsefeesten.stad.gent/api/v1/location/15864533-d5da-4425-897c-162d5e155558",
            "https://gentsefeesten.stad.gent/api/v1/location/c9218a9b-0340-40ed-b2f8-67c74271cb58",
            "https://gentsefeesten.stad.gent/api/v1/location/39a81c28-09ac-496d-acb6-f10aa9db0aea",
            "https://gentsefeesten.stad.gent/api/v1/location/41f39d3d-e906-4333-b590-f273a0a979e1",
            "https://gentsefeesten.stad.gent/api/v1/location/5a123130-716e-4923-a59d-3bbcb863890d",
            "https://gentsefeesten.stad.gent/api/v1/location/bfbb81fb-ede4-4bb9-b09a-ba40390c6596",
            "https://gentsefeesten.stad.gent/api/v1/location/eb4ea42e-e8e0-4945-b098-8b8c98bdb4d1",
            "https://gentsefeesten.stad.gent/api/v1/location/f614c847-2cab-432a-b0f0-6639f8a5ac57",
            "https://gentsefeesten.stad.gent/api/v1/location/0d5f41fa-c8bc-47e3-aac2-c88fcdb29352",
            "https://gentsefeesten.stad.gent/api/v1/location/29e0000a-5cf8-467f-98b8-a57a92ef0298",
            "https://gentsefeesten.stad.gent/api/v1/location/c2559115-bfcb-4cf3-9d4f-a5147f99e371"
        };



        public Manager()
        {
            dataDAO = new RemoteDataManager();
            activeUsers = new Dictionary<long, User>();
            locations_WithURL = new Dictionary<String, String>();
            reply = new ReplyManager();
            //nog te verplaatsen naar propertiesbestand!
            for (int i=0; i<locaties.Count(); i++)
            {
                locations_WithURL.Add(locaties[i], urls[i]);
            }
        }
        /// <summary>
        /// Method to be called when user pushes on "get Started" or after
        /// --> makes a new user in the list + saves his id and timestamp + gives a welcom message etc
        /// </summary>
        /// <param name="id"></param>
        public void startUser(long id)
        {
            activeUsers.Add(id, new User(id));
            saveUsers(id, DateTime.Now);

            reply.SendWelcomeMessage(id);
        }

        
        /// <summary>
        /// Saves all the users + timestamps for case of emergency + knowing how many users
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stamp"></param>
        private void saveUsers(long id, DateTime stamp)
        {
            string path = Path.GetTempFileName();
            string line = "" + id + "\t" + stamp;
            // This text is a1lways added, making the file longer over time
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine("This");
            }

        }

        /// <summary>
        /// Method for each event call from messenger
        /// </summary>
        /// <param name="id"></param>
        public void changeUserState(long id, string payload)
        {
            User currentUser;  //contains the user object linked to the messengerperson who sends an event

            if (!activeUsers.ContainsKey(id))
            {
                currentUser = new User(id);
                activeUsers.Add(id, currentUser);
            }
            else{
                currentUser = activeUsers[id];
            }

            //payload indicates which category data in messengers has been given
            int pos = payload.IndexOf("-");
            String category = payload.Substring(0,pos);
            String value = payload.Substring(pos + 1);

            if (category.Equals("DEVELOPER_DEFINED_LOCATION"))
            { //location button choosed
                setLocationChoice(currentUser,value);
                //standaard enkel nu
            } else if (category.Equals("DEVELOPER_DEFINED_SEARCH")){
                searchResults(currentUser);
            } else if (category.Equals("DEVELOPER_DEFINED_TIME"))
            {
                setTimeChoice(currentUser,value);
            }
            
        }

        private void setTimeChoice(User user, string value)
        {
            //hier tijd invoeren, voorlopig nog basisuur
            user.date = DateTime.Now.AddDays(10).AddHours(9);
        }

        private void searchResults(User user)
        {
            //hier effectieve zoekmethode uitvoeren + gebruiker verwijderen uit active lijst           
            dataDAO.GetEventsHereNow(user);
            activeUsers.Remove(user.id);
           
        }

        private void setLocationChoice(User user,String value)
        {
            // user has clicked on location button: three possibilities
            if (value.Equals("MY_LOCATION"))
            {
                //eigen locatie bepalen en toevoegen met afstandsformule enzovoort
                //voorlopig hardcodering vlasmarkt als locatie
                user.location = locations_WithURL["VLASMARKT"];
            }
            else
            {
                //specific location: use hashmap to get link for request
                user.location = locations_WithURL[value];
            }
        }
        





    }
}
