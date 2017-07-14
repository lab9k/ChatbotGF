using System.Collections.Generic;
using System.Linq;

namespace Chatbot_GF.BotData
{
/// <summary>
/// Parses payload string into object
/// </summary>
    public class PayloadData : List<string>
    {
        private string _Language;
        public string Payload { get; set; }
        public string Value { get; set; }

        public PayloadData(string input)
        {
            string[] list = input.Split('°');
            Payload = list[0];
            if (list.Count() > 1)
            {
                Value = list[1];
                if(list.Count()> 2)
                {
                    _Language = list[2];
                }
            }
        }      

        public string Language {
            get {
                if (string.IsNullOrEmpty(_Language))
                    _Language = "Gents";
                return _Language;
            }
            set { _Language = value; }
        }

        public override string ToString()
        {
            return $"Payload: {Payload}, Value: {Value}, Language: {Language}";
        }
    }
}
