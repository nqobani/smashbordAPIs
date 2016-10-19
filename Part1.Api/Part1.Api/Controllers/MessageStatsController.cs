using Part1.Api.Models;
using Part1.ApplicationLogic.Entities;
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

        //public IEnumerable<MessageCountModel> GetMessageStates()
        //{
        //    var results = _icountMessage.GetMessageStats();

        //    return results.Select(s => new MessageCountModel {
        //        total_message = s.total_message,
        //        key = s.key,
        //        messages = s.messages
        //    }); 
        //}

        //public IEnumerable<MessageCountEntity> GetMessageStats(string fromDate = "now-24H/H", string toDate = "now")
        //{
        //    var results = _icountMessage.GetMessageStats(fromDate, toDate);

        //    return results.Select(i => new MessageCountEntity
        //    {
        //        total_message = i.total_message,
        //        message_states = i.message_states
        //    });
        //}

        public IEnumerable<MessageCountEntity> GetMessageStats(string fromDate = "now-24H/H", string toDate = "now", string interval = "", string goBackBy="2M")
        {
            var results = _icountMessage.GetMessageStats(fromDate, toDate, interval, goBackBy);

            return results.Select(i => new MessageCountEntity
            {
                total_message = i.total_message,
                date = i.date,
                message_states = i.message_states
            });
        }

    }
}
