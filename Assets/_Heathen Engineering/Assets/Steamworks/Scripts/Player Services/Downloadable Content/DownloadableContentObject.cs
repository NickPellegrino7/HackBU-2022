#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System;
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Represents the in game definition of a Steamworks DLC app.
    /// </summary>
    /// <remarks>Steamworks DLC or Downloadable Content is defined on the Steamworks API in your Steamworks Portal.
    /// Please carfully read <a href="https://partner.steamgames.com/doc/store/application/dlc">https://partner.steamgames.com/doc/store/application/dlc</a> before designing features are this concept.</remarks>
    [CreateAssetMenu(menuName = "Steamworks/Downloadable Content Object")]
    public class DownloadableContentObject : ScriptableObject
    {
        /// <summary>
        /// The <see cref="AppId_t"/> assoceated with this DLC
        /// </summary>
        public AppId_t AppId;
        /// <summary>
        /// Is the current user 'subscribed' to this DLC.
        /// This indicates that the current user has right/license this DLC or not.
        /// </summary>
        public bool IsSubscribed = false;
        /// <summary>
        /// Is this DLC currently installed.
        /// </summary>
        public bool IsDlcInstalled = false;
        /// <summary>
        /// Is this DLC currently downloading.
        /// </summary>
        public bool IsDownloading = false;

        /// <summary>
        /// Calls GetIsSubscribed, GetIsInstalled and GetDownloadProgress to update the 3 status values.
        /// This should not be done frequently
        /// </summary>
        public void UpdateStatus()
        {
            if (AppId == AppId_t.Invalid)
            {
                IsSubscribed = false;
                IsDlcInstalled = false;
                IsDownloading = false;
            }

            GetIsSubscribed();
            GetIsInstalled();
            GetDownloadProgress();
        }

        /// <summary>
        /// Updates the IsSubscribed member and Checks with Steamworks to get the current Subscribed state of the DLC
        /// </summary>
        /// <returns></returns>
        public bool GetIsSubscribed()
        {
            IsSubscribed = SteamApps.BIsSubscribedApp(AppId);
            return IsSubscribed;
        }

        /// <summary>
        /// Updates the IsDlcInstalled member and Checks with Steamworks to get the current Installed state of the DLC
        /// </summary>
        /// <returns></returns>
        public bool GetIsInstalled()
        {
            IsDlcInstalled = SteamApps.BIsDlcInstalled(AppId);
            return IsDlcInstalled;
        }

        /// <summary>
        /// Returns the install location of the DLC
        /// </summary>
        /// <returns></returns>
        public string GetInstallDirectory()
        {
            string path;
            if(SteamApps.GetAppInstallDir(AppId, out path, 2048) > 0)
            {
                return path;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Updates the IsDownloading member and Returns the download progress of the DLC if any
        /// </summary>
        /// <returns></returns>
        public float GetDownloadProgress()
        {
            ulong current;
            ulong total;
            IsDownloading = SteamApps.GetDlcDownloadProgress(AppId, out current, out total);
            if (IsDownloading)
            {
                return Convert.ToSingle(current / (double)total);
            }
            else
                return 0f;
        }

        /// <summary>
        /// Gets the time of purchase
        /// </summary>
        /// <returns></returns>
        public DateTime GetEarliestPurchaseTime()
        {
            var val = SteamApps.GetEarliestPurchaseUnixTime(AppId);
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(val);
            return dateTime;
        }

        /// <summary>
        /// Installs the DLC
        /// </summary>
        public void InstallDLC()
        {
            SteamApps.InstallDLC(AppId);
        }

        /// <summary>
        /// Uninstalls the DLC
        /// </summary>
        public void UninstallDLC()
        {
            SteamApps.UninstallDLC(AppId);
        }

        /// <summary>
        /// Opens the store page to the DLC
        /// </summary>
        /// <param name="flag"></param>
        public void OpenStore(EOverlayToStoreFlag flag = EOverlayToStoreFlag.k_EOverlayToStoreFlag_None)
        {
            SteamFriends.ActivateGameOverlayToStore(AppId, flag);
        }
    }
}
#endif