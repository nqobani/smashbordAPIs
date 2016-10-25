using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Part1.Api.Models
{
    public class MessageCountModel
    {
        public long total_message { get; set; }
        public DateTime date { set; get; }
        public IEnumerable<range_buckets> message_states { set;  get;}

    }

    public class range_buckets
    {
        public string key { get; set; }
        public long messages { get; set; }
    }

    public class ResultModel
    {
        public long total_messages { get; set; }
        public IEnumerable<CountAllMessagesModel> users { get; set; }
    }

    public class CountAllMessagesModel
    {
        public string user_id { set; get; }
        public IEnumerable<range_buckets> message_states { set; get; }
    }



    ////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////
    public class UniqueUsersCountModel
    {
        public long AllDocs { get; set; }
        public string Date { get; set; }
        public IEnumerable<pTypeModel> providerTypes { get; set; }
    }

    public class pTypeModel
    {
        public string providerType { get; set; }
        public long DocCount { get; set; }
        public string number_of_users { get; set; }
    }

}