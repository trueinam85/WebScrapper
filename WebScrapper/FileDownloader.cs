using System.Net;

namespace WebScrapper
{
    public class FileDownloader
    {
        private List<string> DownloadedFiles = new();
        public string BaseUrl { get; set; }
        public FileDownloader(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public async Task Download(string filePath, bool isHomePage = false)
        {
            if (DownloadedFiles.Contains(filePath) || string.IsNullOrEmpty(filePath))
            {
                return;
            }

            DownloadedFiles.Add(filePath);
            filePath = CleanUrl(filePath);

            using var httpClient = new HttpClient();
            var resp = await httpClient.GetAsync(BaseUrl + filePath);

            var data = !resp.IsSuccessStatusCode && resp.StatusCode != HttpStatusCode.NotFound ? throw new Exception("Something went wrong!")
                 : await resp.Content.ReadAsByteArrayAsync();

            // it means file should be saved in the root
            if (isHomePage || filePath.LastIndexOf('/') == -1)
            {
                await SaveFile(filePath, data);
                return;
            }

            var directoryName = filePath.Substring(0, filePath.LastIndexOf('/'));

            if (!Directory.Exists(directoryName) && !string.IsNullOrEmpty(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            await SaveFile(filePath, data);

        }

        private static async Task SaveFile(string filePath, byte[] data)
        {
            using var fs = File.Create(filePath);
            await fs.WriteAsync(data, 0, data.Length);
            fs.Close();
        }

        private string CleanUrl(string url)
        {
            url = url.StartsWith(BaseUrl) ? url.Replace(BaseUrl, string.Empty) : url;

            return url.StartsWith("../") ? url.Replace("../", string.Empty) : url;
        }
    }
}
