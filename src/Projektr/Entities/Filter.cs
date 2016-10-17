using System.Collections.Generic;

namespace Projektr.Entities
{
    public class Filter
    {
        public IDictionary<string,Filter> Fields { get; set; }
    }
}
