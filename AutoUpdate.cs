using Log_Handler;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace AutoUpdate
{
    internal class VersionFile
    {
        private XmlDocument versionXml;

        private AvailableVersion _latestVersion;
        public AvailableVersion latestVersion
        {
            get
            {
                return _latestVersion;
            }
        }

        private AvailableVersion _recommendedVersion;
        public AvailableVersion recommendedVersion
        {
            get
            {
                return _recommendedVersion;
            }
        }

        private AvailableVersion _minimumVersion;
        public AvailableVersion minimumVersion
        {
            get
            {
                return _minimumVersion;
            }
        }

        private string _updaterExecutableUrl;
        public string updaterExecutableUrl
        {
            get
            {
                return _updaterExecutableUrl;
            }
        }

        /// <summary>
        /// Loads version information from a version file
        /// </summary>
        /// <param name="url">URL of the version file</param>
        public bool Load(string url)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Loading version file from " + url);

            versionXml = new XmlDocument();

            try
            {
                versionXml.Load(url);
                LogHandler.CreateEntry(SeverityLevel.Trace, "Version file loaded");
            }
            catch(Exception e)
            {
                LogHandler.CreateEntry(e, SeverityLevel.Warn, "Failed to fetch version file from server");
                return false;
            }

            LogHandler.CreateEntry(SeverityLevel.Trace, "Finding default channel");

            string defaultChannel = null;
            if (versionXml.SelectSingleNode("VersionInfo/DefaultChannel") != null)
            {
                defaultChannel = versionXml.SelectSingleNode("VersionInfo/DefaultChannel").InnerText;
                LogHandler.CreateEntry(SeverityLevel.Debug, "Using default channel " + defaultChannel);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to find default channel in version file");
                return false;
            }

            GetLatestVersion(defaultChannel);
            GetRecommendedVersion(defaultChannel);
            GetMinimumVersion(defaultChannel);
            GetUpdaterExecutableUrl();

            LogHandler.CreateEntry(SeverityLevel.Trace, "Finished loading version file");
            return true;
        }

        /// <summary>
        /// Loads version information for the specified channel from a version file
        /// </summary>
        /// <param name="url">URL of the version file</param>
        /// <param name="channel">Channel to check under</param>
        public bool Load(string url, string channel)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Loading version file from " + url);

            versionXml = new XmlDocument();

            try
            {
                versionXml.Load(url);
                LogHandler.CreateEntry(SeverityLevel.Trace, "Version file loaded");
            }
            catch (Exception e)
            {
                LogHandler.CreateEntry(e, SeverityLevel.Warn, "Failed to fetch version file from server");
                return false;
            }
            
            GetLatestVersion(channel);
            GetRecommendedVersion(channel);
            GetMinimumVersion(channel);
            GetUpdaterExecutableUrl();


            LogHandler.CreateEntry(SeverityLevel.Trace, "Finished loading version file");
            return true;
        }

        private void GetLatestVersion(string channel)
        {
            LogHandler.CreateEntry(SeverityLevel.Trace, "Getting latest version for channel '" + channel + "' from version xml");

            _latestVersion = new AvailableVersion();

            string versionNumber = null;
            if (versionXml.SelectSingleNode("VersionInfo/Channels/" + channel + "/LatestVersion") != null)
            {
                versionNumber = versionXml.SelectSingleNode("VersionInfo/Channels/" + channel + "/LatestVersion").InnerText;
                LogHandler.CreateEntry(SeverityLevel.Trace, "Latest version is " + versionNumber);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Info, "Failed to get latest version number from version file");
                return;
            }

            if (versionXml.SelectSingleNode("VersionInfo/Versions/Version[@VersionNumber='" + versionNumber + "']") != null)
            {
                _latestVersion.LoadFromXml(versionXml.SelectSingleNode("VersionInfo/Versions/Version[@VersionNumber='" + versionNumber + "']"));
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Info, "Failed to get latest version node from version file");
                return;
            }
        }

        private void GetRecommendedVersion(string channel)
        {
            LogHandler.CreateEntry(SeverityLevel.Trace, "Getting recommended version for channel '" + channel + "' from version xml");

            _recommendedVersion = new AvailableVersion();

            string versionNumber = null;
            if (versionXml.SelectSingleNode("VersionInfo/Channels/" + channel + "/RecommendedVersion") != null)
            {
                versionNumber = versionXml.SelectSingleNode("VersionInfo/Channels/" + channel + "/RecommendedVersion").InnerText;
                LogHandler.CreateEntry(SeverityLevel.Trace, "Recommended version is " + versionNumber);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to get recommended version number from version file");
                return;
            }

            if (versionXml.SelectSingleNode("VersionInfo/Versions/Version[@VersionNumber='" + versionNumber + "']") != null)
            {
                _recommendedVersion.LoadFromXml(versionXml.SelectSingleNode("VersionInfo/Versions/Version[@VersionNumber='" + versionNumber + "']"));
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to get recommended version node from version file");
                return;
            }
        }

        private void GetMinimumVersion(string channel)
        {
            LogHandler.CreateEntry(SeverityLevel.Trace, "Getting minimum version for channel '" + channel + "' from version xml");

            _minimumVersion = new AvailableVersion();

            string versionNumber = null;
            if (versionXml.SelectSingleNode("VersionInfo/Channels/" + channel + "/MinimumVersion") != null)
            {
                versionNumber = versionXml.SelectSingleNode("VersionInfo/Channels/" + channel + "/MinimumVersion").InnerText;
                LogHandler.CreateEntry(SeverityLevel.Trace, "Minimum version is " + versionNumber);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to get minimum version number from version file");
                return;
            }

            if (versionXml.SelectSingleNode("VersionInfo/Versions/Version[@VersionNumber='" + versionNumber + "']") != null)
            {
                _minimumVersion.LoadFromXml(versionXml.SelectSingleNode("VersionInfo/Versions/Version[@VersionNumber='" + versionNumber + "']"));
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to get minimum version node from version file");
                return;
            }
        }

        private void GetUpdaterExecutableUrl()
        {
            LogHandler.CreateEntry(SeverityLevel.Trace, "Getting updater executable url from version xml");
            
            if (versionXml.SelectSingleNode("VersionInfo/UpdaterExecutableUrl") != null)
            {
                _updaterExecutableUrl = versionXml.SelectSingleNode("VersionInfo/UpdaterExecutableUrl").InnerText;
                LogHandler.CreateEntry(SeverityLevel.Trace, "Updater executable url is " + _updaterExecutableUrl);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to get updater executable url from version file");
                return;
            }
        }
    }

    public class AvailableVersion
    {
        public Version versionNumber { get; set; }
        public DateTime releaseDate { get; set; }
        public string changeLog { get; set; }
        public string targetChannel { get; set; }
        public string installFileLocation { get; set; }
        public string manifestFileLocation { get; set; }

        /// <summary>
        /// Loads Version values from a version XML node
        /// </summary>
        /// <param name="versionNode"></param>
        public void LoadFromXml(XmlNode versionNode)
        {
            LogHandler.CreateEntry(SeverityLevel.Trace, "Loading version information from XML node");

            if (versionNode.Attributes["VersionNumber"] != null)
            {
                try
                {
                    LogHandler.CreateEntry(SeverityLevel.Trace, "Raw version: " + versionNode.Attributes["VersionNumber"].Value);
                    versionNumber = new Version(versionNode.Attributes["VersionNumber"].Value);
                    LogHandler.CreateEntry(SeverityLevel.Trace, "Parsed version: " + versionNumber);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Error, "Unable to parse version number from string:\n" + versionNode.Attributes["VersionNumber"].Value);
                }
            }

            if (versionNode.SelectSingleNode("ReleaseDate") != null)
            {
                try
                {
                    releaseDate = Convert.ToDateTime(versionNode.SelectSingleNode("ReleaseDate").InnerText);
                }
                catch
                {
                    releaseDate = new DateTime();
                }
            }

            if (versionNode.SelectSingleNode("ChangeLog") != null)
                changeLog = versionNode.SelectSingleNode("ChangeLog").InnerText;

            if (versionNode.SelectSingleNode("TargetChannel") != null)
                targetChannel = versionNode.SelectSingleNode("TargetChannel").InnerText;

            if (versionNode.SelectSingleNode("InstallFileLocation") != null)
                installFileLocation = versionNode.SelectSingleNode("InstallFileLocation").InnerText;

            if (versionNode.SelectSingleNode("ManifestFileLocation") != null)
                manifestFileLocation = versionNode.SelectSingleNode("ManifestFileLocation").InnerText;
        }
    }

    public sealed class UpdateHandler
    {
        private static readonly UpdateHandler _instance = new UpdateHandler();

        private static VersionFile versionFile;
        private static Version currentVersion;
        private static string applicationDirectory;
        private static string updaterExecutableFilePath;
        private static bool updaterExecutableDownloadSuccess;

        static UpdateHandler()
        {
            currentVersion = Assembly.GetEntryAssembly().GetName().Version;
            applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            updaterExecutableFilePath = applicationDirectory + @"AutoUpdate.exe";
        }

        private UpdateHandler()
        {
            currentVersion = Assembly.GetEntryAssembly().GetName().Version;
            applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            updaterExecutableFilePath = applicationDirectory + @"AutoUpdate.exe";
        }

        public static UpdateHandler instance
        {
            get
            {
                return _instance;
            }
        }

        public UpdateAvailabilityResponse CheckForUpdate(string url, string channel)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Checking for updates from '" + url + "' with channel '" + channel + "'");

            if (versionFile == null)
            {
                versionFile = new VersionFile();
            }

            if (!versionFile.Load(url, channel))
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to load version file");
                return new UpdateAvailabilityResponse(UpdateAvailability.UpdateCheckFailed);
            }

            if (versionFile.minimumVersion.versionNumber != null)
            {
                LogHandler.CreateEntry(SeverityLevel.Trace, "Comparing minimum version for upgrade");
                if (versionFile.minimumVersion.versionNumber > currentVersion)
                {
                    LogHandler.CreateEntry(SeverityLevel.Debug, "Located required upgrade");
                    return new UpdateAvailabilityResponse(versionFile.minimumVersion, UpdateAvailability.UpgradeRequired);
                }
            }

            if (versionFile.recommendedVersion.versionNumber != null)
            {
                LogHandler.CreateEntry(SeverityLevel.Trace, "Comparing required version for upgrade");
                if (versionFile.recommendedVersion.versionNumber > currentVersion)
                {
                    LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended upgrade");
                    return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.UpgradeRecommended);
                }
            }

            if (versionFile.recommendedVersion.versionNumber != null)
            {
                LogHandler.CreateEntry(SeverityLevel.Trace, "Comparing required version for downgrade");
                if (versionFile.recommendedVersion.versionNumber < currentVersion)
                {
                    LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended downgrade");
                    return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.DowngradeRecommended);
                }
            }

            LogHandler.CreateEntry(SeverityLevel.Debug, "No update available");
            return new UpdateAvailabilityResponse(UpdateAvailability.NoUpdateAvailable);
        }

        public UpdateAvailabilityResponse CheckForUpdate(string url)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Checking for updates from '" + url + "'");

            if (versionFile == null)
            {
                versionFile = new VersionFile();
            }

            if (!versionFile.Load(url))
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to load version file");
                return new UpdateAvailabilityResponse(UpdateAvailability.UpdateCheckFailed);
            }
            
            if (versionFile.minimumVersion.versionNumber != null)
            {
                LogHandler.CreateEntry(SeverityLevel.Trace, "Comparing minimum version for upgrade");
                if (versionFile.minimumVersion.versionNumber > currentVersion)
                {
                    LogHandler.CreateEntry(SeverityLevel.Debug, "Located required upgrade");
                    return new UpdateAvailabilityResponse(versionFile.minimumVersion, UpdateAvailability.UpgradeRequired);
                }
            }

            if (versionFile.recommendedVersion.versionNumber != null)
            {
                LogHandler.CreateEntry(SeverityLevel.Trace, "Comparing required version for upgrade");
                if (versionFile.recommendedVersion.versionNumber > currentVersion)
                {
                    LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended upgrade");
                    return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.UpgradeRecommended);
                }
            }

            if (versionFile.recommendedVersion.versionNumber != null)
            {
                LogHandler.CreateEntry(SeverityLevel.Trace, "Comparing required version for downgrade");
                if (versionFile.recommendedVersion.versionNumber < currentVersion)
                {
                    LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended downgrade");
                    return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.DowngradeRecommended);
                }
            }

            LogHandler.CreateEntry(SeverityLevel.Debug, "No update available");
            return new UpdateAvailabilityResponse(UpdateAvailability.NoUpdateAvailable);
        }

        public void PerformUpdate(AvailableVersion targetVersion)
        {
            DownloadUpdaterExecutable(versionFile.updaterExecutableUrl);
            if (!updaterExecutableDownloadSuccess)
                return;

            string args = "update " + targetVersion.installFileLocation + " " + targetVersion.manifestFileLocation;

            LogHandler.CreateEntry(SeverityLevel.Info, "Launching " + updaterExecutableFilePath);
            Process updaterProcess = new Process();
            updaterProcess.StartInfo.UseShellExecute = false;
            updaterProcess.StartInfo.FileName = updaterExecutableFilePath;
            updaterProcess.StartInfo.CreateNoWindow = true;
            updaterProcess.StartInfo.Arguments = args;

            if (updaterProcess.Start())
            {
                LogHandler.CreateEntry(SeverityLevel.Trace, "AutoUpdate.exe started with Process ID " + updaterProcess.Id);
                LogHandler.CreateEntry(SeverityLevel.Info, "AutoUpdate.exe started correctly, closing application");
                Environment.Exit(0);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Error, "Failed to start AutoUpdate.exe");
            }
        }

        private static void DownloadUpdaterExecutable(string downloadUrl)
        {
            updaterExecutableDownloadSuccess = false;
            LogHandler.CreateEntry(SeverityLevel.Debug, "Fetching updater executable from " + downloadUrl);
            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(UpdaterExecutable_DownloadComplete);
                client.DownloadFileAsync(new Uri(downloadUrl), updaterExecutableFilePath);
            }
        }

        private static void UpdaterExecutable_DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            LogHandler.CreateEntry(SeverityLevel.Trace, "Updater executable download task finished");
            if (e.Cancelled)
            {
                LogHandler.CreateEntry(SeverityLevel.Error, "Updater executable download was cancelled");
                updaterExecutableDownloadSuccess = false;
            }
            else if (e.Error != null)
            {
                LogHandler.CreateEntry(e.Error, SeverityLevel.Error, "Updater executable download encountered an error");
                updaterExecutableDownloadSuccess = false;
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Debug, "Updater executable download was successful");
                updaterExecutableDownloadSuccess = false;
            }
        }
    }

    public class UpdateAvailabilityResponse
    {
        private UpdateAvailability _updateAvailability;
        public UpdateAvailability updateAvailability
        {
            get
            {
                return _updateAvailability;
            }
        }

        private AvailableVersion _newVersion;
        public AvailableVersion newVersion
        {
            get
            {
                return _newVersion;
            }
        }

        public UpdateAvailabilityResponse(AvailableVersion newVersion, UpdateAvailability updateAvailability)
        {
            _newVersion = newVersion;
            _updateAvailability = updateAvailability;
        }

        public UpdateAvailabilityResponse(UpdateAvailability updateAvailability)
        {
            _updateAvailability = updateAvailability;
        }
    }

    public enum UpdateAvailability
    {
        NoUpdateAvailable,
        UpgradeRecommended,
        UpgradeRequired,
        DowngradeRecommended,
        UpdateCheckFailed
    }
}
