#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;
using System;
using UnityEngine;

namespace HeathenEngineering.SteamworksIntegration.API
{
    /// <summary>
    /// Exposes a wide range of information and actions for applications and Downloadable Content (DLC).
    /// </summary>
    /// <remarks>
    /// https://partner.steamgames.com/doc/api/ISteamApps
    /// </remarks>
    public static class App
    {
        public static class Client
        {
            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            static void Init()
            {
                eventDlcInstalled = new DlcInstalledEvent();
                eventNewUrlLaunchParameters = new NewUrlLaunchParametersEvent();
            }

            /// <summary>
            /// Triggered after the current user gains ownership of DLC and that DLC is installed.
            /// </summary>
            public static DlcInstalledEvent EventDlcInstalled
            { 
                get
                {
                    if (m_DlcInstalled_t == null)
                        m_DlcInstalled_t = Callback<DlcInstalled_t>.Create(eventDlcInstalled.Invoke);

                    return eventDlcInstalled;
                }
            }

            /// <summary>
            /// Posted after the user executes a steam url with command line or query parameters such as steam://run/<appid>//?param1=value1;param2=value2;param3=value3; while the game is already running. The new params can be queried with GetLaunchCommandLine and GetLaunchQueryParam.
            /// </summary>
            public static NewUrlLaunchParametersEvent EventNewUrlLaunchParameters
            {
                get
                {
                    if (m_NewUrlLaunchParameters_t == null)
                        m_NewUrlLaunchParameters_t = Callback<NewUrlLaunchParameters_t>.Create(eventNewUrlLaunchParameters.Invoke);

                    return eventNewUrlLaunchParameters;
                }
            }

            private static DlcInstalledEvent eventDlcInstalled = new DlcInstalledEvent();
            private static NewUrlLaunchParametersEvent eventNewUrlLaunchParameters = new NewUrlLaunchParametersEvent();

            private static CallResult<FileDetailsResult_t> m_FileDetailResult_t;

            private static Callback<DlcInstalled_t> m_DlcInstalled_t;
            private static Callback<NewUrlLaunchParameters_t> m_NewUrlLaunchParameters_t;

            /// <summary>
            /// Checks if the active user is subscribed to the current App ID.
            /// </summary>
            /// <remarks>
            /// NOTE: This will always return true if you're using Steam DRM or calling SteamAPI_RestartAppIfNecessary.
            /// </remarks>
            public static bool IsSubscribed => SteamApps.BIsSubscribed();
            /// <summary>
            /// Checks if the active user is accessing the current appID via a temporary Family Shared license owned by another user.
            /// </summary>
            public static bool IsSubscribedFromFamilySharing => SteamApps.BIsSubscribedFromFamilySharing();
            /// <summary>
            /// Checks if the user is subscribed to the current App ID through a free weekend.
            /// </summary>
            public static bool IsSubscribedFromFreeWeekend => SteamApps.BIsSubscribedFromFreeWeekend();
            /// <summary>
            /// Checks if the user has a VAC ban on their account
            /// </summary>
            public static bool IsVACBanned => SteamApps.BIsVACBanned();
            /// <summary>
            /// Gets the Steam ID of the original owner of the current app. If it's different from the current user then it is borrowed.
            /// </summary>
            public static UserData Owner => SteamApps.GetAppOwner();
            /// <summary>
            /// Returns a list of languages supported by the app
            /// </summary>
            public static string[] AvailableLanguages
            {
                get
                {
                    var list = SteamApps.GetAvailableGameLanguages();
                    return list.Split(',');
                }
            }
            /// <summary>
            /// Returns true if a beta branch is being used
            /// </summary>
            public static bool IsBeta => SteamApps.GetCurrentBetaName(out string _, 128);
            /// <summary>
            /// Returns the name of the beta branch being used if any
            /// </summary>
            public static string CurrentBetaName
            {
                get
                {
                    if (SteamApps.GetCurrentBetaName(out string name, 512))
                        return name;
                    else
                        return string.Empty;
                }
            }
            /// <summary>
            /// Gets the current language that the user has set
            /// </summary>
            public static string CurrentGameLanguage => SteamApps.GetCurrentGameLanguage();
            /// <summary>
            /// Returns the metadata for all available DLC
            /// </summary>
            public static DlcData[] Dlc
            {
                get
                {
                    var count = SteamApps.GetDLCCount();
                    if (count > 0)
                    {
                        var result = new DlcData[count];
                        for (int i = 0; i < count; i++)
                        {
                            if (SteamApps.BGetDLCDataByIndex(i, out AppId_t appid, out bool available, out string name, 512))
                            {
                                result[i] = new DlcData(appid, available, name);
                            }
                            else
                            {
                                Debug.LogWarning("Failed to fetch DLC at index [" + i.ToString() + "]");
                            }
                        }
                        return result;
                    }
                    else
                        return new DlcData[0];
                }
            }
            /// <summary>
            /// Checks whether the current App ID is for Cyber Cafes.
            /// </summary>
            public static bool IsCybercafe => SteamApps.BIsCybercafe();
            /// <summary>
            /// Checks if the license owned by the user provides low violence depots.
            /// </summary>
            public static bool IsLowViolence => SteamApps.BIsLowViolence();
            /// <summary>
            /// Gets the App ID of the current process.
            /// </summary>
            public static AppId_t Id => SteamUtils.GetAppID();
            /// <summary>
            /// Gets the buildid of this app, may change at any time based on backend updates to the game.
            /// </summary>
            public static int BuildId => SteamApps.GetAppBuildId();
            /// <summary>
            /// Gets the install folder for a specific AppID.
            /// </summary>
            public static string InstallDirectory
            {
                get
                {
                    SteamApps.GetAppInstallDir(SteamUtils.GetAppID(), out string folder, 2048);
                    return folder;
                }
            }
            /// <summary>
            /// Gets the number of DLC pieces for the current app.
            /// </summary>
            public static int DLCCount => SteamApps.GetDLCCount();
            /// <summary>
            /// Gets the command line if the game was launched via Steam URL, e.g. steam://run/&lt;appid&gt;//&lt;command line&gt;/. This method is preferable to launching with a command line via the operating system, which can be a security risk. In order for rich presence joins to go through this and not be placed on the OS command line, you must enable "Use launch command line" from the Installation &gt; General page on your app.
            /// </summary>
            public static string LaunchCommandLine
            {
                get
                {
                    if (
                SteamApps.GetLaunchCommandLine(out string commandline, 512) > 0)
                        return commandline;
                    else
                        return string.Empty;
                }
            }

