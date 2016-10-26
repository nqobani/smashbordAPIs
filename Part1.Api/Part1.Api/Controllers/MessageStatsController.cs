﻿using Part1.Api.Models;
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
        /// <param name="startDate">startDate is a staring point of the date range. The date format must be like this>> YYYY-MM-DD (e.g. 2016-10-25). There are also special words and abbreviation ('interval') that can be used instead of full dates. <br />Special words examples: day, week, month, year, now, etc <br /><br /> 
        /// abbreviation examples:<br/> h >> Hour in 12hours time, <br/>H >>Hour in 24Hours time, <br/>w >> week, <br/>Y >> full Year e.g (2016),<br/> y >> short year (16),<br/> m >> Minute,<br/> M >> Month etc
        /// </param>
        /// <param name="endDate">Is similar to startDate, the only difference is that it is the ending point of the date range.</param>
        /// <param name="goBackBy">getBackBy works well will time/date abbreviations( h >> Hour in 12hours time, H >>Hour in 24Hours time, w >> week, etc) to specify the time to go back by(e.g. goBackBy=3M - The starting point will be this month/now minus 3 months)</param>
        /// <param name="providerType">providerType can be used to as a search provider type, If you want states about facebook messages only, you can put 'facebook' as a provide type.</param>
        /// <returns></returns>
        /// 
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
        /// <param name="userType">UserType can be facebook, twitter, sms, chat, or all. "all" will return all user types, while "twitter" can only return twitter user types, facebook, chat and sms also do the same as twitter</param>
        /// <param name="startDate">startDate is a starting point of the date range. It start from a date specified in startDate and end on a current date.</param>
        /// <param name="interval">interval works as group by. It can be used to group the result by hour, day, week, month, year etc.</param>
        /// <returns></returns>
        /// 
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


        [Route("tenants")]
        public IEnumerable<tenantsEntity> GetStatsByTenant(string startingPoint = "")
        {
            var t = _icountMessage.GetByTenant(startingPoint);
            return t.Select(i => new tenantsEntity
            {
                key = i.key,
                docCount = i.docCount
            });
        }
    }
}
