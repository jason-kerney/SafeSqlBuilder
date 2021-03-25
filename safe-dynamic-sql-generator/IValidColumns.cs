using System.Collections.Generic;

namespace SafeSqlBuilder
{
    public interface IValidColumns
    {
        bool ContainsInvalidColumns(IEnumerable<string> fields);
        bool ContainsOnlyValidColumns(IEnumerable<string> fields);
        string GetColumn(string field);
        bool IsInvalidColumn(string field);
        IEnumerable<string> GetColumns();
    }
}