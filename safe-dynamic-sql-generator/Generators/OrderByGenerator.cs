namespace SafeSqlBuilder.Generators
{
    public class OrderByGenerator
    {
        private readonly QueryData _queryData;
        public OrderByGenerator(QueryData queryData)
        {
            _queryData = queryData;
        }

        public override string ToString()
        {
            var column = _queryData.GetSortColumn();
            var direction = _queryData.GetSortDirection();

            if (string.IsNullOrWhiteSpace(column))
            {
                return string.Empty;
            }

            return direction == "ASC" 
                ? $"ORDER BY CASE WHEN {column} IS NULL THEN 1 ELSE 0 END, {column}" 
                : $"ORDER BY {column} DESC";
        }
    }
}