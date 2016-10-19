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

    public class range_buckets
    {
        public string key { get; set; }
        public long messages { get; set; }
    }

}
