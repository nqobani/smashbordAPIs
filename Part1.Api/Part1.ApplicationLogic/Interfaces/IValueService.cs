using Part1.ApplicationLogic.Entities;
using System;
using System.Collections.Generic;

namespace Part1.ApplicationLogic.Interfaces
{
    public interface IValueService
    {
        IEnumerable<string> GetValues();

        int GetValue(string id);

        string AddValue(int value);
    }

    public interface ICountMessages
    {
        IEnumerable<CountAllMessagesEntity> GetMessageStats();
        IEnumerable<MessageCountEntity> GetMessageStat(string fromDate, string toDate);
        IEnumerable<MessageCountEntity> GetMessageStats(string interval, string goBackBy);
    }

}