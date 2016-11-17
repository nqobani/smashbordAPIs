using Nest;
using Part1.Data.EsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Part1.ApplicationLogic.ServiceHelper
{
    class ServiceHelperClass
    {
        public IElasticClient _elasticClient { set; get; }

        public ISearchResponse<MessageElasticModel> Mult_MessageType(string interval, string startDate, string endDate, string goBackBy, string mustNot, string providerType)
        {
            int[] provider = new int[4];
            string[] providerTypes = { "facebook", "chat", "sms", "twitter" };
            string[] toMatch = providerType.Split(',');
            string[] notToMath = mustNot.Split(',');
            if (mustNot.Length > 0 && toMatch.Length < 2)
            {
                int countNotToMatch = notToMath.Length;
                toMatch = new string[4 - countNotToMatch];

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
            if (toMatch.Contains("chat"))
            {
                provider[1] = 1;
                count++;
            }
            else
            {
                provider[1] = 0;
            }
            if (toMatch.Contains("sms"))
            {
                provider[2] = 1;
                count++;
            }
            else
            {
                provider[2] = 0;
            }
            if (toMatch.Contains("twitter"))
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
            for (int i = 0; i < provider.Length; i++)
            {
                if (provider[i] == 0)
                {
                    not_to_match[innerCount] = providerTypes[i];
                    innerCount++;
                }
            }

            ISearchResponse<MessageElasticModel> response;

            if (toMatch.Length == 1)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                      .Query(q => q
                                                                                .Bool(b => b
                                                                                        .Must(m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                                        .Should(m => m.Match(d => d.OnField("provider.type").Query(toMatch[0])))
                                                                                         .MustNot(m => m.Match(d => d.OnField("provider.type").Query(not_to_match[0])),
                                                                                                  m => m.Match(d => d.OnField("provider.type").Query(not_to_match[1])),
                                                                                                  m => m.Match(d => d.OnField("provider.type").Query(not_to_match[2])))))
                                                                        .Size(0)
                                                                         .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt).Interval(interval)
                                                                                                                                            .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            else if (toMatch.Length == 2)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                       .Query(q => q
                                                                                .Bool(b => b
                                                                                       .Must(m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                                       .Should(m => m.Match(d => d.OnField("provider.type").Query(toMatch[0])),
                                                                                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[1])))
                                                                                       .MustNot(m => m.Match(d => d.OnField("provider.type").Query(not_to_match[0])),
                                                                                                m => m.Match(d => d.OnField("provider.type").Query(not_to_match[1])))))
                                                                        .Size(0)
                                                                        .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt).Interval(interval)
                                                                                                                                          .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            else if (toMatch.Length == 3)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                       .Query(q => q
                                                                               .Bool(b => b
                                                                                       .Must(m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                                       .Should(m => m.Match(d => d.OnField("provider.type").Query(toMatch[0])),
                                                                                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[1])),
                                                                                               m => m.Match(d => d.OnField("provider.type").Query(toMatch[2])))
                                                                                       .MustNot(m => m.Match(d => d.OnField("provider.type").Query(not_to_match[0])))))
                                                                       .Size(0)
                                                                       .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt).Interval(interval)
                                                                                                                                           .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            else
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                         .Query(q => q
                                                                                .Bool(b => b
                                                                                        .Must(m => m.Range(r => r.OnField(n => n.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))))
                                                                           .Size(0)
                                                                           .Aggregations(a => a.DateHistogram("date_histogram", h => h.Field(m => m.ReceivedAt).Interval(interval)
                                                                                                                                        .Aggregations(v => v.Terms("significant_terms", p => p.Field(x => x.Provider.Type))))));
            }
            return response;
        }

        public ISearchResponse<MessageElasticModel> Multiple_UniqueUsers(string userType, string excludeUserType, string startDate, string endDate, string interval)
        {

            int[] provider = new int[4];
            string[] providerTypes = { "facebook", "chat", "sms", "twitter" };
            string[] toMatch = userType.Split(',');
            string[] notToMath = excludeUserType.Split(',');
            if (excludeUserType.Length > 0 && toMatch.Length < 2)
            {
                int countNotToMatch = notToMath.Length;
                toMatch = new string[4 - countNotToMatch];

                if (excludeUserType.Contains("facebook"))
                {
                    provider[0] = 0;
                }
                else
                {
                    provider[0] = 1;
                }
                if (excludeUserType.Contains("chat"))
                {
                    provider[1] = 0;
                }
                else
                {
                    provider[1] = 1;
                }
                if (excludeUserType.Contains("sms"))
                {
                    provider[2] = 0;
                }
                else
                {
                    provider[2] = 1;
                }
                if (excludeUserType.Contains("twitter"))
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
            if (toMatch.Contains("chat"))
            {
                provider[1] = 1;
                count++;
            }
            else
            {
                provider[1] = 0;
            }
            if (toMatch.Contains("sms"))
            {
                provider[2] = 1;
                count++;
            }
            else
            {
                provider[2] = 0;
            }
            if (toMatch.Contains("twitter"))
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
            for (int i = 0; i < provider.Length; i++)
            {
                if (provider[i] == 0)
                {
                    not_to_match[innerCount] = providerTypes[i];
                    innerCount++;
                }
            }

            ISearchResponse<MessageElasticModel> response= _elasticClient.Search<MessageElasticModel>(s => s);

            if (toMatch.Length == 1)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                                                    .Query(q => q
                                                                                                            .Bool(b => b
                                                                                                                   .Must(m => m.Range(r => r.OnField(f => f.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                                                                   .Should(
                                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[0])))
                                                                                                                  .MustNot(
                                                                                                                        m => m.Match(r => r.OnField(f => f.Provider.Type).Query(not_to_match[0])),
                                                                                                                        m => m.Match(r => r.OnField(f => f.Provider.Type).Query(not_to_match[1])),
                                                                                                                        m => m.Match(r => r.OnField(f => f.Provider.Type).Query(not_to_match[2])))
                                                                                                                   ))
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
            else if (toMatch.Length == 2)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                                                    .Query(q => q
                                                                                                            .Bool(b => b
                                                                                                                   .Must(m => m.Range(r => r.OnField(f => f.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                                                                   .Should(
                                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[0])),
                                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[1])))
                                                                                                                   .MustNot(
                                                                                                                        m => m.Match(r => r.OnField(f => f.Provider.Type).Query(not_to_match[0])),
                                                                                                                        m => m.Match(r => r.OnField(f => f.Provider.Type).Query(not_to_match[1])))
                                                                                                                   ))
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
            else if (toMatch.Length == 3)
            {
                response = _elasticClient.Search<MessageElasticModel>(s => s
                                                                                    .Query(q => q
                                                                                            .Bool(b => b
                                                                                                   .Must(m => m.Range(r => r.OnField(f => f.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                                                   .Should(
                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[0])),
                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[1])),
                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[2])))
                                                                                                  .MustNot(
                                                                                                        m => m.Match(r => r.OnField(f => f.Provider.Type).Query(not_to_match[0])))
                                                                                                   ))
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
                                                                                                   .Must(m => m.Range(r => r.OnField(f => f.ReceivedAt).GreaterOrEquals(startDate).LowerOrEquals(endDate)))
                                                                                                   .Should(
                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[0])),
                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[1])),
                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(toMatch[2])),
                                                                                                       m => m.Match(r => r.OnField(f => f.Provider.Type).Query(not_to_match[3])))
                                                                                                   ))
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

                
            return response;
        }

    }
}
