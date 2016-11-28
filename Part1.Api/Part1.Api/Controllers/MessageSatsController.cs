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

    [RoutePrefix("api")]
    public class MessageController : ApiController
    {
        private readonly ICountMessages _icountMessage;
        public MessageController(ICountMessages icountMessage)
        {
            _icountMessage = icountMessage;
        }
        /// <summary>
        /// Get the number of messages on each provider type based on a time range.
        /// </summary>
        /// <param name="startDate">startDate is a starting point of the date range. It takes the date in the format : YYYY-MM-DD(e.g. 2016-10-28). The startDate must always be a date before the endDate.</param>
        /// <param name="endDate">endDate is an ending point of the date range. Also in the format: YYYY-MM-DD, It must be a date after the date in startDate. </param>
        /// <returns></returns>
        /// 
        [Authorize]
        [Route("messages/all")]
        public IEnumerable<MessageStatsEntity> GetMessageStat(string startDate = "now-24H/H", string endDate = "now")
        {
            var results = _icountMessage.GetMessageStat(startDate, endDate);

            return results.Select(i => new MessageStatsEntity
            {
                total_message = i.total_message,
                message_states = i.message_states
            });
        }
        /// <summary>
        /// GET messages groubed by time interval (hour,day,week,month etc.)<br/><br/>
        /// Intervals are used to group the result (stats) from a specified time range. When you specify the time/date range always put your intervals into considuration because they play a big role on the data that you get from the response e.g. Putting a range that is equal to <i>one and ahalf month(one and ahalf of your interval) and group your results by month</i>, may return a confusing data. A time range 2016-09-15 to 2016-10-31 will return the data from that secified but it will indicate it as if the range was  2016-09-01 to 2016-10-31 (<i><b>Note: It won't included the stats for 2016-09-01 to 2016-09-14</b></i>).
        /// </summary>
        /// <param name="interval">interval works as group by. It can be used to group the results by hour, day, week, month, year etc. <br/> <b>Note:</b> this papareter can not be empty</param>
        /// <param name="startDate">startDate is a starting point of the date range. It takes the date in the format : YYYY-MM-DD(e.g. 2016-10-28). You must make sure that it is always a date before the endDate
        /// </param>
        /// <param name="endDate">Is similar to startDate, the only difference is that it is the ending point of the date range.You must make sure that it is always a date after the startDate</param>
        /// <param name="goBackBy">getBackBy works well will time/date abbreviations( h >> Hour in 12hours time, H >>Hour in 24Hours time, w >> week, etc) to specify the time to go back by(e.g. goBackBy=3M - The starting point will be this month/now minus 3 months)</param>
        /// <param name="providerType">ProviderType allow you to get stats for a/the specific provider type/s. There are four provider types: facebook, twitter, chat and sms. You can specicy a single providerType or multiple providerTypes seperated by a comma(',') for example: facebook,chat,sms or you can just put all if you want them all</param>
        ///<param name="mustNotMath">It take a list ot providerTypes seperated by a comma(',') for example: sms,chat,facebook... The providerTypes you pass in this paremeter won't be included in the response/stats</param>
        /// <returns></returns>
        /// 
        [Authorize]
        [Route("messages/compare")]
        public IEnumerable<MessageCountEntity> GetMessageStats(string interval, string startDate = "", string endDate = "", string goBackBy = "",string mustNotMath = "", string providerType = "all")
        {
            var l = User.Identity;

            //try
            //{

            if (interval.Equals("")|| interval.Equals(null))
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Response: 'Invalid request: interval can not be null'")),
                    ReasonPhrase = "Reason:'Interval can not be null...'"
                };
                throw new HttpResponseException(resp);
            }
            //DateTime startD = Convert.ToDateTime(startDate);
            //    DateTime endD = Convert.ToDateTime(endDate);
            //    if(startD>endD)
            //    {
            //        var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
            //        {
            //            Content = new StringContent(string.Format("Response: 'Invalid date range'")),
            //            ReasonPhrase = "Reason:'The end date can on be a date before the start date'"
            //        };
            //        throw new HttpResponseException(resp);
            //    }
            
            //}
            //catch (Exception)
            //{
            //    var errorCause = new HttpResponseMessage(HttpStatusCode.NotFound)
            //    {
            //        Content = new StringContent(string.Format("Invalid Invalid request, Make sure that you inputs are correct, the required field is not null/empty and parameter name are correct")),
            //        ReasonPhrase = "The end date can on be a date before the start date"
            //    };
            //    throw new HttpResponseException(HttpStatusCode.NotFound);
            //}

            var results = _icountMessage.GetMessageStats(interval, startDate, endDate, goBackBy, mustNotMath, providerType);
            return results.Select(i => new MessageCountEntity
            {
                total_message = i.total_message,
                date = i.date,
                message_states = i.message_states
            });

        }
        /// <summary>
        /// GET the number of user on each provider type based on a time range and grouped by hour, month, day, etc.
        /// </summary>
        /// <param name="userType">TypeOfUser can be facebook, twitter, sms, chat, or all. "all" will return the stats about all user types</param>
        /// <param name="startDate">startDate is a starting point of the date range. It takes in the date in this format: YYYY-MM-DD for example: 2016-10-22. If it is not specified, it will default to the first day of the current year</param>
        /// <param name="interval">interval works as group by. It can be used to group the result by hour, day, week, month, year etc. The default is week.</param>
        /// <param name="endDate">endDate is the ending point of the date range. It takes in the date in this format: YYYY-MM-DD for example: 2016-10-22. If it is not specified, it will default to the current date</param>
        /// <param name="excludeUserType">Takes in a list of user types seperated by a comma(',') for example: sms,chat,facebook... the user types specified in this field will not be included in the response.<br>NOTE: all is not supported</br></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [Route("messages/user")]
        public IEnumerable<UniqueUsersCountEntity> GetMessagesUniqueUsers(string userType,string excludeUserType="", string startDate = "", string endDate="", string interval = "month")
        {
            var results = _icountMessage.GetMessagesUniqueUsers(userType, excludeUserType, startDate, endDate, interval);

            return results.Select(s => new UniqueUsersCountEntity
            {
                Date = s.Date,
                AllDocs = s.AllDocs,
                providerTypes = s.providerTypes
            });
        }
    }
}
