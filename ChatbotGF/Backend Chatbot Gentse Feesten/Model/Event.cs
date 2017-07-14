using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDS.RDF.Query;

namespace Chatbot_GF.Model
{
    public class Name
    {
        public string nl { get; set; }
    }

    public class Contributor
    {
        public string @type { get; set; }
        public Name name { get; set; }
    }

    public class Description
    {
        public string nl { get; set; }
    }

   
    /// <summary>
    /// Event object, values can be null
    /// </summary>
    public class Event
    {
        public Event()
        {
            name = new Name();
            description = new Description();
        }

        public Contributor contributor { get; set; }
        public Description description { get; set; }
        public string duration { get; set; }
        public string endDate { get; set; }
        public String image { get; set; }
        public string locationName { get; set; }
        public IList<string> inLanguage { get; set; }
        public Boolean? isAccessibleForFree { get; set; }
        public IList<string> isPartOf { get; set; }
        public Boolean? isWheelchairUnfriendly { get; set; }
        public IList<string> keywords { get; set; }
        public string location { get; set; }
        public Name name { get; set; }
        public IList<object> offers { get; set; }
        public string organizer { get; set; }
        public object startDate { get; set; }
        public IList<string> subEvent { get; set; }
        public string superEvent { get; set; }
        public IList<string> theme { get; set; }
        public object typicalAgeRange { get; set; }
        public string url { get; set; }
        public IList<object> video { get; set; }
        public string changed { get; set; }
        public string @id { get; set; }
    }
}
