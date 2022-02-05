#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class WorkshopReadCommunityItem
    {
        public string title;
        public string description;
        public AppId_t targetApp;
        public PublishedFileId_t fileId;
        public CSteamID author;
        public DateTime createdOn;
        public DateTime lastUpdated;
        public uint upVotes;
        public uint downVotes;
        public float voteScore;
        public bool isBanned;
        public bool isTagsTruncated;
        public bool isSubscribed;
        public int fileSize;
        [EnumFlags]
        public EItemState StateFlags;

        public ERemoteStoragePublishedFileVisibility Visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate;
        public List<string> Tags = new List<string>();
        public Texture2D previewImage;
        public string PreviewImageLocation;
        public SteamUGCDetails_t SourceItemDetails;

        public UnityEvent PreviewImageUpdated = new UnityEvent();

        public CallResult<RemoteStorageDownloadUGCResult_t> m_RemoteStorageDownloadUGCResult;

        public WorkshopReadCommunityItem(SteamUGCDetails_t itemDetails)
        {
            SourceItemDetails = itemDetails;

            if (itemDetails.m_eFileType != EWorkshopFileType.k_EWorkshopFileTypeCommunity)
            {
                Debug.LogWarning("HeathenWorkshopReadItem is designed to display File Type = Community Item, this item is not a community item and may not load correctly.");
            }

            m_RemoteStorageDownloadUGCResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(HandleUGCDownload);

            targetApp = itemDetails.m_nConsumerAppID;
            fileId = itemDetails.m_nPublishedFileId;
            title = itemDetails.m_rgchTitle;
            description = itemDetails.m_rgchDescription;
            Visibility = itemDetails.m_eVisibility;
            author = new CSteamID(itemDetails.m_ulSteamIDOwner);
            createdOn = SteamSettings.ConvertUnixDate(itemDetails.m_rtimeCreated);
            lastUpdated = SteamSettings.ConvertUnixDate(itemDetails.m_rtimeUpdated);
            upVotes = itemDetails.m_unVotesUp;
            downVotes = itemDetails.m_unVotesDown;
            voteScore = itemDetails.m_flScore;
            isBanned = itemDetails.m_bBanned;
            isTagsTruncated = itemDetails.m_bTagsTruncated;
            fileSize = itemDetails.m_nFileSize;
            Visibility = itemDetails.m_eVisibility;
            Tags.AddRange(itemDetails.m_rgchTags.Split(','));
            uint state = SteamUGC.GetItemState(fileId);
            StateFlags = (EItemState)state;

            isSubscribed = SteamSettings.WorkshopItemStateHasFlag(StateFlags, EItemState.k_EItemStateSubscribed);

            if (itemDetails.m_nPreviewFileSize > 0)
            {
                var previewCall = SteamRemoteStorage.UGCDownload(itemDetails.m_hPreviewFile, 1);
                m_RemoteStorageDownloadUGCResult.Set(previewCall, HandleUGCDownloadPreviewFile);
            }
            else
            {
                Debug.LogWarning("Item [" + title + "] has no preview file!");
            }
        }

        /// <summary>
        /// Generic handler useful for testing and debugging
        /// </summary>
        /// <param name="param"></param>
        /// <param name="bIOFailure"></param>
        private void HandleUGCDownload(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                Debug.LogError("UGC Download generic handler loaded without failure.");
            }
            else
            {
                Debug.LogError("UGC Download request failed.");
            }
        }

        private void HandleUGCDownloadPreviewFile(RemoteStorageDownloadUGCResult_t param, bool bIOFailure)
        { 
            if (!bIOFailure)
            {
                if (param.m_eResult == EResult.k_EResultOK)
                {
                    byte[] imageBuffer = new byte[param.m_nSizeInBytes];
                    var count = SteamRemoteStorage.UGCRead(param.m_hFile, imageBuffer, param.m_nSizeInBytes, 0, EUGCReadAction.k_EUGCRead_ContinueReadingUntilFinished);
                    //Initalize the image, the LoadImage call will resize as required
                    previewImage = new Texture2D(2, 2);
                    previewImage.LoadImage(imageBuffer);
                    PreviewImageLocation = param.m_pchFileName;
                }
                else
                {
                    Debug.LogError("UGC Download: unexpected result state: " + param.m_eResult.ToString() + "\nImage will not be loaded.");
                }
            }
            else
            {
                Debug.LogError("UGC Download request failed.");
            }
        }
    }
}
#endif