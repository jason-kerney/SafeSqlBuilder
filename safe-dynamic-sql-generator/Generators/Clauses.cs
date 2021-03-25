using System.Collections.Generic;

namespace SafeSqlBuilder.Generators
{
    public abstract class Clause
    {
        protected string PropertyName { get; }

        protected Clause(string propertyName)
        {
            PropertyName = propertyName;
        }

        protected abstract string AsString();

        public override string ToString()
        {
            return AsString();
        }

        public abstract IEnumerable<(string, object)> GetParameters();
    }
    
    public abstract class SinglePropertyClause : Clause
    {
        private readonly object _value;
        protected SinglePropertyClause(string propertyName, object value) : base(propertyName)
        {
            _value = value;
        }
        
        protected abstract string GetOperator();

        public abstract string GetSuffixString();

        protected string ParameterName => $"@{PropertyName}_{GetSuffixString()}";

        protected override string AsString()
        {
            return $"[{PropertyName}] {GetOperator()} {ParameterName}";
        }

        public override IEnumerable<(string, object)> GetParameters()
        {
            return new[] {(ParameterName, _value)};
        }
    } 
    
    public class EqualClause : SinglePropertyClause
    {
        public EqualClause(string propertyName, object value) : base(propertyName, value) { }

        protected override string GetOperator() => "=";
        public override string GetSuffixString() => "EQUAL";
    }

    public class InClause : SinglePropertyClause
    {
        public InClause(string propertyName, object value) : base(propertyName, value) { }

        protected override string GetOperator() => "IN";
        public override string GetSuffixString() => GetOperator();
    }

    public class GreaterThanClause : SinglePropertyClause
    {
        public GreaterThanClause(string propertyName, object value) : base(propertyName, value) { }

        protected override string GetOperator() => ">";

        public override string GetSuffixString() => "GREATER";
    }

    public class LessThanClause : SinglePropertyClause
    {
        public LessThanClause(string propertyName, object value) : base(propertyName, value) { }

        protected override string GetOperator() => "<";

        public override string GetSuffixString() => "LESS";
    }

    public class GreaterThanEqualClause : SinglePropertyClause
    {
        public GreaterThanEqualClause(string propertyName, object value) : base(propertyName, value) { }

        protected override string GetOperator() => ">=";

        public override string GetSuffixString() => "GREATER_EQUAL";
    }

    public class LessThanEqualClause : SinglePropertyClause
    {
        public LessThanEqualClause(string propertyName, object value) : base(propertyName, value) { }

        protected override string GetOperator() => "<=";

        public override string GetSuffixString() => "LESS_EQUAL";
    }

    public class BetweenClause : Clause
    {
        private readonly object _startValue;
        private readonly object _endValue;

        public BetweenClause(string propertyName, object startValue, object endValue) : base(propertyName)
        {
            _startValue = startValue;
            _endValue = endValue;
        }

        private string StartParameterName => $"@{PropertyName}_START";
        private string EndParameterName => $"@{PropertyName}_END";

        protected override string AsString() => $"[{PropertyName}] BETWEEN {StartParameterName} AND {EndParameterName}";
        public override IEnumerable<(string, object)> GetParameters()
        {
            return new[] {(StartParameterName, _startValue), (EndParameterName, _endValue)};
        }
    }

    public class NullClause : Clause
    {
        public NullClause(string propertyName) : base(propertyName) { }

        protected override string AsString() => $"[{PropertyName}] IS NULL";
        public override IEnumerable<(string, object)> GetParameters()
        {
            return new (string, object)[0];
        }
    }

    public class NotNullClause : Clause
    {
        public NotNullClause(string propertyName) : base(propertyName) { }

        protected override string AsString() => $"[{PropertyName}] IS NOT NULL";
        public override IEnumerable<(string, object)> GetParameters()
        {
            return new (string, object)[0];
        }
    }
}