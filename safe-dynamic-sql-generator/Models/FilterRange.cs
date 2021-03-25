using System.Text.Json.Serialization;
using SafeSqlBuilder.Helpers;
// ReSharper disable InconsistentNaming

namespace SafeSqlBuilder.Models
{
    public class FilterRange : IValidatable
    {
        [JsonPropertyName("start")]
        public object Start { get; set; }
        [JsonPropertyName("end")]
        public object End { get; set; }
        [JsonPropertyName("rangeOperator")]
        public RangeOperator RangeOperator { get; set; }

        public bool IsValid
        {
            get
            {
                switch (RangeOperator)
                {
                    case RangeOperator.GreaterThan:
                    case RangeOperator.LessThan:
                    case RangeOperator.GreaterThanEqual:
                    case RangeOperator.LessThanEqual:
                        return (
                            Start.IsNotNull() && End.IsNull()
                        ) || (
                            Start.IsNull() && End.IsNotNull()
                        );
                    case RangeOperator.Between:
                        return Start.IsNotNull() && End.IsNotNull();
                    default:
                        return false;
                }
            }
        }

        public bool IsInvalid => !IsValid;
    }
}
