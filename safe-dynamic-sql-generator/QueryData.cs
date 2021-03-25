using System.Collections.Generic;
using System.Linq;
using SafeSqlBuilder.Generators;
using SafeSqlBuilder.Helpers;
using SafeSqlBuilder.Models;

namespace SafeSqlBuilder
{
    public class QueryData : IValidatable
    {
        private readonly Query _query;
        private readonly IValidColumns _validColumns;
        private readonly ClauseFactory _clauseFactory;

        public QueryData(Query query, IValidColumns validColumns, ClauseFactory clauseFactory, string tableName)
        {
            _clauseFactory = clauseFactory;
            _query = query;
            _validColumns = validColumns;
            TableName = tableName;
        }

        public string TableName { get; }

        public virtual bool IsInvalid => !IsValid;

        public virtual bool IsValid =>
            _query != null
            && _validColumns.ContainsOnlyValidColumns(_query.Fields)
            && ContainsValidFilters()
            && ContainsValidGroupBy();

        public virtual bool IsDistinctValues => _query.Distinct ?? false;

        public virtual bool IsNotDistinctValues => !IsDistinctValues;

        public virtual IEnumerable<IEnumerable<Clause>> Clauses
        {
            get
            {
                return
                    _query.Filters.IsNotNull()
                        ? _query.Filters.Select(f => _clauseFactory.Build(f))
                        : new IEnumerable<Clause>[0];
            }
        }

        public bool ContainsValidFilters()
        {
            return _query.Filters.IsNull()
                   || _query.Filters.Empty()
                   || (
                       _query.Filters.All(filter => filter.IsValid)
                       && _validColumns.ContainsOnlyValidColumns(_query.Filters.Select(f => f.Property))
                   );
        }

        public bool ContainsValidGroupBy()
        {
            if (_query.GroupBy == null || _query.GroupBy.Empty())
                return true;

            return _validColumns.ContainsOnlyValidColumns(_query.GroupBy)
                   && ((
                           GroupByContainsAllFields()
                           && _query.Distinct == true
                       )
                       || (
                           _query.GroupBy.All(g => _query.Fields.Contains(g))
                           && _query.Distinct.GetValueOrDefault() == false
                       )
                   );
        }

        public bool GroupByContainsAllFields()
        {
            return _query.Fields.OrderBy(f => f).SequenceEqual(_query.GroupBy.OrderBy(g => g));
        }

        public virtual IEnumerable<string> GetColumns()
        {
            return _query.Fields.Select(f => $"[{_validColumns.GetColumn(f)}]");
        }

        public virtual IEnumerable<string> GetGroupByColumns()
        {
            return _query.GroupBy.IsNull() 
                ? new string[0] 
                : _query.GroupBy.Select(f => $"[{_validColumns.GetColumn(f)}]");
        }

        public virtual string GetSortColumn()
        {
            return string.IsNullOrWhiteSpace(_query.Sort) 
                ? string.Empty 
                : $"[{_validColumns.GetColumn(_query.Sort)}]";
        }

        public virtual string GetSortDirection()
        {
            return _query?.Direction?.ToLower() == "desc" ? "DESC" : "ASC";
        }
    }
}