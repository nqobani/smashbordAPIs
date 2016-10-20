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
    [RoutePrefix("api/MessageStats")]
    public class MessageStatsController : ApiController
    {
        private readonly ICountMessages _icountMessage;

        public MessageStatsController(ICountMessages icountMessage)
        {
            _icountMessage = icountMessage;
        }

        public IEnumerable<CountAllMessagesEntity> GetMessageStats()
        {
            var results = _icountMessage.GetMessageStats();

            return results.Select(s => new CountAllMessagesEntity
            {
                total_messages = s.total_messages,
                message_states = s.message_states
            });
        }

        [Route("date_range")]
        public IEnumerable<MessageCountEntity> GetMessageStat(string fromDate = "now-24H/H", string toDate = "now")
        {
            var results = _icountMessage.GetMessageStat(fromDate, toDate);

            return results.Select(i => new MessageCountEntity
            {
                total_message = i.total_message,
                date = i.date,
                message_states = i.message_states
            });
        }

        [Route("mult_date_ranges")]
        public IEnumerable<MessageCountEntity> GetMessageStats( string interval, string goBackBy="1M")
        {
            var results = _icountMessage.GetMessageStats(interval, goBackBy);

            return results.Select(i => new MessageCountEntity
            {
                total_message = i.total_message,
                date = i.date,
                message_states = i.message_states
            });
        }
    }
}
