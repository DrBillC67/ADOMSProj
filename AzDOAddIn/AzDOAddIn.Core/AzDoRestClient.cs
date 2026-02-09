using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzDOAddIn.RestApiClasses;
using Newtonsoft.Json;

namespace AzDOAddIn.Core
{
    /// <summary>
    /// Azure DevOps REST API client using a single shared HttpClient and async methods.
    /// </summary>
    public sealed class AzDoRestClient
    {
        private const string ParentLinkName = "System.LinkTypes.Hierarchy-Reverse";
        private const string ApiVersion = "7.0";

        private static readonly Lazy<HttpClient> SharedClient = new Lazy<HttpClient>(() =>
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        });

        private static HttpClient Client => SharedClient.Value;

        public static async Task<TeamProjectsResponse> GetTeamProjectsAsync(string azDoUrl, string pat)
        {
            var url = $"{azDoUrl.TrimEnd('/')}/_apis/projects?api-version={ApiVersion}";
            return await InvokeAsync<TeamProjectsResponse>(HttpMethod.Get, url, null, pat).ConfigureAwait(false);
        }

        public static async Task<WebApiTeamResponse> GetTeamsAsync(string azDoUrl, string teamProject, string pat)
        {
            var url = $"{azDoUrl.TrimEnd('/')}/_apis/projects/{Uri.EscapeDataString(teamProject)}/teams?api-version={ApiVersion}";
            return await InvokeAsync<WebApiTeamResponse>(HttpMethod.Get, url, null, pat).ConfigureAwait(false);
        }

        public static async Task<TeamMembersResponse> GetTeamMembersAsync(string azDoUrl, string teamProject, string team, string pat)
        {
            var url = $"{azDoUrl.TrimEnd('/')}/_apis/projects/{Uri.EscapeDataString(teamProject)}/teams/{Uri.EscapeDataString(team)}/members?api-version={ApiVersion}";
            return await InvokeAsync<TeamMembersResponse>(HttpMethod.Get, url, null, pat).ConfigureAwait(false);
        }

        public static async Task<WorkItem> GetWorkItemAsync(string azDoUrl, string teamProject, int wiId, string pat)
        {
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/workitems/{wiId}?$expand=relations&api-version={ApiVersion}";
            return await InvokeAsync<WorkItem>(HttpMethod.Get, url, null, pat).ConfigureAwait(false);
        }

        public static async Task<WorkItemsResponse> GetWorkItemsAsync(string azDoUrl, string teamProject, string wiIds, string pat, string fields = "\"System.Id\",\"System.Title\",\"System.WorkItemType\"")
        {
            var body = $"{{\"ids\":[{wiIds}],\"fields\":[{fields}]}}";
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/workitemsbatch?api-version={ApiVersion}";
            return await InvokeAsync<WorkItemsResponse>(HttpMethod.Post, url, body, pat, "application/json").ConfigureAwait(false);
        }

        public static async Task<WorkItemTypeResponse> GetWorkItemTypesAsync(string azDoUrl, string teamProject, string pat)
        {
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/workitemtypes?api-version={ApiVersion}";
            return await InvokeAsync<WorkItemTypeResponse>(HttpMethod.Get, url, null, pat).ConfigureAwait(false);
        }

