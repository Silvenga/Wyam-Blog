using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using LibGit2Sharp;

namespace Wyam.SlightBlog.Git
{
    public class SetupNativePaths
    {
        public static string NativePlatform
        {
            get
            {
                var os = "any";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    os = "win";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    os = "osx";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    os = "debian.9";
                }

                var arch = RuntimeInformation.OSArchitecture.ToString().ToLower();

                var platform = os + "-" + arch;
                return platform;
            }
        }

        public static void SetupGit()
        {
            var nativeDirectory = Path.Combine(GetExecutingAssemblyDirectory(), "runtimes", NativePlatform, "native");

            Console.WriteLine($"NATIVE {nativeDirectory}");

            GlobalSettings.NativeLibraryPath = nativeDirectory;
        }

        private static string GetExecutingAssemblyDirectory()
        {
            string path = Assembly.GetExecutingAssembly().CodeBase;
            if (path.StartsWith("file:///"))
            {
                path = path.Substring("file://".Length).Replace('/', Path.DirectorySeparatorChar);
                Console.WriteLine(path);
            }
            else if (path.StartsWith("file://"))
            {
                path = "\\\\" + path.Substring(7).Replace('/', Path.DirectorySeparatorChar);
            }

            return Path.GetDirectoryName(path);
        }
    }
}