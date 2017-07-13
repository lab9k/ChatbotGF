using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static Chatbot_GF.BotData.MessengerData;
using Chatbot_GF.BotData;
using Chatbot_GF.MessengerManager;
using Chatbot_GF.Data;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Chatbot_GF.Controllers
{
    [Route("api/[controller]")]
    public class MessengerController : Controller
    {
        private IMessageHandler mhandler;
        private IPayloadHandler phandler;
        private TempUserData uData;
        private readonly ILogger<MessengerController> _logger;

        public MessengerController(ILogger<MessengerController> logger, IPayloadHandler phandler, IMessageHandler mhandler, ITempUserData uData)
        {
            this.mhandler = mhandler;
            this.phandler = phandler;
            this.uData = (TempUserData)uData;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var allUrlKeyValues = Request.Query;
            if (allUrlKeyValues["hub.mode"] == "subscribe" && allUrlKeyValues["hub.verify_token"] == "test123")
            {
                _logger.LogInformation("Messenger GET verification received");
                var returnVal = allUrlKeyValues["hub.challenge"];
                return Json(int.Parse(returnVal));
            }
            return NotFound();
        }


        [HttpPost]
        public ActionResult Post([FromBody] MessengerData data)
        {
            

            Task.Factory.StartNew(() =>
            {
                foreach (var entry in data.entry)
                {
                    foreach (var message in entry.messaging)
                    { 
                        // Check current message if text is recognized and sets corresponding payload
                        Messaging currentMessage = mhandler.MessageRecognized(message);
                        if (currentMessage.postback != null)
                        {
                            _logger.LogInformation("Messenger postback data received");
                            phandler.handle(message);
                        }
                        else if (!string.IsNullOrWhiteSpace(currentMessage?.message?.quick_reply?.payload))
                        {
                            //set the quick reply payload as the message payload
                            currentMessage.postback = new Postback { payload = message.message.quick_reply.payload };
                            _logger.LogInformation("Messenger quickreply data received");
                            phandler.handle(message);
                        }
                        else if(currentMessage?.message?.attachments != null)
                        {
                            try
                            {
                                Attachment locationAtt = currentMessage?.message?.attachments[0];
                                Coordinates coords = locationAtt.payload?.coordinates;
                                string lang = uData.GetLanguage(currentMessage.sender.id);
                                if (string.IsNullOrWhiteSpace(lang))
                                    lang = "";
                                if (!uData.WantsToilet(message.sender.id))
                                {
                                    currentMessage.postback = new Postback { payload = $"DEVELOPER_DEFINED_COORDINATES°{coords.lon}:{coords.lat}°{lang}" };
                                    _logger.LogInformation($"Messenger locationdata received, toilet: false, lat: {coords.lat}, long {coords.lon}");
                                    phandler.handle(message);
                                }
                                else
                                {
                                    currentMessage.postback = new Postback { payload = $"GET_TOILET°{coords.lon}:{coords.lat}°{lang}" };
                                    _logger.LogInformation($"Messenger locationdata received, toilet: true, lat: {coords.lat}, long {coords.lon}");
                                    phandler.handle(message);
                                }
                                uData.Remove(message.sender.id); //Remove the user from the set
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            
                        }
                        else
                        {
                            _logger.LogInformation("Messenger text data received");
                            mhandler.CheckForKnowText(currentMessage);
                        }
                    }
                }
            });
            return Ok();
        }
    }
}