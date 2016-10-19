﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Part1.Api.Models
{
    public class MessageCountModel
    {
        public long total_message { get; set; }
        public IEnumerable<range_buckets> message_states { set;  get;}

    }

    public class range_buckets
    {
        public string key { get; set; }
        public long messages { get; set; }
    }
    
}