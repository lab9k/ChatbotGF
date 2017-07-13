using Chatbot_GF.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private ILogger<FreeTextHandler> _logger;


        public FreeTextHandler(ILogger<FreeTextHandler> logger,IReplyManager rmanager, IDataConstants Constants, IRemoteDataManager Remote)
        {
            _logger = logger;
            RMmanager = (ReplyManager)rmanager;
            this.Constants =(DataConstants) Constants;
            this.Remote = (RemoteDataManager)Remote;
            InitReplies();
        }
        private void InitReplies()
        {
            _logger.LogInformation(this.GetType().ToString() + "Loading replies");

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
                _logger.LogInformation(this.GetType().ToString() + "Know location found");

                Remote.GetEventsHereNow(id, Constants.GetLocationBySearchTag(text).Id, Constants.Now, "NL", 3);
            }
            else
            {
                res = GetResponse(text);
                if (res != null)
                {
                    RMmanager.SendTextMessage(id, res);
                    _logger.LogInformation(this.GetType().ToString() + "Keywords found, replying with defined message");

                }
                else if(text.Length > 3)
                {
                    _logger.LogInformation(this.GetType().ToString() + "Searching for event by name");
                    Remote.GetEventByName(text, id, "NL");
                }
                else
                {
                    _logger.LogInformation(this.GetType().ToString() + "Input too short");

                    RMmanager.SendTextMessage(id, Constants.GetMessage("INVALID_INPUT", "NL"));
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
                    if (!string.IsNullOrWhiteSpace(ReplyStore.GetSection(query + ":" + words[count])?.GetValue<string>("response")))
                    {
                        KeywordsFound.Add(words[count]);
                        words.RemoveAt(count);
                        count = 0; //restart
                    }
                    else if (ReplyStore[query + ":haschildren"] != null && ReplyStore[query + ":haschildren"] == "false")
                    {
                        break; //stop recursion, object has no child keywords
                    }
                    else
                    {
                        count++;
                    }
                }
                return ReplyStore[string.Join(":", KeywordsFound) + ":response"];
            }catch(Exception ex) {
                _logger.LogWarning(101, ex, "Exception while searching for keyword");
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

                    if (!string.IsNullOrWhiteSpace(ReplyStore.GetSection(query + ":" + words[count])?.GetValue<string>("payload")))
                    {
                        KeywordsFound.Add(words[count]);
                        words.RemoveAt(count);
                        count = 0; //restart
                    }
                    else if (ReplyStore[query + ":haschildren"] != null && ReplyStore[query + ":haschildren"] == "false")
                    {
                        break; //stop recursion, object has no child keywords
                    }
                    else
                    {
                        count++;
                    }
                }
                return ReplyStore[string.Join(":", KeywordsFound) + ":payload"];
            }
            catch (Exception ex)
            {
                _logger.LogWarning(101, ex, "Exception while searching for keyword");
                return null;
            }
        }

        public static bool IsEqual(string first, string second, int tollerance)
        {
            return string.Compare(first, second) < tollerance;
        }

    }
}
