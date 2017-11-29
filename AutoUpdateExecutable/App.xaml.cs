using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AutoUpdateExecutable
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            StartupArgsHandler.LoadStartupArgs(e);

            Shutdown();
        }
    }

    public sealed class StartupArgsHandler
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
            for (int i = 0; i < args.Args.Length; i++)
            {
                if (args.Args[i] == "Update")
                {
                    installFileLocation = args.Args[i + 1];
                    manifestFileLocation = args.Args[i + 2];
                    break;
                }
            }
        }
    }

    public enum UpdateMode
    {
        Update,
        Install,
        Uninstall
    }
}
