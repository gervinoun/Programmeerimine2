using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KooliProjekt.WindowsForms.Api
{
    public class OperationResultResponse
    {
        [JsonPropertyName("propertyErrors")]
        public IDictionary<string, string> PropertyErrors { get; set; }

        [JsonPropertyName("errors")]
        public IList<string> Errors { get; set; }

        [JsonPropertyName("hasErrors")]
        public bool HasErrors { get; set; }
    }
}
