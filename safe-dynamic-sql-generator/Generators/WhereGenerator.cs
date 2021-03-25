using System.Collections.Generic;
using System.Linq;

namespace SafeSqlBuilder.Generators
{
    public class WhereGenerator
    {
        private readonly IEnumerable<Clause>[] _clauses;

        public WhereGenerator(QueryData queryData)
        {
            _clauses = queryData.Clauses.Where(c => c.Any()).ToArray();
        }

        public override string ToString()
        {
            if (_clauses.Length == 0)
            {
                return "";
            }

            var (open, close) =
                1 < _clauses.Length
                    ? ("(", ")")
                    : ("", "");
            
            var clauses =
                from clauseGroup in _clauses
                select $"{open}{string.Join(" OR ", clauseGroup)}{close}";

            return $"WHERE {string.Join(" AND ", clauses)}";
        }

        public IEnumerable<(string, object)> GetParameters()
        {
            return _clauses
                .SelectMany(clauseGroup => 
                    clauseGroup
                        .SelectMany(c => c.GetParameters())
                    );
        }
    }
}