using Newtonsoft.Json;

namespace AzDOAddIn.RestApiClasses
{
    public class TeamMembersResponse
    {
        [JsonProperty("count")]
        public int count { get; set; }
        [JsonProperty("value")]
        public TeamMember[] TeamMembers { get; set; }
    }

    public class TeamMember
    {
        [JsonProperty("identity")]
        public IdentityRef identity { get; set; }
        [JsonProperty("isTeamAdmin")]
        public bool isTeamAdmin { get; set; }
    }

    public class IdentityRef
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("displayName")]
        public string displayName { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("descriptor")]
        public string descriptor { get; set; }
        [JsonProperty("imageUrl")]
        public string imageUrl { get; set; }
        [JsonProperty("isDeletedInOrigin")]
        public bool isDeletedInOrigin { get; set; }
        [JsonProperty("_links")]
        public object _links { get; set; }
    }
}
