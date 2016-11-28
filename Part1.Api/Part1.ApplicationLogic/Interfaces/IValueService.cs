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
        IEnumerable<MessageStatsEntity> GetMessageStat(string fromDate, string toDate);
        IEnumerable<MessageCountEntity> GetMessageStats(string interval, string startDate, string endDate, string goBackBy, string mustNot, string providerType);
        IEnumerable<UniqueUsersCountEntity> GetMessagesUniqueUsers(string userType, string excludeUserType, string startDate,string endDate, string interval);
    }
}