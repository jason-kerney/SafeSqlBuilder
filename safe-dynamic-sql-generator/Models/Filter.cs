using System.Linq;
using System.Text.Json.Serialization;
using SafeSqlBuilder.Helpers;
// ReSharper disable InconsistentNaming

namespace SafeSqlBuilder.Models
{
    public class Filter : IValidatable
    {
        [JsonPropertyName("property")]
        public string Property { get; set; }
        [JsonPropertyName("values")]
        public object[] Values { get; set; }
        [JsonPropertyName("range")]
        public FilterRange Range { get; set; }
        [JsonPropertyName("includeNulls")]
        public bool? includeNulls { get; set; }

        public bool IsValid =>
            Property.IsNotNull()
            && Property.Trim().Any()
            && (
                (
                    includeNulls.IsNotNull()
                    && Range.IsNull()
                ) || (
                    (
                        Values.IsNotNull()
                        && Values.Any()
                    )
                    || (
                        Range.IsNotNull()
                        && Range.IsValid
                    )
                )
            );
    }
}
