using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempCodeStorage
{
    class TempCodeStorage
    {
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

            public FileAsset(string directoryPath, string fileName, Version version) : base(AssetType.RegistryValue, version)
            {
                this.directoryPath = directoryPath;
                this.fileName = fileName;
            }
        }
    }
}
