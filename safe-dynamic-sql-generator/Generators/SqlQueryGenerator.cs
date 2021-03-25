using System;
using SafeSqlBuilder.Helpers;

namespace SafeSqlBuilder.Generators
{
    public class SqlQueryGenerator
    {
        private readonly SelectGenerator _selectGenerator;
        private readonly WhereGenerator _whereGenerator;
        private readonly GroupByGenerator _groupByGenerator;
        private readonly OrderByGenerator _orderByGenerator;

        public SqlQueryGenerator(
            SelectGenerator selectGenerator,
            WhereGenerator whereGenerator,
            GroupByGenerator groupByGenerator,
            OrderByGenerator orderByGenerator
            )
        {
            _selectGenerator = selectGenerator;
            _whereGenerator = whereGenerator;
            _groupByGenerator = groupByGenerator;
            _orderByGenerator = orderByGenerator;
        }

        public override string ToString()
        {
            if (_selectGenerator.IsNull()) throw new ArgumentException("Query Must Have a Select Part");
            
            var sb = $"{_selectGenerator}";
            sb = GetPartFrom(sb, _whereGenerator);
            sb = GetPartFrom(sb, _groupByGenerator);
            sb = GetPartFrom(sb, _orderByGenerator);
            return sb.Trim();
        }

        private static string GetPartFrom(string sb, object value)
        {
            return value == null ? sb.Trim() : sb.Trim() + $" {value}";
        }
    }
}