using System;
using System.Collections.Generic;
using System.Linq;
using SafeSqlBuilder.Helpers;
using SafeSqlBuilder.Models;

namespace SafeSqlBuilder.Generators
{
    public class ClauseFactory
    {
        private readonly IValidColumns _validColumns;

        public ClauseFactory(IValidColumns validColumns)
        {
            _validColumns = validColumns;
        }
        
        public virtual IEnumerable<Clause> Build(Filter filter)
        {
            var propertyName = _validColumns.GetColumn(filter.Property);
            var clauses = new List<Clause>();
            
            if (filter.Values.IsNotNull() && filter.Values.Any())
            {
                // ReSharper disable once PossibleInvalidOperationException
                if (filter.includeNulls.IsNotNull() && !filter.includeNulls.Value)
                {
                    throw new ArgumentException("When providing values in a filter, you cannot also set include nulls to false. This negates the filter entirely.");
                }
                clauses.Add(GetValuesClause(propertyName, filter));
            }

            if (filter.Range.IsNotNull())
            {
                clauses.Add(GetRangeClause(filter, propertyName));
            }

            if (filter.includeNulls.GetValueOrDefault())
            {
                clauses.Add(new NullClause(propertyName));
            }
            else if (filter.includeNulls.IsNotNull())
            {
                clauses.Add(new NotNullClause(propertyName));
            }
            
            return clauses;
        }

        private static Clause GetRangeClause(Filter filter, string propertyName)
        {
            Clause rangeClause = filter.Range.RangeOperator switch
            {
                RangeOperator.GreaterThan => new GreaterThanClause(propertyName, GetSingleValueFromRange(filter.Range)),
                RangeOperator.GreaterThanEqual => new GreaterThanEqualClause(propertyName, GetSingleValueFromRange(filter.Range)),
                RangeOperator.LessThanEqual => new LessThanEqualClause(propertyName, GetSingleValueFromRange(filter.Range)),
                RangeOperator.Between => new BetweenClause(propertyName, filter.Range.Start, filter.Range.End),
                _ => new LessThanClause(propertyName, GetSingleValueFromRange(filter.Range))
            };
            return rangeClause;
        }

        private static object GetSingleValueFromRange(FilterRange filterRange)
        {
            return filterRange
                .Start.IsNull()
                ? filterRange.End
                : filterRange.Start;
        }

        private static Clause GetValuesClause(string propertyName, Filter filter)
        {
            if (filter.Values.Length == 1)
            {
                return new EqualClause(propertyName, filter.Values.First());
            }

            return new InClause(propertyName, filter.Values);
        }
    }
}