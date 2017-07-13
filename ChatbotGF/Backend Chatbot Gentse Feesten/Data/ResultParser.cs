using Chatbot_GF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF;
using VDS.RDF.Query;

namespace Chatbot_GF.Data
{
    public class ResultParser
    {

        public static Event GetEvent(SparqlResult res)
        {
            Event e = new Event();
            foreach (String key in res.Variables)
            {
                
                    switch (key)
                    {
                        case "sub":
                            e.id = res[key].ToString();
                            break;
                        case "url":
                            e.url = normalizeUrl(res[key].ToString());
                            break;
                        case "name":
                            e.name.nl = normalizeString(res[key].ToString());
                            break;
                        case "startdate":
                            e.startDate = normalizeUrl(res[key].ToString());
                            break;
                        case "enddate":
                            e.endDate = normalizeUrl(res[key].ToString());
                            break;
                        case "description":
                            e.description.nl = normalizeString(res[key].ToString());
                            break;
                        case "organizer":
                            e.organizer = res[key].ToString();
                            break;
                        case "image":
                            e.image = normalizeUrl(res[key].ToString());
                            break;
                        case "location":
                            e.location = normalizeUrl(res[key].ToString());
                            break;
                        case "isFree":
                            e.isAccessibleForFree = ParseBool(res[key].ToString());
                            break;
                        case "isWheelchairUnfriendly":
                            e.isWheelchairUnfriendly = ParseBool(res[key].ToString());
                            break;
                        case "location_name":
                            e.locationName = normalizeString(res[key].ToString());
                            break;
                        default:
                            break;
                    }
                
            }
            return e;
        }

        private static string normalizeString(string s)
        {
            int lastIndex = s.LastIndexOf('@');
            if(lastIndex > 0)
            {
                return s.Substring(0,lastIndex);
            }
            else
            {
                return s;
            }
        }

        private static string normalizeUrl(string url)
        {
            url = url.Replace(" ", "%20");
            int lastIndex = url.LastIndexOf('^') - 1 ;
            if (lastIndex > 0)
            {
                return url.Substring(0, lastIndex);
            }
            else
            {
                return url;
            }
        }

        public static bool? ParseBool(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                //System.Console.WriteLine(str);
                str = normalizeUrl(str);
                return (str.Equals("1") || str.Equals("true"));
            }
            else
            {
                return null;
            }
        }

        public static DateTime normalizeDate(string date)
        {
            DateTime myDate = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm:sszzz",System.Globalization.CultureInfo.InvariantCulture);
            return myDate;
        }
    }
}
