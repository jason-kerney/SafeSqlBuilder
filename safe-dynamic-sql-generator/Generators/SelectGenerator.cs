using System;
using System.Collections.Generic;
using SafeSqlBuilder.Helpers;

namespace SafeSqlBuilder.Generators
{
    public class SelectGenerator
    {
        private readonly IEnumerable<string> _fields;
        private readonly QueryData _queryData;
        public SelectGenerator(QueryData queryData)
        {
            _queryData = queryData;
            _fields = _queryData?.GetColumns();
        }

        public override string ToString()
        {
            if (_queryData.IsNull() || _queryData.IsInvalid) throw new ArgumentException("The provided query specification is invalid.");
            
            return $"SELECT {string.Join(", ", _fields)} FROM {_queryData.TableName}";
        }
    }
}