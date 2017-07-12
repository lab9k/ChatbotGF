using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_GF.Data
{
    public interface ITempUserData
    {
        void Remove(long id);
        void Add(long id, string lang, bool? toilet);
    }
}
