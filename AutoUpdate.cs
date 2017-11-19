using Log_Handler;
using System;
using System.Reflection;
using System.Threading.Tasks;
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

        private bool _versionInfoLoaded;

        /// <summary>
        /// Loads version information from a version file
        /// </summary>
        /// <param name="url">URL of the version file</param>
        public bool Load(string url)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Loading version file from " + url);

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
            if (versionXml.SelectSingleNode("DefaultChannel") != null)
            {
                defaultChannel = versionXml.SelectSingleNode("DefaultChannel").Value;
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
            
            LogHandler.CreateEntry(SeverityLevel.Trace, "Finished loading version file");
            return true;
        }

        private void GetLatestVersion(string channel)
        {
            LogHandler.CreateEntry(SeverityLevel.Trace, "Getting latest version for channel '" + channel + "' from version xml");

            string versionNumber = null;
            if (versionXml.SelectSingleNode("Channels/" + channel + "/LatestVersion") != null)
            {
                versionNumber = versionXml.SelectSingleNode("Channels/" + channel + "/LatestVersion").Value;
                LogHandler.CreateEntry(SeverityLevel.Trace, "Latest version is " + versionNumber);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Info, "Failed to get latest version number from version file");
                return;
            }

            if (versionXml.SelectSingleNode("Versions/Version[@VersionNumber='" + versionNumber + "']") != null)
            {
                _latestVersion.LoadFromXml(versionXml.SelectSingleNode("Versions/Version[@VersionNumber='" + versionNumber + "']"));
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

            string versionNumber = null;
            if (versionXml.SelectSingleNode("Channels/" + channel + "/RecommendedVersion") != null)
            {
                versionNumber = versionXml.SelectSingleNode("Channels/" + channel + "/RecommendedVersion").Value;
                LogHandler.CreateEntry(SeverityLevel.Trace, "Recommended version is " + versionNumber);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to get recommended version number from version file");
                return;
            }

            if (versionXml.SelectSingleNode("Versions/Version[@VersionNumber='" + versionNumber + "']") != null)
            {
                _recommendedVersion.LoadFromXml(versionXml.SelectSingleNode("Versions/Version[@VersionNumber='" + versionNumber + "']"));
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

            string versionNumber = null;
            if (versionXml.SelectSingleNode("Channels/" + channel + "/MinimumVersion") != null)
            {
                versionNumber = versionXml.SelectSingleNode("Channels/" + channel + "/MinimumVersion").Value;
                LogHandler.CreateEntry(SeverityLevel.Trace, "Minimum version is " + versionNumber);
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to get minimum version number from version file");
                return;
            }

            if (versionXml.SelectSingleNode("Versions/Version[@VersionNumber='" + versionNumber + "']") != null)
            {
                _minimumVersion.LoadFromXml(versionXml.SelectSingleNode("Versions/Version[@VersionNumber='" + versionNumber + "']"));
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
                _updaterExecutableUrl = versionXml.SelectSingleNode("VersionInfo/UpdaterExecutableUrl").Value;
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

            if (versionNode.SelectSingleNode("VersionNumber") != null)
            {
                try
                {
                    versionNumber = new Version(versionNode.SelectSingleNode("VersionNumber").Value);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Error, "Unable to parse version number from string:\n" + versionNode.SelectSingleNode("VersionNumber").Value);
                }
            }

            if (versionNode.SelectSingleNode("ReleaseDate") != null)
            {
                try
                {
                    releaseDate = Convert.ToDateTime(versionNode.SelectSingleNode("ReleaseDate").Value);
                }
                catch
                {
                    releaseDate = new DateTime();
                }
            }

            if (versionNode.SelectSingleNode("ChangeLog") != null)
                changeLog = versionNode.SelectSingleNode("ChangeLog").Value;

            if (versionNode.SelectSingleNode("TargetChannel") != null)
                targetChannel = versionNode.SelectSingleNode("TargetChannel").Value;

            if (versionNode.SelectSingleNode("InstallFileLocation") != null)
                installFileLocation = versionNode.SelectSingleNode("InstallFileLocation").Value;

            if (versionNode.SelectSingleNode("ManifestFileLocation") != null)
                manifestFileLocation = versionNode.SelectSingleNode("ManifestFileLocation").Value;
        }
    }

    public sealed class UpdateHandler
    {
        private static readonly UpdateHandler _instance = new UpdateHandler();

        private static VersionFile versionFile;
        private static Version currentVersion;

        static UpdateHandler()
        {
            currentVersion = Assembly.GetEntryAssembly().GetName().Version;
        }

        private UpdateHandler()
        {
            currentVersion = Assembly.GetEntryAssembly().GetName().Version;
        }

        public static UpdateHandler instance
        {
            get
            {
                return _instance;
            }
        }

        public static UpdateAvailabilityResponse CheckForUpdate(string url, string channel)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Checking for updates from '" + url + "' with channel '" + channel);

            if (!versionFile.Load(url, channel))
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to load version file");
                return new UpdateAvailabilityResponse(UpdateAvailability.UpdateCheckFailed);
            }

            if (versionFile.minimumVersion.versionNumber > currentVersion)
            {
                LogHandler.CreateEntry(SeverityLevel.Debug, "Located required upgrade");
                return new UpdateAvailabilityResponse(versionFile.minimumVersion, UpdateAvailability.UpgradeRequired);
            }

            if (versionFile.recommendedVersion.versionNumber > currentVersion)
            {
                LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended upgrade");
                return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.UpgradeRecommended);
            }

            if (versionFile.recommendedVersion.versionNumber < currentVersion)
            {
                LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended downgrade");
                return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.DowngradeRecommended);
            }

            LogHandler.CreateEntry(SeverityLevel.Debug, "No update available");
            return new UpdateAvailabilityResponse(UpdateAvailability.NoUpdateAvailable);
        }

        public static UpdateAvailabilityResponse CheckForUpdate(string url)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Checking for updates from '" + url);

            if (!versionFile.Load(url))
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Failed to load version file");
                return new UpdateAvailabilityResponse(UpdateAvailability.UpdateCheckFailed);
            }

            if (versionFile.minimumVersion.versionNumber > currentVersion)
            {
                LogHandler.CreateEntry(SeverityLevel.Debug, "Located required upgrade");
                return new UpdateAvailabilityResponse(versionFile.minimumVersion, UpdateAvailability.UpgradeRequired);
            }

            if (versionFile.recommendedVersion.versionNumber > currentVersion)
            {
                LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended upgrade");
                return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.UpgradeRecommended);
            }

            if (versionFile.recommendedVersion.versionNumber < currentVersion)
            {
                LogHandler.CreateEntry(SeverityLevel.Debug, "Located recommended downgrade");
                return new UpdateAvailabilityResponse(versionFile.recommendedVersion, UpdateAvailability.DowngradeRecommended);
            }

            LogHandler.CreateEntry(SeverityLevel.Debug, "No update available");
            return new UpdateAvailabilityResponse(UpdateAvailability.NoUpdateAvailable);
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
