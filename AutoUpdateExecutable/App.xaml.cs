using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Log_Handler;
using System.Xml;

namespace AutoUpdateExecutable
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ApplicationVariables.Load();

            StartupArgsHandler.LoadStartupArgs(e);
        }
    }

    internal class Manifest
    {
        private XmlDocument xmlDocument;
        public Version version;
        public Guid applicationGuid;
        public List<FileAsset> fileAssets;
        public List<FolderAsset> folderAssets;
        public List<RegistryAsset> registryAssets;
        public List<OneTimeAsset> oneTimeAssets;
        
        /// <summary>
        /// Loads the manifest file from the given location
        /// </summary>
        /// <param name="location">String representation of a URL or local file path where the manifest file is stored</param>
        public void Load(string location)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Loading manifest file from " + location);
            try
            {
                xmlDocument.Load(location);
            }
            catch (Exception e)
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load xml file from manifest");
                return;
            }

            if (xmlDocument.SelectSingleNode("Manifest/Version") != null)
            {
                try
                {
                    version = new Version(xmlDocument.SelectSingleNode("Manifest/Version").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse version from manifest");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load version node from manifest");
            }

            if (xmlDocument.SelectSingleNode("Manifest/ApplicationGuid") != null)
            {
                try
                {
                    applicationGuid = new Guid(xmlDocument.SelectSingleNode("Manifest/ApplicationGuid").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse guid from manifest");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load guid node from manifest");
            }

            if (xmlDocument.SelectNodes("Manifest/FileAssets") != null)
            {
                foreach (XmlNode assetNode in xmlDocument.SelectNodes("Manifest/FileAssets"))
                {
                    fileAssets.Add(new FileAsset(assetNode));
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load file asset nodes from manifest");
            }

            if (xmlDocument.SelectNodes("Manifest/FolderAssets") != null)
            {
                foreach (XmlNode assetNode in xmlDocument.SelectNodes("Manifest/FolderAssets"))
                {
                    folderAssets.Add(new FolderAsset(assetNode));
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load folder asset nodes from manifest");
            }

            if (xmlDocument.SelectNodes("Manifest/RegistryAssets") != null)
            {
                foreach (XmlNode assetNode in xmlDocument.SelectNodes("Manifest/RegistryAssets"))
                {
                    registryAssets.Add(new RegistryAsset(assetNode));
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load registry asset nodes from manifest");
            }

            if (xmlDocument.SelectNodes("Manifest/OneTimeAssets") != null)
            {
                foreach (XmlNode assetNode in xmlDocument.SelectNodes("Manifest/OneTimeAssets"))
                {
                    oneTimeAssets.Add(new OneTimeAsset(assetNode));
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load one time asset nodes from manifest");
            }
        }
    }

    internal class DestinationFilePath
    {
        private string filePath;

        public DestinationFilePath(string filePath)
        {
            this.filePath = filePath;
        }

        public override string ToString()
        {
            string parsedPath = filePath;
            parsedPath.Replace(@"[InstallDirectory]\", ApplicationVariables.applicationDirectory);
            parsedPath.Replace(@"[AppData]\", ApplicationVariables.appData);
            parsedPath.Replace(@"[Desktop]\", ApplicationVariables.desktop);
            parsedPath.Replace(@"[StartMenu]\", ApplicationVariables.startMenu);
            return parsedPath;
        }
    }

    internal class FileAsset
    {
        public Version version;
        public string fileName;
        public DestinationFilePath destinationPath;

        public FileAsset(XmlNode assetNode)
        {
            if (assetNode.SelectSingleNode("Version") != null)
            {
                try
                {
                    version = new Version(assetNode.SelectSingleNode("Version").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse version from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load version node from file asset node");
            }

            if (assetNode.SelectSingleNode("FileName") != null)
            {
                try
                {
                    fileName = assetNode.SelectSingleNode("FileName").InnerText;
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse file name from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load file name from file asset node");
            }

            if (assetNode.SelectSingleNode("DestinationPath") != null)
            {
                try
                {
                    destinationPath = new DestinationFilePath(assetNode.SelectSingleNode("DestinationPath").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse destination file path from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load destination file path from file asset node");
            }
        }
    }

    internal class FolderAsset
    {
        public Version version;
        public DestinationFilePath destinationPath;

        public FolderAsset(XmlNode assetNode)
        {
            if (assetNode.SelectSingleNode("Version") != null)
            {
                try
                {
                    version = new Version(assetNode.SelectSingleNode("Version").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse version from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load version node from file asset node");
            }

            if (assetNode.SelectSingleNode("DestinationPath") != null)
            {
                try
                {
                    destinationPath = new DestinationFilePath(assetNode.SelectSingleNode("DestinationPath").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse destination file path from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load destination file path from file asset node");
            }
        }
    }

    internal class RegistryAsset
    {
        public Version version;
        public string keyPath;
        public string keyName;
        public string keyValue;

        public RegistryAsset(XmlNode assetNode)
        {
            if (assetNode.SelectSingleNode("Version") != null)
            {
                try
                {
                    version = new Version(assetNode.SelectSingleNode("Version").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse version from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load version node from file asset node");
            }

            if (assetNode.SelectSingleNode("KeyPath") != null)
            {
                try
                {
                    keyPath = assetNode.SelectSingleNode("KeyPath").InnerText;
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse registry key path from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load registry key path from file asset node");
            }

            if (assetNode.SelectSingleNode("KeyName") != null)
            {
                try
                {
                    keyName = assetNode.SelectSingleNode("KeyName").InnerText;
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse registry key name from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load registry key name from file asset node");
            }

            if (assetNode.SelectSingleNode("KeyValue") != null)
            {
                try
                {
                    keyValue = assetNode.SelectSingleNode("KeyValue").InnerText;
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse registry key value from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load registry key value from file asset node");
            }
        }
    }

    internal class OneTimeAsset
    {
        public Version version;
        public string fileName;

        public OneTimeAsset(XmlNode assetNode)
        {

            if (assetNode.SelectSingleNode("Version") != null)
            {
                try
                {
                    version = new Version(assetNode.SelectSingleNode("Version").InnerText);
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse version from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load version node from file asset node");
            }

            if (assetNode.SelectSingleNode("FileName") != null)
            {
                try
                {
                    fileName = assetNode.SelectSingleNode("FileName").InnerText;
                }
                catch
                {
                    LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to parse one-time file name from file asset node");
                }
            }
            else
            {
                LogHandler.CreateEntry(SeverityLevel.Warn, "Unable to load one-time file name from file asset node");
            }
        }
    }

    internal static class ApplicationVariables
    {
        public static string applicationDirectory;
        public static string appData;
        public static string desktop;
        public static string startMenu;
        public static Manifest currentManifest;

        public static void Load()
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Loading application variables");
            applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
            appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            startMenu = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            currentManifest = new Manifest();
            currentManifest.Load(applicationDirectory + "InstallManifest.xml");
        }
    }

    internal sealed class StartupArgsHandler
    {
        private static readonly StartupArgsHandler _instance = new StartupArgsHandler();

        static StartupArgsHandler()
        {

        }

        private StartupArgsHandler()
        {

        }

        public static StartupArgsHandler instance
        {
            get
            {
                return _instance;
            }
        }

        private static UpdateMode updateMode;
        private static string installFileLocation;
        private static string manifestFileLocation;

        public static void LoadStartupArgs(StartupEventArgs args)
        {
            LogHandler.CreateEntry(SeverityLevel.Debug, "Loading startup args");
            for (int i = 0; i < args.Args.Length; i++)
            {
                if (args.Args[i] == "Update")
                {
                    LogHandler.CreateEntry(SeverityLevel.Trace, "Arg " + i.ToString() + " is 'Update'");
                    updateMode = UpdateMode.Update;
                    installFileLocation = args.Args[i + 1];
                    manifestFileLocation = args.Args[i + 2];
                    break;
                }
            }

            LogHandler.CreateEntry(SeverityLevel.Trace, "Finished loading startup args");
        }
    }

    public enum UpdateMode
    {
        Update,
        Install,
        Uninstall
    }
}