            /// <summary>
            /// Checks if a specific app is installed.
            /// </summary>
            /// <remarks>
            /// The app may not actually be owned by the current user, they may have it left over from a free weekend, etc.
            /// This only works for base applications, not Downloadable Content(DLC). Use IsDlcInstalled for DLC instead.
            /// </remarks>
            /// <param name="appId"></param>
            /// <returns></returns>
            public static bool IsAppInstalled(AppId_t appId) => SteamApps.BIsAppInstalled(appId);
            /// <summary>
            /// Checks if the user owns a specific DLC and if the DLC is installed
            /// </summary>
            /// <param name="appId">The App ID of the DLC to check.</param>
            /// <returns></returns>
            public static bool IsDlcInstalled(AppId_t appId) => SteamApps.BIsDlcInstalled(appId);
            /// <summary>
            /// Gets the download progress for optional DLC.
            /// </summary>
            /// <param name="appId"></param>
            /// <param name="bytesDownloaded"></param>
            /// <param name="bytesTotal"></param>
            /// <returns></returns>
            public static bool GetDlcDownloadProgress(AppId_t appId, out ulong bytesDownloaded, out ulong bytesTotal) => SteamApps.GetDlcDownloadProgress(appId, out bytesDownloaded, out bytesTotal);
            /// <summary>
            /// Gets the install directory of the app if any
            /// </summary>
            /// <param name="appId"></param>
            /// <returns></returns>
            public static string GetAppInstallDirectory(AppId_t appId)
            {
                SteamApps.GetAppInstallDir(appId, out string folder, 2048);
                return folder;
            }
            /// <summary>
            /// Returns the collection of installed depots in mount order
            /// </summary>
            /// <param name="appId"></param>
            /// <returns></returns>
            public static DepotId_t[] InstalledDepots(AppId_t appId)
            {
                var results = new DepotId_t[256];
                var count = SteamApps.GetInstalledDepots(appId, results, 256);
                Array.Resize(ref results, (int)count);
                return results;
            }
            /// <summary>
            /// Parameter names starting with the character '@' are reserved for internal use and will always return an empty string. Parameter names starting with an underscore '_' are reserved for steam features -- they can be queried by the game, but it is advised that you not param names beginning with an underscore for your own features.
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public static string QueryLaunchParam(string key) => SteamApps.GetLaunchQueryParam(key);
            /// <summary>
            /// Install an optional DLC
            /// </summary>
            /// <param name="appId"></param>
            public static void InstallDLC(AppId_t appId) => SteamApps.InstallDLC(appId);
            /// <summary>
            /// Uninstall an optional DLC
            /// </summary>
            /// <param name="appId"></param>
            public static void UninstallDLC(AppId_t appId) => SteamApps.UninstallDLC(appId);
            /// <summary>
            /// Checks if the active user is subscribed to a specified appId.
            /// </summary>
            /// <param name="appId"></param>
            /// <returns></returns>
            public static bool IsSubscribedApp(AppId_t appId) => SteamApps.BIsSubscribedApp(appId);
            public static bool IsTimedTrial(out uint secondsAllowed, out uint secondsPlayed) => SteamApps.BIsTimedTrial(out secondsAllowed, out secondsPlayed);
            /// <summary>
            /// Gets the current beta branch name if any
            /// </summary>
            /// <param name="name">outputs the name of the current beta branch if any</param>
            /// <returns>True if the user is running from a beta branch</returns>
            public static bool GetCurrentBetaName(out string name) => SteamApps.GetCurrentBetaName(out name, 512);
            /// <summary>
            /// Gets the time of purchase of the specified app
            /// </summary>
            /// <param name="appId"></param>
            /// <returns></returns>
            public static DateTime GetEarliestPurchaseTime(AppId_t appId)
            {
                var secondsSince1970 = SteamApps.GetEarliestPurchaseUnixTime(appId);
                return new DateTime(1970, 1, 1).AddSeconds(secondsSince1970);
            }
            /// <summary>
            /// Asynchronously retrieves metadata details about a specific file in the depot manifest.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="callback"></param>
            public static void GetFileDetails(string name, Action<FileDetailsResult_t, bool> callback)
            {
                if (callback == null)
                    return;

                if (m_FileDetailResult_t == null)
                    m_FileDetailResult_t = CallResult<FileDetailsResult_t>.Create();

                var handle = SteamApps.GetFileDetails(name);
                m_FileDetailResult_t.Set(handle, callback.Invoke);
            }
            /// <summary>
            /// If you detect the game is out-of-date (for example, by having the client detect a version mismatch with a server), you can call use MarkContentCorrupt to force a verify, show a message to the user, and then quit.
            /// </summary>
            /// <param name="missingFilesOnly"></param>
            /// <returns></returns>
            public static bool MarkContentCorrupt(bool missingFilesOnly) => SteamApps.MarkContentCorrupt(missingFilesOnly);

        }
    }
}
#endif