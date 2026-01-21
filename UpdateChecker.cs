using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace User.PluginMiniSectors
{
    public class UpdateCheckResult
    {
        public bool UpdateAvailable { get; set; }
        public string CurrentCommit { get; set; }
        public string LatestCommit { get; set; }
        public string DownloadUrl { get; set; }
        public string ReleaseDate { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UpdateChecker
    {
        private const string GitHubApiBase = "https://api.github.com";
        private const string RepoOwner = "jtexp";
        private const string RepoName = "SimhubPluginMiniSectors";
        private const string ReleaseTag = "nightly";
        private const string AssetName = "User.PluginMiniSectors.zip";

        private readonly HttpClient _httpClient;

        public UpdateChecker()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MiniSectorsPlugin");
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        }

        public static string GetCurrentCommit()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream("User.PluginMiniSectors.GitCommit.txt"))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd().Trim();
                        }
                    }
                }
            }
            catch
            {
                // Ignore
            }

            // Fallback: check for file next to assembly
            try
            {
                var assemblyPath = Assembly.GetExecutingAssembly().Location;
                var commitFile = Path.Combine(Path.GetDirectoryName(assemblyPath), "GitCommit.txt");
                if (File.Exists(commitFile))
                {
                    return File.ReadAllText(commitFile).Trim();
                }
            }
            catch
            {
                // Ignore
            }

            return "unknown";
        }

        public static string GetCurrentVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            // Try to get informational version first (e.g., "0.0.0-nightly")
            var infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            if (infoVersion != null && !string.IsNullOrEmpty(infoVersion.InformationalVersion))
            {
                return infoVersion.InformationalVersion;
            }
            // Fall back to numeric version
            var version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public async Task<UpdateCheckResult> CheckForUpdateAsync()
        {
            var result = new UpdateCheckResult
            {
                CurrentCommit = GetCurrentCommit()
            };

            try
            {
                // Fetch release info for the nightly tag
                var releaseUrl = $"{GitHubApiBase}/repos/{RepoOwner}/{RepoName}/releases/tags/{ReleaseTag}";
                var response = await _httpClient.GetAsync(releaseUrl);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        result.ErrorMessage = "No nightly release found.";
                    }
                    else
                    {
                        result.ErrorMessage = $"GitHub API error: {response.StatusCode}";
                    }
                    return result;
                }

                var json = await response.Content.ReadAsStringAsync();
                var release = JObject.Parse(json);

                // Parse commit from release notes
                var body = release["body"]?.ToString() ?? "";
                result.LatestCommit = ParseCommitFromReleaseNotes(body);
                result.ReleaseDate = ParseDateFromReleaseNotes(body);

                // Find the zip asset
                var assets = release["assets"] as JArray;
                if (assets != null)
                {
                    foreach (var asset in assets)
                    {
                        if (asset["name"]?.ToString() == AssetName)
                        {
                            result.DownloadUrl = asset["browser_download_url"]?.ToString();
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(result.DownloadUrl))
                {
                    result.ErrorMessage = "Release found but zip asset is missing.";
                    return result;
                }

                // Compare commits - if different, update available
                // "unknown" current commit always means update available
                if (result.CurrentCommit == "unknown" ||
                    (!string.IsNullOrEmpty(result.LatestCommit) &&
                     !result.CurrentCommit.StartsWith(result.LatestCommit, StringComparison.OrdinalIgnoreCase) &&
                     !result.LatestCommit.StartsWith(result.CurrentCommit, StringComparison.OrdinalIgnoreCase)))
                {
                    result.UpdateAvailable = true;
                }

                return result;
            }
            catch (HttpRequestException ex)
            {
                result.ErrorMessage = $"Network error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Error checking for updates: {ex.Message}";
                return result;
            }
        }

        public async Task<string> DownloadUpdateAsync(string downloadUrl, Action<int> progressCallback = null)
        {
            // Create temp directory for download
            var tempDir = Path.Combine(Path.GetTempPath(), "MiniSectorsUpdate");
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, recursive: true);
            }
            Directory.CreateDirectory(tempDir);

            var zipPath = Path.Combine(tempDir, AssetName);

            // Download the zip file
            using (var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                var downloadedBytes = 0L;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var buffer = new byte[8192];
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        if (totalBytes > 0 && progressCallback != null)
                        {
                            var percent = (int)((downloadedBytes * 100) / totalBytes);
                            progressCallback(percent);
                        }
                    }
                }
            }

            // Extract the zip
            var extractDir = Path.Combine(tempDir, "extracted");
            ZipFile.ExtractToDirectory(zipPath, extractDir);

            // Find the DLL in extracted contents
            var dllPath = Path.Combine(extractDir, "User.PluginMiniSectors.dll");
            if (!File.Exists(dllPath))
            {
                throw new FileNotFoundException("DLL not found in downloaded archive.");
            }

            return dllPath;
        }

        public static string ComputeSHA256(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public void LaunchUpdater(string newDllPath, string expectedHash = null)
        {
            // Find paths
            var pluginDll = Assembly.GetExecutingAssembly().Location;
            var simhubFolder = Path.GetDirectoryName(pluginDll);
            var simhubExe = Path.Combine(simhubFolder, "SimHubWPF.exe");
            var updaterExe = Path.Combine(simhubFolder, "MiniSectorsUpdater.exe");

            if (!File.Exists(updaterExe))
            {
                throw new FileNotFoundException("Updater not found. Please reinstall the plugin.", updaterExe);
            }

            // Build arguments
            var args = $"--simhub \"{simhubExe}\" --dll \"{newDllPath}\" --target \"{pluginDll}\"";
            if (!string.IsNullOrEmpty(expectedHash))
            {
                args += $" --hash \"{expectedHash}\"";
            }

            // Launch updater
            var startInfo = new ProcessStartInfo
            {
                FileName = updaterExe,
                Arguments = args,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }

        private static string ParseCommitFromReleaseNotes(string body)
        {
            // Look for "Commit: abc123..." pattern
            var match = Regex.Match(body, @"Commit:\s*([a-f0-9]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        private static string ParseDateFromReleaseNotes(string body)
        {
            // Look for "Built: 2024-01-15 10:30 UTC" pattern
            var match = Regex.Match(body, @"Built:\s*(.+?UTC)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }
    }
}
