using Nest;
using Part1.ApplicationLogic.Interfaces;
using Part1.Data.EsModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Part1.ApplicationLogic.Services
{
    public class ValueService : IValueService
    {
        private readonly IElasticClient _elasticClient;

        public ValueService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public IEnumerable<string> GetValues()
        {
            return new string[] { "value1", "value2" };
        }

        public string AddValue(int value)
        {
            var valueToAdd = new EsValue
            {
                Id = Guid.NewGuid(),
                Value = value
            };

            //var r = _eFRepository.Add(valueToAdd);
            //
            //await _eFRepository.SaveAsync();

            var res = _elasticClient
                .Index(valueToAdd, v => v
                .Id(valueToAdd.Id.ToString()));

            Debug.WriteLine(res.RequestInformation.Success);

            return valueToAdd.Id.ToString();
        }

        public int GetValue(string id)
        {
            var t = _elasticClient.Get<EsValue>(id);
            return t.Source.Value;
        }
    }
}