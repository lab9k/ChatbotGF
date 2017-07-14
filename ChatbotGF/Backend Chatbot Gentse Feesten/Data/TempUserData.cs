using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_GF.Data
{
    /// <summary>
    /// Temporarily stores user data to provide a working flow
    /// </summary>
    public class TempUserData : ITempUserData
    {
        private Dictionary<long, UserData> UserLanguage;

        private Dictionary<DateTime, long> LastConnected;
        private Dictionary<long, DateTime> InverseLastConnected;

        private static TempUserData instance;
        private static readonly int MAX_USERS = 500;
        private static readonly int KEEP_ALIVE_MINUTES = 10;
        public TempUserData()
        {
            UserLanguage = new Dictionary<long, UserData>();
            LastConnected = new Dictionary<DateTime, long>();
            InverseLastConnected = new Dictionary<long, DateTime>();

        }

        public class UserData
        {
            public string Lang { get; set; }
            public bool Toilet { get; set; }
        }
        /// <summary>
        /// Removes all users that did not perform any action in x minutes 
        /// </summary>
        /// <param name="minutes"></param>
        public void CleanMaps(int minutes)
        {
            DateTime threshhold = DateTime.Now.AddMinutes(minutes * 2);
            foreach (DateTime time in LastConnected.Keys)
            {

                if (DateTime.Compare(time, threshhold) < 0) //date is before threshhold
                {
                    Remove(LastConnected[time]);
                }
            }

        }
        public string GetLanguage(long id)
        {
            if (UserLanguage.ContainsKey(id))
            {
                string lang = UserLanguage[id].Lang;
                //Remove(id);
                return lang;
            }
            else
            {
                return null;
            }
        }

        public bool WantsToilet(long id)
        {
            if (UserLanguage.ContainsKey(id))
            {
                bool res = UserLanguage[id].Toilet;
                Console.WriteLine($"Found id: {res}");
                //Remove(id);
                return res;
            }
            else
            {
                return false;
            }
        }

        public void Remove(long id)
        {
            if (UserLanguage.ContainsKey(id))//Data could already be deleted
            {
                LastConnected.Remove(InverseLastConnected[id]);
                InverseLastConnected.Remove(id);
                UserLanguage.Remove(id);
            }
        }

        public void Add(long id, string lang, bool? toilet)
        {
            try
            {
                Remove(id);
                DateTime now = DateTime.Now;
                Console.WriteLine("Added user, toilet= " + (toilet ?? false));
                UserLanguage.Add(id, new UserData { Lang = lang, Toilet = (toilet ?? false) });
                LastConnected.Add(now, id);
                InverseLastConnected.Add(id, now);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            if(UserLanguage.Count > MAX_USERS)
                CleanMaps(KEEP_ALIVE_MINUTES); //remove users that did not connect in more than x minutes
        }



    }
}
