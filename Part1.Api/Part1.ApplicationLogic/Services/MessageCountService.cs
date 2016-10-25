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


        public ResultEntity GetMessageStats()
        {
            var response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                         .Aggregations(a => a
                                                                                        .Terms("group_all_by_source_id", t => t
                                                                                                                              .Field(f => f.Source.Id)
                                                                                                                                    .Aggregations(agg => agg
                                                                                                                                                    .Terms("group_all_by_provider_type", tm=> tm
                                                                                                                                                                                          .Field(f => f.Provider.Type))))));

            var aggs = response.Aggs.Terms("group_all_by_source_id").Items;


            var terms = aggs.Select(i => new CountAllMessagesEntity
            {
                user_id = i.Key,
                message_states = i.Terms("group_all_by_provider_type").Items
                .Select(d => new range_buckets {
                    key = d.Key,
                    messages = d.DocCount
                })
            });
            ResultEntity res = new ResultEntity();
            res.users = terms;
            res.total_messages = response.HitsMetaData.Total;

            return res;
        }

        public IEnumerable<MessageCountEntity> GetMessageStat(string fromDate = "now-24H/H", string toDate = "now")
        {
            var response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                        .Aggregations(a => a
                                                                                        .DateRange("date_range", dateR => dateR
                                                                                                                        .Field(f => f.ReceivedAt)
                                                                                                                        .Format("YYYY-MM-DD")
                                                                                                                        .Ranges(r => r.From(fromDate).To(toDate))
                                                                                                                        .Aggregations(a2 => a2
                                                                                                                                        .Terms("signifint_terms", st => st
                                                                                                                                                                 .Field(f2 => f2.Provider.Type)))))
                                                                       );

            var agg = response.Aggs.DateRange("date_range").Items;


            var items = agg.Select(i => new MessageCountEntity
            {
                total_message = i.DocCount,
                message_states = i.Terms("signifint_terms").Items
                                .Select(st => new range_buckets
                                {
                                    key = st.Key,
                                    messages = st.DocCount
                                })

            });

            return items;
        }

        public IEnumerable<MessageCountEntity> GetMessageStats(String interval, string startDate, string endDate, String goBackBy, string providerType /* goBackBy>> This variable will contain time(period) that the aggrigation will have to start from */)
        {

            
            if (goBackBy.Equals(""))
            {
                if (startDate.Length < 1)
                {
                    startDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-01";
                }
                if (providerType.Length < 0 || providerType.ToLower().Equals("all"))
                {
                    providerType = "";
                }
                if (endDate.Length < 1)
                {
                    endDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day;
                }
                
            }
            else
            {
                if (endDate.Length > 0 && startDate.Length > 0)
                {
                    goBackBy = "";
                }
                if(endDate.Length >0 && startDate.Length < 1)
                {
                    startDate = "now-" + goBackBy;
                }
                if(endDate.Length<1 && startDate.Length<1)
                {
                    endDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day;
                    startDate = "now-" + goBackBy;
                }
            }

            interval = interval.ToLower();
            var response = _elasticClient.Search<MessageElasticModel>(s => s);

            if (providerType.Length < 0)
            {
                    response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                  .Query(q => q.Range(r => r.OnField(of => of.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                  .Size(0)
                                                                  .Aggregations(a => a.DateHistogram("date_histogram", dh => dh
                                                                                                                      .Field(f => f.ReceivedAt)
                                                                                                                      .Interval(interval)
                                                                                                                      .Aggregations(a2 => a2.SignificantTerms("significant_terss", st => st
                                                                                                                                                                                   .Field(f => f.Provider.Type)))))
                                                                 );
            }
            else////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///
            {
                    response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                  .Query(q => q.Range(r => r.OnField(of => of.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                  .Size(0)
                                                                  .Aggregations(a => a.DateHistogram("date_histogram", dh => dh
                                                                                                                      .Field(f => f.ReceivedAt)
                                                                                                                      .Interval(interval)
                                                                                                                      .Aggregations(a2 => a2.SignificantTerms("significant_terss", st => st
                                                                                                                                                                                   .Field(f => f.Provider.Type)))))
                                                                 );

            }
            
           



            
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
        //Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko//
        public IEnumerable<UniqueUsersCountEntity> GetMessagesUniqueUsers(string userType, string startDate, string interval)
        {

            if (startDate.Equals(""))
            {
                startDate = DateTime.Now.Year.ToString() + "-01-01";
            }

            if (interval.Equals(""))
            {
                interval = "week";
            }
            var response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                    .Query(q => q.Range(r => r.OnField(m => m.ReceivedAt).GreaterOrEquals(startDate)))
                                                                                 .Size(0)
                                                                                 .Aggregations(a => a.DateHistogram("dateHistogram", b => b
                                                                                                                                   .Field(v => v.ReceivedAt)
                                                                                                                                           .Interval(interval)
                                                                                                                                            .Aggregations(m => m
                                                                                                                                                          .Terms("ProviderTypes", d => d
                                                                                                                                                                                    .Field(f => f.Provider.Type)
                                                                                                                                                                                    .Aggregations(n => n
                                                                                                                                                                                                   .Cardinality("unique_users", c => c.Field(e => e.Source.Name)))))))
                                                                          );

            var agg = response.Aggs.DateHistogram("dateHistogram").Items;
            var items = agg.Select(i => new UniqueUsersCountEntity
            {
                Date = i.Date + "",
                AllDocs = i.DocCount,
                providerTypes = i.Terms("ProviderTypes").Items.
                Select(b => new pTypeEntity
                {
                    providerType = b.Key,
                    DocCount = b.DocCount,
                    number_of_users = b.Cardinality("unique_users").Value + ""

                })
            });
            return items;
        }
    }
}
