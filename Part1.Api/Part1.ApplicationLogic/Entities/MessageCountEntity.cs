using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part1.ApplicationLogic.Entities
{
    public class MessageCountEntity
    {
        public long total_message { get; set; }
        public DateTime date { set; get; }
        public IEnumerable<range_buckets> message_states { set; get; }

    }
    public class MessageStatsEntity
    {
        public long total_message { get; set; }
        public IEnumerable<range_buckets> message_states { set; get; }

    }

    public class range_buckets
    {
        public string key { get; set; }
        public long messages { get; set; }
    }


    public class ResultEntity
    {
        public long total_messages { get; set; }
        public IEnumerable<CountAllMessagesEntity> users { get; set; }
    }
    public class CountAllMessagesEntity
    {
        public string user_id { set; get; }
        public IEnumerable<range_buckets> message_states { set; get; }
    }




    ////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////////Ntobeko////
    public class UniqueUsersCountEntity
    {
        public long AllDocs { get; set; }
        public string Date { get; set; }
        public IEnumerable<pTypeEntity> providerTypes { get; set; }
    }

    public class pTypeEntity
    {
        public string providerType { get; set; }
        public long DocCount { get; set; }
        public string number_of_users { get; set; }
    }
    public class tenantsEntity
    {
        public string key { get; set; }
        public long docCount { get; set; }
    }
}
