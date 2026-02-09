using Newtonsoft.Json;

namespace AzDOAddIn.RestApiClasses
{
    public class WebApiTeamResponse
    {
        [JsonProperty("count")]
        public int count { get; set; }
        [JsonProperty("value")]
        public WebApiTeam[] WebApiTeams { get; set; }
    }

    public class WebApiTeam
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("projectId")]
        public string projectId { get; set; }
        [JsonProperty("projectName")]
        public string projectName { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("identityUrl")]
        public string identityUrl { get; set; }
        [JsonProperty("identity")]
        public object identity { get; set; }
    }
}
