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
    [RoutePrefix("api/message")]
    public class MessageStatsController : ApiController
    {
        private readonly ICountMessages _icountMessage;

        public MessageStatsController(ICountMessages icountMessage)
        {
            _icountMessage = icountMessage;
        }
        [Route("users")]
        public ResultEntity GetMessageStats()
        {
            var results = _icountMessage.GetMessageStats();

            var user =  results.users.Select(s => new CountAllMessagesEntity
            {
                user_id = s.user_id,
                message_states = s.message_states
            });
            ResultEntity res = new ResultEntity();
            res.users = user;
            res.total_messages= results.total_messages;


            return res;
        }

        [Route("singleRange")]
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

        [Route("multRange")]
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
