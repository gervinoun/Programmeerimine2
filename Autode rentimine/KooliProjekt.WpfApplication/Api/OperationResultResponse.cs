using System.Collections.Generic;
using Newtonsoft.Json;

namespace KooliProjekt.WpfApplication.Api
{
    public class OperationResultResponse
    {
        [JsonProperty("propertyErrors")]
        public IDictionary<string, string> PropertyErrors { get; set; }

        [JsonProperty("errors")]
        public IList<string> Errors { get; set; }

        [JsonProperty("hasErrors")]
        public bool HasErrors { get; set; }
    }
}
