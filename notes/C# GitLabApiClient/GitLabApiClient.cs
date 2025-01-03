using Console.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Console.ApiClient
{
    public class GitLabApiClient
    {
        private readonly string _gitLabUrl;
        private readonly string _privateToken;
        private readonly IHttpClientFactory _httpClientFactory;

        public GitLabApiClient(IHttpClientFactory httpClientFactory, IOptions<GitLabApiOptions> gitLabApiOptions)
        {
            _gitLabUrl = gitLabApiOptions.Value.Url;
            _privateToken = gitLabApiOptions.Value.PrivateToken;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary> 下載指定專案和分支的儲存庫壓縮檔案，並將其儲存到指定的路徑 </summary>
        public async Task DownloadRepositoryArchiveAsync(int projectId, string branch, string savePath)
        {
            string url = $"{_gitLabUrl}/api/v4/projects/{projectId}/repository/archive.zip?ref={branch}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("PRIVATE-TOKEN", _privateToken);
            var client = _httpClientFactory.CreateClient("gitlab");
            var resp = await client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            using var fs = new FileStream(savePath, FileMode.Create);
            await resp.Content.CopyToAsync(fs);
        }

        /// <summary> 取得指定專案和分支下特定路徑的檔案資訊 </summary>
        public async Task<List<GitLabFileDto>> GetFileInfosAsync(int projectId, string branch, string path)
        {
            string url = $"{_gitLabUrl}/api/v4/projects/{projectId}/repository/tree?path={path}&ref={branch}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("PRIVATE-TOKEN", _privateToken);
            var client = _httpClientFactory.CreateClient("gitlab");
            var resp = await client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            string respStr = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<GitLabFileDto>>(respStr) ?? new();
        }

        /// <summary> 下載指定專案和分支下特定路徑的檔案，並將其儲存到指定的路徑 </summary>
        public async Task DownloadFileAsync(int projectId, string branch, string path, string savePath)
        {
            string url = $"{_gitLabUrl}/api/v4/projects/{projectId}/repository/files/{Uri.EscapeDataString(path)}/raw?ref={branch}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("PRIVATE-TOKEN", _privateToken);
            var client = _httpClientFactory.CreateClient("gitlab");
            var resp = await client.SendAsync(req);
            resp.EnsureSuccessStatusCode();
            using var fs = new FileStream(savePath, FileMode.Create);
            await resp.Content.CopyToAsync(fs);
        }
    }
}