﻿using Part1.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Part1.ApplicationLogic.Entities;
using Nest;
using Part1.Data.EsModels;

namespace Part1.ApplicationLogic.Services
{
    public class MessageCountService : ICountMessages
    {
        private readonly IElasticClient _elasticClient;

        public MessageCountService(IElasticClient client)
        {
            _elasticClient = client;
        }
        public IEnumerable<MessageCountEntity> GetMessageStats()
        {
            var response = _elasticClient.Search<MessageElasticModel>(s => s
                           .Aggregations(a => a
                           .DateRange("date_range", dateR => dateR
                           .Field(f => f.SentAt)
                           .Format("YYYY-MM-DD")
                           .Ranges(
                               r => r.To("now"),
                               r => r.From("now-24H")
                            )
                            .Aggregations(a2 => a2
                            .SignificantTerms("signifint_terms", st => st
                            .Field(f2 => f2.Provider.Type))))));

            var agg = response.Aggs.DateRange("date_range").Items;


            
            int count = agg.Count;


            var items = agg.Select(i => new MessageCountEntity
            {
                total_message = i.DocCount,
                
            });

            // throw new NotImplementedException();
            return items;
        }
    }
}
