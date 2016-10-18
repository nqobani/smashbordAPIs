using Part1.Api.Models;
using Part1.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Part1.Api.Controllers
{
    public class MessageStatsController : ApiController
    {
        private readonly ICountMessages _icountMessage;

        public MessageStatsController(ICountMessages icountMessage)
        {
            _icountMessage = icountMessage;
        }

        public IEnumerable<MessageCountModel> GetMessageStates()
        {
            var results = _icountMessage.GetMessageStats();

            return results.Select(s => new MessageCountModel {
                total_message = s.total_message,
                key = s.key,
                messages = s.messages
            }); 
        }

    }
}
