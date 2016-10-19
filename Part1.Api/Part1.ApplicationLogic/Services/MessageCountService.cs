using Part1.ApplicationLogic.Interfaces;
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
        //public IEnumerable<MessageCountEntity> GetMessageStats()
        //{
        //    var response = _elasticClient.Search<MessageElasticModel>(s => s
        //                   .Aggregations(a => a
        //                   .DateRange("date_range", dateR => dateR
        //                   .Field(f => f.ReceivedAt)
        //                   .Format("YYYY-MM-DD")
        //                   .Ranges(
        //                       r => r.To("now"),
        //                       r => r.From("now-24H")
        //                    )
        //                    .Aggregations(a2 => a2
        //                    .SignificantTerms("signifint_terms", st => st
        //                    .Field(f2 => f2.Provider.Type))))));

        //    var agg = response.Aggs.DateRange("date_range").Items;

            
        //    int count = agg.Count;


        //    var items = agg.Select(i => new MessageCountEntity
        //    {
        //        total_message = i.DocCount,
                
        //    });

        //    // throw new NotImplementedException();
        //    return items;
        //}

        //public IEnumerable<MessageCountEntity> GetMessageStats(string fromDate="2016-10-01", string toDate= "2016-10-19")
        //{
        //    var response = _elasticClient.Search<MessageElasticModel>(s => s
        //                   .Aggregations(a => a
        //                   .DateRange("date_range", dateR => dateR
        //                   .Field(f => f.ReceivedAt)
        //                   .Format("YYYY-MM-DD")
        //                   .Ranges(
        //                       r => r.From("2016-10-01").To("2016-10-19")
        //                    )
        //                    .Aggregations(a2 => a2
        //                    .SignificantTerms("signifint_terms", st => st
        //                    .Field(f2 => f2.Provider.Type))))));

        //    var agg = response.Aggs.DateRange("date_range").Items;


        //    var items = agg.Select(i => new MessageCountEntity
        //    {
        //        total_message = i.DocCount,
        //        message_states = i.SignificantTerms("signifint_terms").Items
        //                        .Select(st => new range_buckets {
        //                            key = st.Key,
        //                            messages = st.DocCount
        //                        })

        //    });

        //    return items;
        //}

        public IEnumerable<MessageCountEntity> GetMessageStats(string fromDate, string toDate, string interval, string goBackBy /*This variable will contain time(period) that the aggrigation will have to start from */)
        {
            if(interval.Length > 0)
            {
                var response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                      .Query(q => q.Range(r => r.OnField(of => of.ReceivedAt).GreaterOrEquals("now-"+goBackBy)))
                                                                      .Size(0)
                                                                      .Aggregations(a => a.DateHistogram("date_histogram", dh => dh
                                                                                                                          .Field(f => f.ReceivedAt)
                                                                                                                          .Interval(interval)
                                                                                                                          .Aggregations(a2 => a2.SignificantTerms("significant_terss", st => st
                                                                                                                                                                                       .Field(f => f.Provider.Type))))));


                var aggs = response.Aggs.DateHistogram("date_histogram").Items;
                var terms = aggs.Select(s => new MessageCountEntity
                {
                    total_message = s.DocCount,
                    date = s.Date,
                    message_states = s.SignificantTerms("significant_terss").Items
                                     .Select(ss => new range_buckets
                                     {
                                         key = ss.Key,
                                         messages = ss.DocCount
                                     })
                });

                return terms;
            }
            else
            {
                var response = _elasticClient.Search<MessageElasticModel>(s => s
                           .Aggregations(a => a
                           .DateRange("date_range", dateR => dateR
                           .Field(f => f.ReceivedAt)
                           .Format("YYYY-MM-DD")
                           .Ranges(
                               r => r.From("2016-10-01").To("2016-10-19")
                            )
                            .Aggregations(a2 => a2
                            .SignificantTerms("signifint_terms", st => st
                            .Field(f2 => f2.Provider.Type))))));

                var agg = response.Aggs.DateRange("date_range").Items;


                var items = agg.Select(i => new MessageCountEntity
                {
                    total_message = i.DocCount,
                    message_states = i.SignificantTerms("signifint_terms").Items
                                    .Select(st => new range_buckets
                                    {
                                        key = st.Key,
                                        messages = st.DocCount
                                    })

                });

                return items;
            }
            
        }
    }
}