        public static async Task<WorkItemQueryResult> GetWiqlResultAsync(string azDoUrl, string teamProject, string wiqlText, string pat)
        {
            var escaped = wiqlText.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " ");
            var body = $"{{\"query\":\"{escaped}\"}}";
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/wiql?api-version={ApiVersion}";
            return await InvokeAsync<WorkItemQueryResult>(HttpMethod.Post, url, body, pat, "application/json").ConfigureAwait(false);
        }

        public static async Task<List<WorkItem>> GetChildWorkItemsAsync(string azDoUrl, string teamProject, int wiId, string pat)
        {
            var queryText = $"SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.Id] = {wiId}) And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') ORDER BY [System.Id] mode(MayContain)";
            var result = await GetWiqlResultAsync(azDoUrl, teamProject, queryText, pat).ConfigureAwait(false);
            var list = new List<WorkItem>();
            if (result?.workItemRelations == null) return list;
            foreach (var relation in result.workItemRelations)
            {
                if (relation?.target == null) continue;
                var item = await GetWorkItemAsync(azDoUrl, teamProject, relation.target.id, pat).ConfigureAwait(false);
                if (item != null) list.Add(item);
            }
            return list;
        }

        public static async Task<WorkItem> PublishNewWorkItemAsync(string azDoUrl, string teamProject, string pat, string workItemType, Dictionary<string, string> fields, int parentId = 0)
        {
            var operations = new List<object>();
            foreach (var kv in fields)
                operations.Add(new { op = "add", path = $"/fields/{kv.Key}", @from = (string)null, value = kv.Value });
            if (parentId > 0)
                operations.Add(new { op = "add", path = "/relations/-", value = new { rel = ParentLinkName, url = $"{azDoUrl.TrimEnd('/')}/_apis/wit/workItems/{parentId}" } });
            var body = JsonConvert.SerializeObject(operations);
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/workitems/${Uri.EscapeDataString(workItemType)}?api-version={ApiVersion}";
            return await InvokeAsync<WorkItem>(HttpMethod.Post, url, body, pat, "application/json-patch+json").ConfigureAwait(false);
        }

        public static async Task<WorkItem> AddParentLinkAsync(string azDoUrl, string teamProject, string pat, int id, int parentId)
        {
            var body = $"[{{\"op\":\"add\",\"path\":\"/relations/-\",\"value\":{{\"rel\":\"{ParentLinkName}\",\"url\":\"{azDoUrl.TrimEnd('/')}/_apis/wit/workItems/{parentId}\"}}}}]";
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/workitems/{id}?api-version={ApiVersion}";
            return await InvokeAsync<WorkItem>(new HttpMethod("PATCH"), url, body, pat, "application/json-patch+json").ConfigureAwait(false);
        }

        public static async Task<WorkItem> RemoveParentLinkAsync(string azDoUrl, string teamProject, string pat, int id)
        {
            var workItem = await GetWorkItemAsync(azDoUrl, teamProject, id, pat).ConfigureAwait(false);
            if (workItem?.relations == null || workItem.relations.Length == 0)
                return workItem;
            int parentIndex = -1;
            for (int i = 0; i < workItem.relations.Length; i++)
            {
                if (workItem.relations[i].rel == ParentLinkName) { parentIndex = i; break; }
            }
            if (parentIndex < 0) return workItem;
            var body = $"[{{\"op\":\"remove\",\"path\":\"/relations/{parentIndex}\"}}]";
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/workitems/{id}?api-version={ApiVersion}";
            return await InvokeAsync<WorkItem>(new HttpMethod("PATCH"), url, body, pat, "application/json-patch+json").ConfigureAwait(false);
        }

        public static async Task<WorkItem> UpdateWorkItemAsync(string azDoUrl, string teamProject, string pat, int id, Dictionary<string, string> fields)
        {
            var operations = fields.Select(kv => new { op = "add", path = $"/fields/{kv.Key}", @from = (string)null, value = kv.Value }).ToList<object>();
            var body = JsonConvert.SerializeObject(operations);
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/workitems/{id}?api-version={ApiVersion}";
            return await InvokeAsync<WorkItem>(new HttpMethod("PATCH"), url, body, pat, "application/json-patch+json").ConfigureAwait(false);
        }

        public static async Task<ClassificationNode> GetIterationNodeAsync(string azDoUrl, string teamProject, string pat, string path)
        {
            var url = $"{azDoUrl.TrimEnd('/')}/{Uri.EscapeDataString(teamProject)}/_apis/wit/classificationnodes/Iterations/{Uri.EscapeDataString(path)}?api-version={ApiVersion}";
            return await InvokeAsync<ClassificationNode>(HttpMethod.Get, url, null, pat).ConfigureAwait(false);
        }

        private static async Task<T> InvokeAsync<T>(HttpMethod method, string requestUrl, string requestBody, string pat, string contentType = "application/json")
        {
            using var request = new HttpRequestMessage(method, requestUrl);
            if (!string.IsNullOrEmpty(pat))
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));
            if (requestBody != null)
                request.Content = new StringContent(requestBody, Encoding.UTF8, contentType);

            var response = await Client.SendAsync(request).ConfigureAwait(false);
            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new AzDoApiException("401: Unauthorized. Check your PAT and permissions.", response.StatusCode, responseContent);
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new AzDoApiException("404: Not Found.", response.StatusCode, responseContent);
                throw new AzDoApiException($"Request failed: {response.StatusCode}. {responseContent}", response.StatusCode, responseContent);
            }

            return JsonConvert.DeserializeObject<T>(responseContent);
        }
    }
}
