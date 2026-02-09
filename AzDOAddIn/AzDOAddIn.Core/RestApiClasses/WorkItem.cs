using Newtonsoft.Json;
using System.Collections.Generic;

namespace AzDOAddIn.RestApiClasses
{
    public class WorkItemsResponse
    {
        [JsonProperty("count")]
        public int count { get; set; }
        [JsonProperty("value")]
        public WorkItem[] WorkItems { get; set; }
    }

    public class WorkItem
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("rev")]
        public int rev { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("fields")]
        public Dictionary<string, object> fields { get; set; }
        [JsonProperty("relations")]
        public WorkItemRelation[] relations { get; set; }

        public class WorkItemRelation
        {
            [JsonProperty("url")]
            public string url { get; set; }
            [JsonProperty("rel")]
            public string rel { get; set; }
            [JsonProperty("attributes")]
            public object attributes { get; set; }
        }
    }
}
