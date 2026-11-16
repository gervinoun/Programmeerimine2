using System.Collections.Generic;

namespace KooliProjekt.BlazorWasm.Api
{
    public class OperationResultResponse<T>
    {
        public T Value { get; set; }
        public IDictionary<string, string> PropertyErrors { get; set; } = new Dictionary<string, string>();
        public IList<string> Errors { get; set; } = new List<string>();
        public bool HasErrors { get; set; }
    }
}
