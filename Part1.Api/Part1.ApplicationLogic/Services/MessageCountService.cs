using Part1.ApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Part1.ApplicationLogic.Entities;
using Nest;
using Part1.Data.EsModels;
using Part1.ApplicationLogic.ServiceHelper;
using System.Net;

namespace Part1.ApplicationLogic.Services
{
    public class MessageCountService : ICountMessages
    {
        private readonly IElasticClient _elasticClient;
        ServiceHelperClass service_helper = new ServiceHelperClass();

        public MessageCountService(IElasticClient client)
        {
            _elasticClient = client;
            service_helper._elasticClient = _elasticClient;
        }

        public IEnumerable<MessageStatsEntity> GetMessageStat(string fromDate = "now-24H/H", string toDate = "now")
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

            var items = agg.Select(i => new MessageStatsEntity
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

        public IEnumerable<MessageCountEntity> GetMessageStats(string interval, string startDate, string endDate, string goBackBy, string mustNot, string providerType /* goBackBy>> This variable will contain time(period) that the aggrigation will have to start from */)
        {
            //try
            //{

            
            
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
                    int day = System.DateTime.Now.Day;
                    String ds = "0";;
                    if (day < 10) {
                        ds = "0" + day;
                    }
                    else
                    {
                        ds = day + "";
                    }

                    endDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + ds;
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
                if (providerType.Length < 0 || providerType.ToLower().Equals("all"))
                {
                    providerType = "";
                }
                if (endDate.Length<1 && startDate.Length<1)
                {
                    endDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day;
                    startDate = "now-" + goBackBy;
                }
            }

            interval = interval.ToLower();
            var response = _elasticClient.Search<MessageElasticModel>(s => s);
            if ((mustNot.Split(',').Length > 0 || providerType.Split(',').Length > 1) && (!(providerType.ToLower().Contains("") && mustNot.Length==0)))
            {
                if (mustNot.Length > 0 || providerType.Split(',').Length > 1)
                {


                    var results = service_helper.Mult_MessageType(interval, startDate, endDate, goBackBy, mustNot, providerType);
                    var aggss = results.Aggs.DateHistogram("date_histogram").Items;
                    var termss = aggss.Select(s => new MessageCountEntity
                    {
                        total_message = s.DocCount,
                        date = s.Date,
                        message_states = s.Terms("significant_terms").Items
                                         .Select(ss => new range_buckets
                                         {
                                             key = ss.Key,
                                             messages = ss.DocCount
                                         })
                    });
                    return termss;
                }
            }
            if (providerType.Length < 0)
            {
                    response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                  .Query(q => q.Range(r => r.OnField(of => of.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                  .Size(0)
                                                                  .Aggregations(a => a.DateHistogram("date_histogram", dh => dh
                                                                                                                      .Field(f => f.ReceivedAt)
                                                                                                                      .Interval(interval)
                                                                                                                      .Aggregations(a2 => a2.Terms("significant_terms", st => st
                                                                                                                                                                                   .Field(f => f.Provider.Type)))))
                                                                 );
            }
            else////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///////With specified provider type///
            {
                if(providerType.Length < 1)
                {
                    response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                  .Query(q => q.Range(r => r.OnField(of => of.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                  .Size(0)
                                                                  .Aggregations(a => a.DateHistogram("date_histogram", dh => dh
                                                                                                                      .Field(f => f.ReceivedAt)
                                                                                                                      .Interval(interval)
                                                                                                                      .Aggregations(a2 => a2.Terms("significant_terms", st => st
                                                                                                                                                                                   .Field(f => f.Provider.Type)))))
                                                                 );
                }
                else
                {
                    response = _elasticClient.Search<MessageElasticModel>(s => s
                           .Query(q => q
                           .Bool(b => b
                           .Must(
                               m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)),
                               m => m.Match(d => d.OnField("provider.type").Query(providerType.ToLower())))
                               ))
                          .Size(0)
                          .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt)
                                                                                    .Interval(interval)
                                             .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
                }
                    

            }

            var aggs = response.Aggs.DateHistogram("date_histogram").Items;
                var terms = aggs.Select(s => new MessageCountEntity
                {
                    total_message = s.DocCount,
                    date = s.Date,
                    message_states = s.Terms("significant_terms").Items
                                     .Select(ss => new range_buckets
                                     {
                                         key = ss.Key,
                                         messages = ss.DocCount
                                     })
                });
                return terms;
            //}
            //catch (Exception dc)
            //{
            //    throw dc;
            //}
        }
        //Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko////Ntobeko//
        public IEnumerable<UniqueUsersCountEntity> GetMessagesUniqueUsers(string userType, string excludeUserType, string startDate,string endDate, string interval)
        {
            userType = userType.ToLower();
            if (startDate.Equals(""))
            {
                startDate = DateTime.Now.Year.ToString() + "-01-01";
            }
            if(endDate.Equals(""))
            {
                string day = "";
                if(DateTime.Now.Day<10)
                {
                    day = "0" + DateTime.Now.Day;
                }
                else
                {
                    day = DateTime.Now.Day.ToString();
                }
                endDate =DateTime.Now.Year+"-"+DateTime.Now.Month+"-"+day;
            }
            if (interval.Equals(""))
            {
                interval = "week";
            }
            var response = _elasticClient.Search<MessageElasticModel>(s => s);

            if(userType.Contains(","))
            {
                response = service_helper.Multiple_UniqueUsers(userType, excludeUserType, startDate, endDate, interval);

                var aggss = response.Aggs.DateHistogram("dateHistogram").Items;
                var itemss = aggss.Select(i => new UniqueUsersCountEntity
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
                return itemss;
            }
            if(userType.Equals("all"))
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                        .Query(q => q.Range(r => r.OnField(m => m.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
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
            }
            else
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                .Query(q => q
                                                                       .Bool(b => b
                                                                             .Must(
                                                                                m => m.Range(r => r.OnField(f => f.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)),
                                                                                m => m.Match(r => r.OnField(f => f.Provider.Type).Query(userType.ToLower())))))
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
            }

            


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
