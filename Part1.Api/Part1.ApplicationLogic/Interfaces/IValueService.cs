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
        IEnumerable<MessageCountEntity> GetMessageStat(string fromDate, string toDate);
        IEnumerable<MessageCountEntity> GetMessageStats(String interval, string startDate, string endDate, String goBackBy, string providerType);
        IEnumerable<UniqueUsersCountEntity> GetMessagesUniqueUsers(string userType, string startDate, string interval);
        IEnumerable<tenantsEntity> GetByTenant(string startingPoint);
    }
}