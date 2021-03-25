using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SafeSqlBuilder.Helpers;

namespace SafeSqlBuilder
{
    public class ValidColumns : IValidColumns
    {
        private readonly List<string> _knownColumns;

        public ValidColumns(IEnumerable<string> knownColumns)
        {
            _knownColumns = new List<string>(knownColumns);
        }

        public bool ContainsInvalidColumns(IEnumerable<string> fields)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (fields.IsNull()) return true;
            
            // ReSharper disable once PossibleMultipleEnumeration
            var badFields = GetBadFields(fields);

            return badFields.Any();
        }

        public string GetColumn(string field)
        {
            // This is here to guarantee that we control all column strings.
            if(IsInvalidColumn(field)) return string.Empty;
            
            var index = _knownColumns.IndexOf(field);
            return _knownColumns[index];
        }

        public bool IsInvalidColumn(string field)
        {
            return _knownColumns.All(c => c != field);
        }

        private IEnumerable<string> GetBadFields(IEnumerable<string> fields)
        {
            var set = new HashSet<string>(fields);
            // Removes all good fields
            set.ExceptWith(_knownColumns);
            return set;
        }

        public bool ContainsOnlyValidColumns(IEnumerable<string> fields)
        {
            return !ContainsInvalidColumns(fields);
        }

        public IEnumerable<string> GetColumns()
        {
            return _knownColumns.Select(a => a);
        }

        public static IValidColumns From<T>()
        {
            var columns = 
                typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(property => property.Name);

            return new ValidColumns(columns);
        }
    }
}