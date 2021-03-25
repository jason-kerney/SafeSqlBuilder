using System;
using System.Collections.Generic;
using System.Linq;
using SafeSqlBuilder;
using SafeSqlBuilder.Generators;
using SafeSqlBuilder.Helpers;
using SafeSqlBuilder.Models;
using Moq;
// ReSharper disable PossibleMultipleEnumeration

namespace SafeSqlBuilder.Tests
{
    public static class Builder
    {
        public static (Query, IValidColumns, ClauseFactory) InitializeBasicItemsForQueryData(params string[] fields)
        {
            return InitializeItemsForQueryData(fields);
        }

        public static (Query, IValidColumns, ClauseFactory) InitializeItemsForQueryData(string[] fields, Filter[] filters = null, string[] groupBy = null, Func<Filter, IEnumerable<Clause>> clauseBuilder = null, bool columnsAreInvalid = false)
        {
            var (q, v, c) = InitializeFakeItemsForQueryData(fields, filters, groupBy, clauseBuilder, _ => columnsAreInvalid);
            return (q, v.Object, c.Object);
        }

        public static (Query, Mock<IValidColumns>, Mock<ClauseFactory>) InitializeFakeItemsForQueryData(string[] fields, Filter[] filters = null, string[] groupBy = null, Func<Filter, IEnumerable<Clause>> clauseBuilder = null, Func<string, bool> columnIsInvalid = null)
        {
            var fakeValidColumns = BuildFakeValidColumns(columnIsInvalid);
            
            var validColumns = fakeValidColumns.Object;
            var fakeClauseFactory = BuildFakeClauseFactory(validColumns, clauseBuilder);

            var query = BuildQuery(fields, filters, groupBy);

            return (query, fakeValidColumns, fakeClauseFactory);
        }

        public static Query BuildQuery(string[] fields, Filter[] filters = null, string[] groupBy = null, bool? distinct = null, string direction = null, string sort = null)
        {
            return new Query
            {
                Fields = fields,
                Filters = filters,
                GroupBy = groupBy,
                Distinct = distinct,
                Direction = direction,
                Sort = sort,
            };
        }

        public static Query AddGroupBy(this Query query, params string[] fields)
        {
            query.GroupBy = fields;
            return query;
        }

        public static Query AddFilters(this Query query, params Filter[] filters)
        {
            if (query.Filters == null)
            {
                query.Filters = filters;
            }
            else
            {
                var f = new List<Filter>(query.Filters);    
                f.AddRange(filters);

                query.Filters = f.ToArray();
            }

            return query;
        }

        public static Query AddFilter(this Query query, string property = null, object[] values = null, FilterRange range = null, bool? includeNulls = null)
        {
            var filter = new Filter
            {
                Property = property,
                includeNulls = includeNulls,
                Range = range,
                Values = values
            };

            if (query.Filters == null)
            {
                query.Filters = new[] {filter};
            }
            else
            {
                var filters = new List<Filter>(query.Filters) {filter};

                query.Filters = filters.ToArray();
            }

            return query;
        }

        public static Filter SetRange(this Filter filter, object start = null, object end = null, RangeOperator rangeOperator = (RangeOperator)0)
        {
            filter ??= new Filter();
            
            var range = new FilterRange
            {
                Start = start,
                End = end,
                RangeOperator = rangeOperator,
            };

            filter.Range = range;

            return filter;
        }

        public static Mock<ClauseFactory> BuildFakeClauseFactory(IValidColumns validColumns = null, Func<Filter, IEnumerable<Clause>> clauseBuilder = null)
        {
            var columnValidator =
                validColumns ?? BuildValidColumns();
            
            var fakeClauseFactory = new Mock<ClauseFactory>(columnValidator);

            if (clauseBuilder != null)
            {
                fakeClauseFactory
                    .Setup(factory => factory.Build(It.IsAny<Filter>()))
                    .Returns(clauseBuilder);
            }

            return fakeClauseFactory;
        }

        public static ClauseFactory BuildClauseFactory(IValidColumns validColumns = null, Func<Filter, IEnumerable<Clause>> clauseBuilder = null)
        {
            return BuildFakeClauseFactory(validColumns, clauseBuilder).Object;
        }

        public static Mock<IValidColumns> BuildFakeValidColumns(Func<string, bool> columnIsInvalid = null)
        {
            var fakeValidColumns = new Mock<IValidColumns>();
            fakeValidColumns
                .Setup(columns => columns.GetColumn(It.IsAny<string>()))
                .Returns<string>(Clean);

            var validator =
                columnIsInvalid ?? NoInvalid;

            fakeValidColumns
                .Setup(columns => columns.ContainsInvalidColumns(It.IsAny<IEnumerable<string>>()))
                .Returns<IEnumerable<string>>(fields => fields.Empty() || fields.All(validator));

            fakeValidColumns
                .Setup(columns => columns.ContainsOnlyValidColumns(It.IsAny<IEnumerable<string>>()))
                .Returns<IEnumerable<string>>(fields => fields.Empty() || fields.All(Inverse(validator)));

            fakeValidColumns
                .Setup(columns => columns.IsInvalidColumn(It.IsAny<string>()))
                .Returns(validator);
            
            return fakeValidColumns;
        }

        public static IValidColumns BuildValidColumns(Func<string, bool> columnIsInvalid = null)
        {
            return BuildFakeValidColumns(columnIsInvalid).Object;
        }

        private static Func<string, bool> Inverse(Func<string, bool> validator)
        {
            return field => !validator(field);
        }

        public static string Clean(string value) => $"{value}_Cleaned";
        public static bool NoInvalid(string _) => false;
        public static bool AllInvalid(string _) => true;
        public static Func<string, bool> BuildValidator(string expected) => field => field != expected;

        public static T[] Pack<T>(params T[] items) => items;
        public static T[][] DoublePack<T>(params T[] items) => new[] {items};

        public static QueryData BuildQueryData(Query query, IValidColumns validColumns = null)
        {
            var columns = 
                validColumns
                ?? BuildValidColumns();
            
            return new QueryData(
                query,
                columns,
                BuildClauseFactory(columns),
                "[MySchema].[MyTable]"
            );
        }

        public static QueryData BuildQueryData(string[] fields, Filter[] filters = null, string[] groupBy = null, bool? distinct = null, string direction = null, string sort = null, IValidColumns validColumns = null)
        {
            var query = BuildQuery(fields, filters, groupBy, distinct, direction, sort);
            return BuildQueryData(query, validColumns);
        }

        public static QueryData EmptyFakeQueryData => GetFakeQueryData().Object;

        public static QueryData GetQueryData(IEnumerable<IEnumerable<Clause>> clauses = null) => GetFakeQueryData(clauses).Object; 
        public static Mock<QueryData> GetFakeQueryData(IEnumerable<IEnumerable<Clause>> clauses = null)
        {
            var doubleQueryData = new Mock<QueryData>(null, null, null, null);

            if (clauses.IsNotNull())
            {
                doubleQueryData
                    .SetupGet(data => data.Clauses)
                    .Returns(clauses);
            }

            return doubleQueryData;
        }
    }
}