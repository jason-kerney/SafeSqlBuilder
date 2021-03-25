// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;

namespace SafeSqlBuilder.Models
{
    public class Query
    {
        [JsonPropertyName("fields")]
        public string[] Fields { get; set; }
        [JsonPropertyName("distinct")]
        public bool? Distinct { get; set; }
        [JsonPropertyName("filters")]
        public Filter[] Filters { get; set; }
        [JsonPropertyName("groupBy")]
        public string[] GroupBy { get; set; }
        [JsonPropertyName("sort")]
        public string Sort { get; set; }
        [JsonPropertyName("direction")]
        public string Direction { get; set; }
    }
}
