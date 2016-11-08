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
                    if (day < 20) {
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
                if(endDate.Length<1 && startDate.Length<1)
                {
                    endDate = System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day;
                    startDate = "now-" + goBackBy;
                }
            }

            interval = interval.ToLower();
            var response = _elasticClient.Search<MessageElasticModel>(s => s);
            //if((mustNot.Split(',').Length>0|| providerType.Split(',').Length > 1)&& !providerType.ToLower().Contains("all"))
            //{
            //    if(mustNot.Length>0 || providerType.Split(',').Length > 1)
            //    {

                
            //    var results = Mult_MessageType(interval, startDate, endDate, goBackBy, mustNot, providerType);
            //    var aggss = results.Aggs.DateHistogram("date_histogram").Items;
            //    var termss = aggss.Select(s => new MessageCountEntity
            //    {
            //        total_message = s.DocCount,
            //        date = s.Date,
            //        message_states = s.Terms("significant_terms").Items
            //                         .Select(ss => new range_buckets
            //                         {
            //                             key = ss.Key,
            //                             messages = ss.DocCount
            //                         })
            //    });
            //    return termss;}
            //}
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
            var response = _elasticClient.Search<MessageElasticModel>(s => s);
            if(userType.Length <1 || userType.Equals("all"))
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
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
            }
            else
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                .Query(q => q
                                                                       .Bool(b => b
                                                                             .Must(
                                                                                m => m.Range(r => r.OnField(f => f.ReceivedAt).GreaterOrEquals(startDate)),
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

        public ISearchResponse<MessageElasticModel> Mult_MessageType(string interval, string startDate, string endDate, string goBackBy, string mustNot, string providerType)
        {
            int[] provider = new int[4];
            string[] providerTypes = { "facebook", "chat", "sms", "twitter" };
            string[] toMatch = providerType.Split(',');
            string[] notToMath = mustNot.Split(',');
            if (mustNot.Length > 0 && toMatch.Length<1)
            {
                int countNotToMatch = notToMath.Length;
                toMatch = new string[4-countNotToMatch];

                        if (mustNot.Contains("facebook"))
                        {
                            provider[0] = 0;
                        }
                        else
                        {
                            provider[0] = 1;
                        }
                        if (mustNot.Contains("chat"))
                        {
                            provider[1] = 0;
                        }
                        else
                        {
                            provider[1] = 1;
                        }
                        if (mustNot.Contains("sms"))
                        {
                            provider[2] = 0;
                        }
                        else
                        {
                            provider[2] = 1;
                        }
                        if (mustNot.Contains("twitter"))
                        {
                            provider[3] = 0;
                        }
                        else
                        {
                            provider[3] = 1;
                        }
                int inner_Count = 0;
                for (int i = 0; i < provider.Length; i++)
                {
                    if (provider[i] == 1)
                    {
                        toMatch[inner_Count] = providerTypes[i];
                        inner_Count++;
                    }
                }
            }

            int count = 0;
            if (toMatch.Contains("facebook"))
            {
                provider[0] = 1;
                count++;
            }
            else
            {
                provider[0] = 0;
            }
            if(toMatch.Contains("chat"))
            {
                provider[1] = 1;
                count++;
            }
            else
            {
                provider[1] = 0;
            }
            if(toMatch.Contains("sms"))
            {
                provider[2] = 1;
                count++;
            }
            else
            {
                provider[2] = 0;
            }
            if(toMatch.Contains("twitter"))
            {
                provider[3] = 1;
                count++;
            }
            else
            {
                provider[3] = 0;
            }
            string[] not_to_match = new string[4 - count];
            int innerCount = 0;
            for (int i = 0;i<provider.Length;i++)
            {
                if(provider[i] == 0)
                {
                    not_to_match[innerCount] = providerTypes[i];
                    innerCount++;
                }
            }

            var response = _elasticClient.Search<MessageElasticModel>(s => s);

            if (toMatch.Length == 1)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                           .Query(q => q
                           .Bool(b => b
                           .Must(
                               m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate))
                               )
                            .Should(
                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[0]))
                              )
                              .MustNot(
                               m => m.Match(d => d.OnField("provider.type").Query(not_to_match[0])),
                               m => m.Match(d => d.OnField("provider.type").Query(not_to_match[1])),
                               m => m.Match(d => d.OnField("provider.type").Query(not_to_match[2]))
                               )
                               ))
                               .Size(0)
                               .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt)
                                                                                    .Interval(interval)
                                             .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            else if (toMatch.Length == 2)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                           .Query(q => q
                           .Bool(b => b
                           .Must(
                               m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate))
                               )
                            .Should(
                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[0])),
                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[1]))
                              )
                              .MustNot(
                               m => m.Match(d => d.OnField("provider.type").Query(not_to_match[0])),
                               m => m.Match(d => d.OnField("provider.type").Query(not_to_match[1]))
                               )
                               ))
                               .Size(0)
                               .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt)
                                                                                    .Interval(interval)
                                             .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            else if(toMatch.Length == 3)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                           .Query(q => q
                           .Bool(b => b
                           .Must(
                               m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate))
                               )
                            .Should(
                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[0])),
                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[1])),
                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[2]))
                              )
                              .MustNot(
                               m => m.Match(d => d.OnField("provider.type").Query(not_to_match[0]))
                               )
                               ))
                               .Size(0)
                               .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt)
                                                                                    .Interval(interval)
                                             .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            else{
                response = _elasticClient.Search<MessageElasticModel>(s => s
                           .Query(q => q
                           .Bool(b => b
                           .Must(
                               m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate))
                               )
                               ))
                               .Size(0)
                               .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt)
                                                                                    .Interval(interval)
                                             .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            return response;
        }

        public IEnumerable<tenantsEntity> GetByTenant(string startingPoint)
        {
            if (startingPoint.Equals(""))
            {
                startingPoint = DateTime.Now.Year.ToString() + "-01-01";
            }

            var result = _elasticClient.Search<MessageElasticModel>(s => s
                           .Query(z => z.Range(r => r.OnField(m => m.ReceivedAt).GreaterOrEquals(startingPoint)))
                           .Size(0)
                           .Aggregations(na => na
                                    .Terms("tenant", st => st
                                        .Field(o => o.TenantId))));

            var TenantCount = result.Aggs.Terms("tenant").Items;
            var items = TenantCount.Select(i => new tenantsEntity
            {
                key = i.Key + "",
                docCount = i.DocCount

            });
            return items;
        }
    }
}
