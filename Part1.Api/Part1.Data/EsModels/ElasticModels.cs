using Nest;
using System;
using System.Collections.Generic;

namespace Part1.Data.EsModels
{
    [ElasticType(Name = "messageelasticmodel")]
    public class MessageElasticModel
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public string UserId { get; set; }

        public string ExternalLink { get; set; }

        public string Text { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public ISet<string> Tags { get; set; }

        public SourceElasticModel Source { get; set; }

        public ProviderElasticModel Provider { get; set; }

        public DateTimeOffset SentAt { get; set; }

        public DateTimeOffset ReceivedAt { get; set; }
    }



    public class ProviderElasticModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class SourceElasticModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string ImageUrl { get; set; }

        [ElasticProperty(Index = FieldIndexOption.NotAnalyzed)]
        public string OriginId { get; set; }

        public bool HasNote { get; set; }
    }
}