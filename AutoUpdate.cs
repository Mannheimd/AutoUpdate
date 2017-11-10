using Log_Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutoUpdate
{
    class VersionFile
    {
        private XmlDocument versionXml;

        private Version _latestVersion;
        public Version latestVersion
        {
            get
            {
                return _latestVersion;
            }
        }

        private Version _recommendedVersion;
        public Version recommendedVersion
        {
            get
            {
                return _recommendedVersion;
            }
        }

        private Version _minimumVersion;
        public Version minimumVersion
        {
            get
            {
                return _minimumVersion;
            }
        }

        private bool _versionInfoLoaded;

        /// <summary>
        /// Loads version information from a version file
        /// </summary>
        /// <param name="url">URL of the version file</param>
        public void Load(string url)
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
                return;
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
                return;
            }

            GetLatestVersion(defaultChannel);
            GetRecommendedVersion(defaultChannel);
            GetMinimumVersion(defaultChannel);

            LogHandler.CreateEntry(SeverityLevel.Trace, "Finished loading version file");
        }

        /// <summary>
        /// Loads version information for the specified channel from a version file
        /// </summary>
        /// <param name="url">URL of the version file</param>
        /// <param name="channel">Channel to check under</param>
        public void Load(string url, string channel)
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
                return;
            }
            
            GetLatestVersion(channel);
            GetRecommendedVersion(channel);
            GetMinimumVersion(channel);
            
            LogHandler.CreateEntry(SeverityLevel.Trace, "Finished loading version file");
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
    }

    class Version
    {
        public string versionNumber { get; set; }
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
                versionNumber = versionNode.SelectSingleNode("VersionNumber").Value;

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

    class Asset
    {
        public AssetType type { get; }
        public Version version { get; }

        public Asset(AssetType type, Version version)
        {
            this.type = type;
            this.version = version;
        }
    }

    class RegistrySubKeyAsset : Asset
    {
        public string subKeyPath { get; }

        public RegistrySubKeyAsset(string subKeyPath, Version version) : base(AssetType.RegistrySubKey, version)
        {
            this.subKeyPath = subKeyPath;
        }
    }

    class RegistryValueAsset : Asset
    {
        public string subKeyPath { get; }
        public string value { get; }

        public RegistryValueAsset(string subKeyPath, string value, Version version) : base(AssetType.RegistryValue, version)
        {
            this.subKeyPath = subKeyPath;
            this.value = value;
        }
    }

    class DirectoryAsset : Asset
    {
        public string directoryPath { get; }

        public DirectoryAsset(string directoryPath, Version version) : base(AssetType.RegistryValue, version)
        {
            this.directoryPath = directoryPath;
        }
    }

    class FileAsset : Asset
    {
        public string directoryPath { get; }
        public string fileName { get; }

        public FileAsset(string directoryPath,string fileName, Version version) : base(AssetType.RegistryValue, version)
        {
            this.directoryPath = directoryPath;
            this.fileName = fileName;
        }
    }

    enum ChangeStatus
    {
        NotStarted,
        InProgress,
        Successful,
        Failed,
        RevertPending,
        RevertInProgress,
        RevertSuccessful,
        RevertFailed
    }

    /// <summary>
    /// Details the type of asset that might be listed in a manifest file
    /// </summary>
    enum AssetType
    {
        RegistrySubKey,
        RegistryValue,
        Folder,
        File
    }
}
