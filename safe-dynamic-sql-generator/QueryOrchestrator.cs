using System.Collections.Generic;
using SafeSqlBuilder.Generators;
using SafeSqlBuilder.Models;

namespace SafeSqlBuilder
{
    public class QueryOrchestrator
    {
        public virtual (string, IEnumerable<(string, object)> parameter) BuildQuery(Query query, IValidColumns validColumns, string tableName)
        {
            var queryData = new QueryData(query, validColumns, new ClauseFactory(validColumns), tableName);
            var whereGenerator = new WhereGenerator(queryData);
            var groupByGenerator = new GroupByGenerator(queryData);
            var orderByGenerator = new OrderByGenerator(queryData);
            var parameter = whereGenerator.GetParameters();

            return (
                new SqlQueryGenerator(
                    new SelectGenerator(queryData),
                    whereGenerator,
                    groupByGenerator,
                    orderByGenerator
                ).ToString()
                , parameter);
        }
    }
}