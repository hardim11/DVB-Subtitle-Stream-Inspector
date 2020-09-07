using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nikse.SubtitleEdit.Core
{
    public static class Configuration
    {


        public static IEnumerable<Encoding> AvailableEncodings => GetAvailableEncodings();


        private static IEnumerable<Encoding> GetAvailableEncodings()
        {
            var encodings = new List<Encoding>();
            foreach (var ei in Encoding.GetEncodings())
            {
                try
                {
                    encodings.Add(Encoding.GetEncoding(ei.CodePage));
                }
                catch
                {
                    // though advertised, this code page is not supported
                }
            }
            return encodings.AsEnumerable();
        }

        public static bool IsRunningOnLinux { get { return false; } }
        public static bool IsRunningOnWindows { get { return true; } }
        public static bool IsRunningOnMac { get { return false; } }
        public static readonly string BaseDirectory = GetBaseDirectory();
        public static readonly string DataDirectory = GetDataDirectory();
        public static readonly string DictionariesDirectory = DataDirectory + "Dictionaries" + Path.DirectorySeparatorChar;
        public static readonly string PluginsDirectory = DataDirectory + "Plugins" + Path.DirectorySeparatorChar;



        public static Settings Settings
        {
            get { return new Settings(); }
        }

        private static string GetBaseDirectory()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(assembly.Location) + Path.DirectorySeparatorChar;
        }



        private static string GetDataDirectory()
        {
            // hack for unit tests
            var assembly = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetExecutingAssembly();
            var srcTestResultsIndex = assembly.Location.IndexOf(@"\src\TestResults", StringComparison.Ordinal);
            if (srcTestResultsIndex > 0)
            {
                var debugOrReleaseFolderName = "Release";
#if DEBUG
                debugOrReleaseFolderName = "Debug";
#endif
                return $@"{assembly.Location.Substring(0, srcTestResultsIndex)}\src\Test\bin\{debugOrReleaseFolderName}\";
            }

            var appDataRoamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Subtitle Edit");
            if (IsRunningOnLinux || IsRunningOnMac)
            {
                if (!Directory.Exists(appDataRoamingPath) && !File.Exists(Path.Combine(BaseDirectory, ".PACKAGE-MANAGER")))
                {
                    try
                    {
                        var path = Path.Combine(Directory.CreateDirectory(Path.Combine(BaseDirectory, "Dictionaries")).FullName, "not-a-word-list");
                        File.Create(path).Close();
                        File.Delete(path);
                        return BaseDirectory; // user installation
                    }
                    catch
                    {
                        // ignored
                    }
                }
                Directory.CreateDirectory(Path.Combine(appDataRoamingPath, "Dictionaries"));
                return appDataRoamingPath + Path.DirectorySeparatorChar; // system installation
            }

            var installerPath = GetInstallerPath();
            var hasUninstallFiles = Directory.GetFiles(BaseDirectory, "unins*.*").Length > 0;
            var hasDictionaryFolder = Directory.Exists(Path.Combine(BaseDirectory, "Dictionaries"));

            if ((installerPath == null || !installerPath.TrimEnd(Path.DirectorySeparatorChar).Equals(BaseDirectory.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
                && !hasUninstallFiles && (hasDictionaryFolder || !Directory.Exists(Path.Combine(appDataRoamingPath, "Dictionaries"))))
            {
                return BaseDirectory;
            }

            if (Directory.Exists(appDataRoamingPath))
            {
                return appDataRoamingPath + Path.DirectorySeparatorChar;
            }

            try
            {
                Directory.CreateDirectory(appDataRoamingPath);
                Directory.CreateDirectory(Path.Combine(appDataRoamingPath, "Dictionaries"));
                return appDataRoamingPath + Path.DirectorySeparatorChar;
            }
            catch
            {
                throw new Exception("Please re-install Subtitle Edit (installer version)");
            }
        }

        private static string GetInstallerPath()
        {
            const string valueName = "InstallLocation";
            var value = RegistryUtil.GetValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\SubtitleEdit_is1", valueName);
            if (value != null && Directory.Exists(value))
            {
                return value;
            }

            value = RegistryUtil.GetValue(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\SubtitleEdit_is1", valueName);
            if (value != null && Directory.Exists(value))
            {
                return value;
            }

            return null;
        }

    }
}
