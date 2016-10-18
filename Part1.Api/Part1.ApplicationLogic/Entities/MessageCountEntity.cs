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
        //public range_buckets[] message_states { set; get; }
        public string key { get; set; }
        public long messages { get; set; }
    }

    //public class range_buckets
    //{
    //    public string key { get; set; }
    //    public long messages { get; set; }
    //}
}
