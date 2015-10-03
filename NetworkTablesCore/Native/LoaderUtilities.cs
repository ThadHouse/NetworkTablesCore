﻿using System;
using System.IO;
using System.Reflection;

namespace NetworkTables.Native
{
    enum OsType
    {
        Windows32,
        Windows64,
        Linux32,
        Linux64,
        RoboRio
    }
    internal static class LoaderUtilities
    {
        internal static OsType GetOsType()
        {
            var platform = (int)Environment.OSVersion.Platform;
            if (platform == 4 || platform == 6 || platform == 128)
            {
                //These 3 mean we are running on a unix based system
                if (Environment.Is64BitProcess)
                {
                    //We are 64 bit. RIO is not 64 bit, so we can force return.
                    return OsType.Linux64;
                }
                else
                {
                    //We need to check for the RIO
                    return File.Exists("/usr/local/frc/bin/frcRunRobot.sh") ? OsType.RoboRio : OsType.Linux32;
                }
            }
            else
            {
                //Assume we are on windows otherwise
                return Environment.Is64BitProcess ? OsType.Windows64 : OsType.Windows32;
            }
        }

        internal static bool CheckOsValid(OsType type)
        {
            switch (type)
            {
                case OsType.Windows32:
                    return true;
                case OsType.Windows64:
                    return true;
                case OsType.Linux32:
                    return false;
                case OsType.Linux64:
                    return false;
                case OsType.RoboRio:
                    return true;
                default:
                    return false;
            }
        }

        internal static string ExtractLibrary(OsType type)
        {
            string inputName;
            string outputName;
            switch (type)
            {
                case OsType.Windows32:
                    inputName = "NetworkTables.NativeLibraries.ntcore32.dll";
                    outputName = "ntcore.dlln";
                    break;
                case OsType.Windows64:
                    inputName = "NetworkTables.NativeLibraries.ntcore64.dll";
                    outputName = "ntcore.dlln";
                    break;
                case OsType.Linux32:
                    return null;
                case OsType.Linux64:
                    return null;
                case OsType.RoboRio:
                    inputName = "NetworkTables.NativeLibraries.libntcorerio.so";
                    outputName = "/home/lvuser/libntcore.so";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(inputName))
            {
                if (s == null || s.Length == 0)
                    return null;
                var bytes = new byte[(int)s.Length];
                s.Read(bytes, 0, (int)s.Length);

                if (File.Exists(outputName))
                    File.Delete(outputName);
                File.WriteAllBytes(outputName, bytes);
            }
            return outputName;

        }

        internal static IntPtr LoadLibrary(string dllLoc, OsType type, out ILibraryLoader loader)
        {
            switch (type)
            {
                case OsType.Windows32:
                case OsType.Windows64:
                    loader = new WindowsLibraryLoader();
                    return loader.LoadLibrary(dllLoc);
                case OsType.Linux32:
                case OsType.Linux64:
                case OsType.RoboRio:
                    loader = new RoboRioLibraryLoader();
                    return loader.LoadLibrary(dllLoc);
                default:
                    loader = null;
                    return IntPtr.Zero;
            }
        }
    }
}