using Part1.Api.Models;
using Part1.ApplicationLogic.Entities;
using Part1.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="startDate">startDate is a staring point of the date range</param>
        /// <param name="endDate"></param>
        /// <param name="goBackBy"></param>
        /// <param name="providerType"></param>
        /// <returns></returns>
        [Route("groupby_range")]
        public IEnumerable<MessageCountEntity> GetMessageStats( string interval, string startDate="", string endDate="", string goBackBy= "", string providerType = "all")
        {
            var results = _icountMessage.GetMessageStats(interval, startDate, endDate, goBackBy, providerType);

            return results.Select(i => new MessageCountEntity
            {
                total_message = i.total_message,
                date = i.date,
                message_states = i.message_states
            });
        }
        /// <summary>
        /// This is itjhgj jvgjbn jgjvbj jgjhghj 
        /// </summary>
        /// <param name="userType">sedwseds</param>
        /// <param name="startDate">sdsd</param>
        /// <param name="interval">sdsdsds</param>
        /// <returns></returns>
        [Route("user")]
        public IEnumerable<UniqueUsersCountEntity> GetMessagesUniqueUsers(string userType, string startDate="", string interval="month")
        {
            var results = _icountMessage.GetMessagesUniqueUsers(userType, startDate, interval);

            return results.Select(s => new UniqueUsersCountEntity {
                Date = s.Date,
                AllDocs= s.AllDocs,
                providerTypes = s.providerTypes
            });
        }
    }
}
