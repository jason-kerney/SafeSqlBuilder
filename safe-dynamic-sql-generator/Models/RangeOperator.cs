using System.Text.Json.Serialization;

namespace SafeSqlBuilder.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RangeOperator
    {
        GreaterThan,
        LessThan,
        GreaterThanEqual,
        LessThanEqual,
        Between,
    }
}