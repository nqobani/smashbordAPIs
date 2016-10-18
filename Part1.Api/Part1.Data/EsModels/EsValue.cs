using Nest;
using System;

namespace Part1.Data.EsModels
{
    [ElasticType(IdProperty = "id", Name = "esvalue")]
    public class EsValue
    {
        [ElasticProperty(Name = "_id", Index = FieldIndexOption.NotAnalyzed, Type = FieldType.String)]
        public Guid Id { get; set; }

        [ElasticProperty(Name = "value", Index = FieldIndexOption.Analyzed, Type = FieldType.Integer)]
        public int Value { get; set; }
    }
}