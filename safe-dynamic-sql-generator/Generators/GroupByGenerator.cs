using System.Collections.Generic;
using SafeSqlBuilder.Helpers;

namespace SafeSqlBuilder.Generators
{
    public class GroupByGenerator
    {
        private readonly IEnumerable<string> _groupByFields;
        public GroupByGenerator(QueryData queryData)
        {
            _groupByFields = queryData.GetGroupByColumns();
        }

        public override string ToString()
        {
            return _groupByFields.IsNullOrEmpty() 
                ? string.Empty 
                : $"GROUP BY {string.Join(", ", _groupByFields)}";
        }
    }
}
