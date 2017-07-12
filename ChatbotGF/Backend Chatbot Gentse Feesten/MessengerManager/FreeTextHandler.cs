using Chatbot_GF.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_GF.MessengerManager
{
    public class FreeTextHandler : ITextHandler
    {
        private IConfiguration ReplyStore;
        private ReplyManager RMmanager;
        private DataConstants Constants;
        private RemoteDataManager Remote;


        public FreeTextHandler(IReplyManager rmanager, IDataConstants Constants, IRemoteDataManager Remote)
        {
            RMmanager = (ReplyManager)rmanager;
            this.Constants =(DataConstants) Constants;
            this.Remote = (RemoteDataManager)Remote;
            InitReplies();
        }
        private void InitReplies()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("replies.json");
            ReplyStore = builder.Build();
        }


        public void CheckText(long id,string text)
        {
            string res;
            if (!string.IsNullOrEmpty(Constants.GetLocationBySearchTag(text)?.Id))
            {
                Remote.GetEventsHereNow(id, Constants.GetLocationBySearchTag(text).Id, Constants.Now, "NL");
            }
            else
            {
                res = GetResponse(text);
                if (res != null)
                {
                    RMmanager.SendTextMessage(id, res);
                }
                else if(text.Length > 3)
                {
                    Remote.GetEventByName(text, id, "NL");
                }
                else
                {
                    //Send a message
                }
            }
        }
        
        private string RemoveNonAlphanumerics(string text)
        {
            char[] arr = text.Where(c => (char.IsLetterOrDigit(c) ||
                             char.IsWhiteSpace(c) ||
                             c == '-')).ToArray();

            return new string(arr);
        }

        public string GetResponse(string text)
        {
            
            try
            {
                text = RemoveNonAlphanumerics(text); 

                List<string> words = text.ToLower().Split(' ').ToList();
                List<string> KeywordsFound = new List<string>();
                KeywordsFound.Add("keywords");
                int count = 0;
                while (count < words.Count)
                {                    
                    string query = string.Join(":", KeywordsFound);
                    //Console.WriteLine("Searching " + query + ":" + words[count]);
                    //Console.WriteLine(ReplyStore["keywords:feestje:waar:response:nl"]);
                    if (!string.IsNullOrWhiteSpace(ReplyStore.GetSection(query + ":" + words[count])?.GetValue<string>("response")))
                    {
                       // Console.WriteLine(query + ":" + words[count] + "Keyword found!");
                        KeywordsFound.Add(words[count]);
                        words.RemoveAt(count);
                        count = 0; //restart
                    }
                    else if (ReplyStore[query + ":haschildren"] != null && ReplyStore[query + ":haschildren"] == "false")
                    {
                       // Console.WriteLine("No children, strop recursion");
                        break; //stop recursion, object has no child keywords
                    }
                    else
                    {
                        count++;
                    }
                }
                //Console.WriteLine(string.Join(":", KeywordsFound) + ":response");
                return ReplyStore[string.Join(":", KeywordsFound) + ":response"];
            }catch(Exception ex)
            {
                //Console.WriteLine(ex);
                return null;
            }
        }

        public string GetPayload(string text)
        {
            try
            {
                text = RemoveNonAlphanumerics(text);

                List<string> words = text.ToLower().Split(' ').ToList();
                List<string> KeywordsFound = new List<string>();
                KeywordsFound.Add("payloads");
                int count = 0;
                while (count < words.Count)
                {
                    string query = string.Join(":", KeywordsFound);
                   // Console.WriteLine("Searching " + query + ":" + words[count]);
                    //Console.WriteLine(ReplyStore["keywords:feestje:waar:response:nl"]);

                    if (!string.IsNullOrWhiteSpace(ReplyStore.GetSection(query + ":" + words[count])?.GetValue<string>("payload")))
                    {
                        //Console.WriteLine(query + ":" + words[count] + "Keyword found!");
                        KeywordsFound.Add(words[count]);
                        words.RemoveAt(count);
                        count = 0; //restart
                    }
                    else if (ReplyStore[query + ":haschildren"] != null && ReplyStore[query + ":haschildren"] == "false")
                    {
                       // Console.WriteLine("No children, stop recursion");
                        break; //stop recursion, object has no child keywords
                    }
                    else
                    {
                        count++;
                    }
                }
                //Console.WriteLine(string.Join(":", KeywordsFound) + ":payload");
                return ReplyStore[string.Join(":", KeywordsFound) + ":payload"];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool IsEqual(string first, string second, int tollerance)
        {
            return string.Compare(first, second) < tollerance;
        }

    }
}
