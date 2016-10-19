using Part1.ApplicationLogic.Entities;
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
        //IEnumerable<MessageCountEntity> GetMessageStats();
        IEnumerable<MessageCountEntity> GetMessageStats(string fromDate, string toDate);
    }

}