#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Simplified interface for working with Steamworks UGC aka Workshop.
    /// </summary>
    /// <remarks>
    /// Valve's Steamworks User Generated Content is a wide reaching topic covering not just Steamworks Workshop.
    /// In addition to serving as a simple mod repository UGC items can be used with leaderboards or by players to upload videos, screen shots and more.
    /// The methods provided in this class are meant to simplify working with the ISteamUGC interface, in particular all callbacks and callresults have been wrapped as UnityEvents and can easily be registered in code via
    /// <code>
    /// SteamworksUserGeneratedContent.OnItemDownloaded.AddListener(HandleUgcItemDownloaded);
    /// </code>
    /// Common tasks such as creating a new UGC item as a Steamworks Workshop entry have been simplified into single calls ... note the orinal step by step calls as defined in Valve's documentation are also available.
    /// It is strongly recomended that you read the available forums and documenaiton from Valve on the use and best practices around Steamworks UGC before commiting a design.
    /// </remarks>
    public class UserGeneratedContentTools
    {
        private CallResult<AddAppDependencyResult_t> m_AddAppDependencyResults;
        private CallResult<AddUGCDependencyResult_t> m_AddUGCDependencyResults;
        private CallResult<UserFavoriteItemsListChanged_t> m_UserFavoriteItemsListChanged;
        private CallResult<CreateItemResult_t> m_CreatedItem;
        private CallResult<DeleteItemResult_t> m_DeleteItem;
        private Callback<DownloadItemResult_t> m_DownloadItem;
        private CallResult<GetAppDependenciesResult_t> m_AppDependenciesResult;
        private CallResult<GetUserItemVoteResult_t> m_GetUserItemVoteResult;
        private CallResult<RemoveAppDependencyResult_t> m_RemoveAppDependencyResult;
        private CallResult<RemoveUGCDependencyResult_t> m_RemoveDependencyResult;
        private CallResult<SteamUGCRequestUGCDetailsResult_t> m_SteamUGCRequestUGCDetailsResult;
        private CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;
        private CallResult<SetUserItemVoteResult_t> m_SetUserItemVoteResult;
        private CallResult<StartPlaytimeTrackingResult_t> m_StartPlaytimeTrackingResult;
        private CallResult<StopPlaytimeTrackingResult_t> m_StopPlaytimeTrackingResult;
        private CallResult<SubmitItemUpdateResult_t> m_SubmitItemUpdateResult;
        private CallResult<RemoteStorageSubscribePublishedFileResult_t> m_RemoteStorageSubscribePublishedFileResult;
        private CallResult<RemoteStorageUnsubscribePublishedFileResult_t> m_RemoteStorageUnsubscribePublishedFileResult;

        #region Events
        /// <summary>
        /// Occures when a UGC item is downloaded
        /// </summary>
        [HideInInspector]
        public UnityWorkshopDownloadedItemResultEvent evtItemDownloaded;
        /// <summary>
        /// Occures when a UGC item is created
        /// </summary>
        [HideInInspector]
        public UnityWorkshopItemCreatedEvent evtItemCreated;
        /// <summary>
        /// Occures when a UGC item create is requested but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopItemCreatedEvent evtItemCreateFailed;
        /// <summary>
        /// Occures when a UGC item is deleted
        /// </summary>
        [HideInInspector]
        public UnityWorkshopItemDeletedEvent evtItemDeleted;
        /// <summary>
        /// Occures when a UGC item delete is requested but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopItemDeletedEvent evtItemDeleteFailed;
        /// <summary>
        /// Occures when a UGC item favorite flag is changed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopFavoriteItemsListChangedEvent evtFavoriteItemsChanged;
        /// <summary>
        /// Occures when a UGC item favorite flag change is requested but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopFavoriteItemsListChangedEvent evtFavoriteItemsChangeFailed;
        /// <summary>
        /// Occures when a UGC item add app dependency is called
        /// </summary>
        [HideInInspector]
        public UnityWorkshopAddAppDependencyResultEvent evtAddedAppDependency;
        /// <summary>
        /// Occures when a UGC item add app dependency is called but fails
        /// </summary>
        [HideInInspector]
        public UnityWorkshopAddAppDependencyResultEvent evtAddAppDependencyFailed;
        /// <summary>
        /// Occures when a UGC item add dependency is called
        /// </summary>
        [HideInInspector]
        public UnityWorkshopAddDependencyResultEvent evtAddDependency;
        /// <summary>
        /// Occures when a UGC item add dependency is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopAddDependencyResultEvent evtAddDependencyFailed;
        /// <summary>
        /// Occures when a UGC item app dependency result is called and recieved
        /// </summary>
        [HideInInspector]
        public UnityWorkshopGetAppDependenciesResultEvent evtAppDependenciesResults;
        /// <summary>
        /// Occures when a UGC item app dependency result is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopGetAppDependenciesResultEvent evtAppDependenciesResultsFailed;
        /// <summary>
        /// Occures when a UGC item vote called and succesful
        /// </summary>
        [HideInInspector]
        public UnityWorkshopGetUserItemVoteResultEvent evtUserItemVoteResults;
        /// <summary>
        /// Occures when a UGC item vote called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopGetUserItemVoteResultEvent evtUserItemVoteResultsFailed;
        /// <summary>
        /// Occures when a UGC item remove app dependency is called
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoveAppDependencyResultEvent evtRemoveAppDependencyResults;
        /// <summary>
        /// Occures when a UGC item remove app dependency is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoveAppDependencyResultEvent evtRemoveAppDependencyResultsFailed;
        /// <summary>
        /// Occures when a UGC item remove dependency is called
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoveUGCDependencyResultEvent evtRemoveDependencyResults;
        /// <summary>
        /// Occures when a UGC item remove dependency is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoveUGCDependencyResultEvent evtRemoveDependencyResultsFailed;
        /// <summary>
        /// Occures when a UGC item details request is called
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSteamUGCRequestUGCDetailsResultEvent evtRequestDetailsResults;
        /// <summary>
        /// Occures when a UGC item details is requested but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSteamUGCRequestUGCDetailsResultEvent evtRequestDetailsResultsFailed;
        /// <summary>
        /// Occures when a UGC item query / search is completed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSteamUGCQueryCompletedEvent evtQueryCompelted;
        /// <summary>
        /// Occures when a UGC item query / search is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSteamUGCQueryCompletedEvent evtQueryCompeltedFailed;
        /// <summary>
        /// Occures when a UGC item set vote is called and returned
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSetUserItemVoteResultEvent evtSetUserItemVoteResult;
        /// <summary>
        /// Occures when a UGC item set vote is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSetUserItemVoteResultEvent evtSetUserItemVoteResultFailed;
        /// <summary>
        /// Occures when a UGC item start playtime tracking is called and returned
        /// </summary>
        [HideInInspector]
        public UnityWorkshopStartPlaytimeTrackingResultEvent evtStartPlaytimeTrackingResult;
        /// <summary>
        /// Occures when a UGC item start playtime tracking is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopStartPlaytimeTrackingResultEvent evtStartPlaytimeTrackingResultFailed;
        /// <summary>
        /// Occures when a UGC item stop playtime tracking is called and returned
        /// </summary>
        [HideInInspector]
        public UnityWorkshopStopPlaytimeTrackingResultEvent evtStopPlaytimeTrackingResult;
        /// <summary>
        /// Occures when a UGC item stop playtime tracking is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopStopPlaytimeTrackingResultEvent evtStopPlaytimeTrackingResultFailed;
        /// <summary>
        /// Occures when a UGC item submit item update is called and returned
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSubmitItemUpdateResultEvent evtSubmitItemUpdateResult;
        /// <summary>
        /// Occures when a UGC item submit item update is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopSubmitItemUpdateResultEvent evtSubmitItemUpdateResultFailed;
        /// <summary>
        /// Occures when a UGC subscribe is called and returned
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoteStorageSubscribePublishedFileResultEvent evtRemoteStorageSubscribeFileResult;
        /// <summary>
        /// Occures when a UGC subscribe is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoteStorageSubscribePublishedFileResultEvent evtRemoteStorageSubscribeFileResultFailed;
        /// <summary>
        /// Occures when a UGC unsubscribe is called and returned
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoteStorageUnsubscribePublishedFileResultEvent evtRemoteStorageUnsubscribeFileResult;
        /// <summary>
        /// Occures when a UGC unsubscribe is called but failed
        /// </summary>
        [HideInInspector]
        public UnityWorkshopRemoteStorageUnsubscribePublishedFileResultEvent evtRemoteStorageUnsubscribeFileResultFailed;
        #endregion

        #region Workshop System
        /// <summary>
        /// Used by the callback on <see cref="CreateItem(AppId_t, string, string, string, string, string, List{string}, EWorkshopFileType, ERemoteStoragePublishedFileVisibility, Action{OperationStatus})"/>
        /// </summary>
        /// <remarks>
        /// This object contains all of the relivent information for a create and update process.
        /// in addition to the <see cref="hasError"/> and <see cref="errorMessage"/> members the object caries with it the <see cref="ugcFileId"/> and the create and update result handles from Valve.
        /// </remarks>
        public class OperationStatus
        {
            public bool hasError;
            public string errorMessage;
            public bool ugcItemCreated;
            public bool ugcItemUpdated;
            public PublishedFileId_t ugcFileId;
            public CreateItemResult_t createItemResult;
            public SubmitItemUpdateResult_t submitItemUpdateResult;
            public UGCUpdateHandle_t updateHandle;
            public AppId_t targetApp;
            public EWorkshopFileType fileType;
            public string title;
            public string description;
            public ERemoteStoragePublishedFileVisibility visibility;
            public List<string> tags;
            public string contentFolderPath;
            public string previewImagePath;
        }

        private bool UGCReg = false;
        private Action<OperationStatus> createUpdatecallback;
        private string processingChangeNote;
        private bool processingCreateAndUpdate = false;
        private OperationStatus workingStatus = null;

        /// <summary>
        /// Creates a new UGC aka Workshop file and updates its content and attributes according to the provided inputs.
        /// </summary>/// <param name="targetApp">[Required] The App Id the resulting UGC item should be assoceated with</param>
        /// <param name="title">[Required] The title of the new UGC item</param>
        /// <param name="description">[Optional] The description to be applied to the new UGC item</param>
        /// <param name="changeNote">[Optional] The annotation to be applied to the update step.</param>
        /// <param name="contentFolderPath">[Required] The UNC path to the folder which contains the content of the UGC item. [NOTE] this is a folder path not a file path.</param>
        /// <param name="previewImagePath">[Required] The UNC path to the image file to be used as the preview ... this should be a jpg or png and should be less than 1mb in size</param>
        /// <param name="tags">[Optional] A list of tags to be assoceated with the new item</param>
        /// <param name="fileType">[Required] The type of UGC item to create ... k_EWorkshopFileTypeCommunity is a Workshop item such as a mod and is the most common to be used.</param>
        /// <param name="visibility">[Required] Sets the visibility of the new UGC item ... its common to start all new items as k_ERemoteStoragePublishedFileVisibilityPrivate</param>
        /// <param name="completionCallback">A function to be called when the process completes. This will be called rather the process is a success or failure and the content of the <see cref="OperationStatus"/> paramiter will contain details about the process.</param>
        /// <returns></returns>
        public bool CreateItem(AppId_t targetApp, string title, string description, string changeNote, string contentFolderPath, string previewImagePath, List<string> tags = null, EWorkshopFileType fileType = EWorkshopFileType.k_EWorkshopFileTypeCommunity, ERemoteStoragePublishedFileVisibility visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate, Action<OperationStatus> completionCallback = null)
        {
            if(processingCreateAndUpdate)
            {
                Debug.LogError("Create operation aborted, a create or update request is already being processed.");
                return false;
            }

            if (targetApp == AppId_t.Invalid)
            {
                Debug.LogError("Create operation aborted, the current AppId is invalid.");
                return false;
            }

            if (string.IsNullOrEmpty(title))
            {
                Debug.LogError("Create operation aborted, Title is null or empty and must have a value.");
                return false;
            }

            if (string.IsNullOrEmpty(contentFolderPath))
            {
                Debug.LogError("Create operation aborted, Content location is null or empty and must have a value.");
                return false;
            }
            else if (!Directory.Exists(contentFolderPath))
            {
                Debug.LogError("Create operation aborted, Content location does not exist or is not accessable.");
                return false;
            }

            if (string.IsNullOrEmpty(previewImagePath))
            {
                Debug.LogError("Create operation aborted, Preview image location is null or empty and must have a value.");
                return false;
            }
            else if(!previewImagePath.ToLower().EndsWith("png") && !previewImagePath.ToLower().EndsWith("jpg"))
            {
                Debug.LogError("Create operation aborted, Preview image should be of type jpg or png ... note that jpg is preferable in most cases and typically smaller in size.");
            }
            else if (File.Exists(previewImagePath))
            {
                Debug.LogError("Create operation aborted, Preview image location does not exist or is not accessable.");
                return false;
            }
            else if (new FileInfo(previewImagePath).Length / 1048576 >= 1)
            {
                Debug.LogError("Create operation aborted, Preview image is 1mb or greater in size and is not likely to be accepted for upload.");
                return false;
            }

            createUpdatecallback = completionCallback;
            processingChangeNote = changeNote;
            processingCreateAndUpdate = true;
            workingStatus = new OperationStatus
            {
                hasError = false,
                errorMessage = string.Empty,
                ugcItemCreated = false,
                ugcItemUpdated = false,
                title = title,
                targetApp = targetApp,
                description = description,
                contentFolderPath = contentFolderPath,
                previewImagePath = previewImagePath,
                visibility = visibility,
                tags = tags,
            };

            var call = SteamUGC.CreateItem(targetApp, fileType);
            m_CreatedItem.Set(call, HandleItemCreateProcessing);

            return true;
        }
        
        private bool UpdateItem(string changeNote)
        {
            workingStatus.updateHandle = SteamUGC.StartItemUpdate(workingStatus.targetApp, workingStatus.ugcFileId);

            if (!SteamUGC.SetItemTitle(workingStatus.updateHandle, workingStatus.title))
            {
                workingStatus.hasError = true;
                workingStatus.errorMessage = "Failed to update item title, item has not been updated.";
                Debug.LogError("UGC Update ... Failed to update item title, item has not been updated.");
                return false;
            }

            if (!string.IsNullOrEmpty(workingStatus.description))
            {
                if (!SteamUGC.SetItemDescription(workingStatus.updateHandle, workingStatus.description))
                {
                    workingStatus.hasError = true;
                    workingStatus.errorMessage = "Failed to update item description, item has not been updated.";
                    Debug.LogError("UGC Update ... Failed to update item description, item has not been updated.");
                    return false;
                }
            }

            if (!SteamUGC.SetItemVisibility(workingStatus.updateHandle, workingStatus.visibility))
            {
                workingStatus.hasError = true;
                workingStatus.errorMessage = "Failed to update item visibility, item has not been updated.";
                Debug.LogError("UGC Update ... Failed to update item visibility, item has not been updated.");
                return false;
            }

            if (workingStatus.tags != null)
            {
                if (!SteamUGC.SetItemTags(workingStatus.updateHandle, workingStatus.tags))
                {
                    workingStatus.hasError = true;
                    workingStatus.errorMessage = "Failed to update item tags, item has not been updated.";
                    Debug.LogError("UGC Update ... Failed to update item tags, item has not been updated.");
                    return false;
                }
            }

            if (!SteamUGC.SetItemContent(workingStatus.updateHandle, workingStatus.contentFolderPath))
            {
                workingStatus.hasError = true;
                workingStatus.errorMessage = "Failed to update item content location, item has not been updated.";
                Debug.LogError("UGC Update ... Failed to update item content location, item has not been updated.");
                return false;
            }

            if (!SteamUGC.SetItemPreview(workingStatus.updateHandle, workingStatus.previewImagePath))
            {
                workingStatus.hasError = true;
                workingStatus.errorMessage = "Failed to update item preview, item has not been updated.";
                Debug.LogError("UGC Update ... Failed to update item preview, item has not been updated.");
                return false;
            }

            var call = SteamUGC.SubmitItemUpdate(workingStatus.updateHandle, changeNote);
            m_SubmitItemUpdateResult.Set(call, HandleItemUpdatedProcess);

            return true;
        }

        private void HandleItemUpdatedProcess(SubmitItemUpdateResult_t param, bool bIOFailure)
        {
            workingStatus.submitItemUpdateResult = param;

            if (bIOFailure)
            {
                workingStatus.hasError = true;
                workingStatus.errorMessage = "Steamworks Client failed to submit item updates.";
                evtSubmitItemUpdateResultFailed.Invoke(param);

                if (createUpdatecallback != null)
                    createUpdatecallback.Invoke(workingStatus);
            }
            else
            {
                evtSubmitItemUpdateResult.Invoke(param);

                if (createUpdatecallback != null)
                    createUpdatecallback.Invoke(workingStatus);
            }
        }

        private void HandleItemCreateProcessing(CreateItemResult_t param, bool bIOFailure)
        {
            workingStatus.createItemResult = param;

            if (bIOFailure)
            {
                workingStatus.hasError = true;
                workingStatus.errorMessage = "Steamworks Client failed to create UGC item.";
                evtItemCreateFailed.Invoke(param);
                if (createUpdatecallback != null)
                    createUpdatecallback.Invoke(workingStatus);
            }
            else
            {
                workingStatus.ugcFileId = param.m_nPublishedFileId;
                evtItemCreated.Invoke(param);
            }

            if (processingCreateAndUpdate)
            {
                processingCreateAndUpdate = false;
                if (!UpdateItem(processingChangeNote) && createUpdatecallback != null)
                    createUpdatecallback.Invoke(workingStatus);

                processingChangeNote = string.Empty;
            }
        }

        /// <summary>
        /// Registeres the callbacks for the UGC aka Workshop system
        /// Note this is called by the HeathenWorkshopBrowser
        /// </summary>
        public void RegisterSystem()
        {
            if (UGCReg)
                return;

            UGCReg = true;
            m_AddAppDependencyResults = CallResult<AddAppDependencyResult_t>.Create(HandleAddAppDependencyResult);
            m_AddUGCDependencyResults = CallResult<AddUGCDependencyResult_t>.Create(HandleAddUGCDependencyResult);
            m_UserFavoriteItemsListChanged = CallResult<UserFavoriteItemsListChanged_t>.Create(HandleUserFavoriteItemsListChanged);
            m_CreatedItem = CallResult<CreateItemResult_t>.Create(HandleCreatedItem);
            m_DeleteItem = CallResult<DeleteItemResult_t>.Create(HandleDeleteItem);
            m_DownloadItem = Callback<DownloadItemResult_t>.Create(HandleDownloadedItem);
            m_AppDependenciesResult = CallResult<GetAppDependenciesResult_t>.Create(HandleGetAppDependenciesResults);
            m_GetUserItemVoteResult = CallResult<GetUserItemVoteResult_t>.Create(HandleGetUserItemVoteResult);
            m_RemoveAppDependencyResult = CallResult<RemoveAppDependencyResult_t>.Create(HandleRemoveAppDependencyResult);
            m_RemoveDependencyResult = CallResult<RemoveUGCDependencyResult_t>.Create(HandleRemoveDependencyResult);
            m_SteamUGCRequestUGCDetailsResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create(HandleRequestDetailsResult);
            m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(HandleQueryCompleted);
            m_SetUserItemVoteResult = CallResult<SetUserItemVoteResult_t>.Create(HandleSetUserItemVoteResult);
            m_StartPlaytimeTrackingResult = CallResult<StartPlaytimeTrackingResult_t>.Create(HandleStartPlaytimeTracking);
            m_StopPlaytimeTrackingResult = CallResult<StopPlaytimeTrackingResult_t>.Create(HandleStopPlaytimeTracking);
            m_SubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(HandleItemUpdateResult);
            m_RemoteStorageSubscribePublishedFileResult = CallResult<RemoteStorageSubscribePublishedFileResult_t>.Create(HandleSubscribeFileResult);
            m_RemoteStorageUnsubscribePublishedFileResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(HandleUnsubscribeFileResult);

            evtItemDownloaded = new UnityWorkshopDownloadedItemResultEvent();
            evtItemCreated = new UnityWorkshopItemCreatedEvent();
            evtItemCreateFailed = new UnityWorkshopItemCreatedEvent();
            evtItemDeleted = new UnityWorkshopItemDeletedEvent();
            evtItemDeleteFailed = new UnityWorkshopItemDeletedEvent();
            evtFavoriteItemsChanged = new UnityWorkshopFavoriteItemsListChangedEvent();
            evtFavoriteItemsChangeFailed = new UnityWorkshopFavoriteItemsListChangedEvent();
            evtAddedAppDependency = new UnityWorkshopAddAppDependencyResultEvent();
            evtAddAppDependencyFailed = new UnityWorkshopAddAppDependencyResultEvent();
            evtAddDependency = new UnityWorkshopAddDependencyResultEvent();
            evtAddDependencyFailed = new UnityWorkshopAddDependencyResultEvent();
            evtAppDependenciesResults = new UnityWorkshopGetAppDependenciesResultEvent();
            evtAppDependenciesResultsFailed = new UnityWorkshopGetAppDependenciesResultEvent();
            evtUserItemVoteResults = new UnityWorkshopGetUserItemVoteResultEvent();
            evtUserItemVoteResultsFailed = new UnityWorkshopGetUserItemVoteResultEvent();
            evtRemoveAppDependencyResults = new UnityWorkshopRemoveAppDependencyResultEvent();
            evtRemoveAppDependencyResultsFailed = new UnityWorkshopRemoveAppDependencyResultEvent();
            evtRemoveDependencyResults = new UnityWorkshopRemoveUGCDependencyResultEvent();
            evtRemoveDependencyResultsFailed = new UnityWorkshopRemoveUGCDependencyResultEvent();
            evtRequestDetailsResults = new UnityWorkshopSteamUGCRequestUGCDetailsResultEvent();
            evtRequestDetailsResultsFailed = new UnityWorkshopSteamUGCRequestUGCDetailsResultEvent();
            evtQueryCompelted = new UnityWorkshopSteamUGCQueryCompletedEvent();
            evtQueryCompeltedFailed = new UnityWorkshopSteamUGCQueryCompletedEvent();
            evtSetUserItemVoteResult = new UnityWorkshopSetUserItemVoteResultEvent();
            evtSetUserItemVoteResultFailed = new UnityWorkshopSetUserItemVoteResultEvent();
            evtStartPlaytimeTrackingResult = new UnityWorkshopStartPlaytimeTrackingResultEvent();
            evtStartPlaytimeTrackingResultFailed = new UnityWorkshopStartPlaytimeTrackingResultEvent();
            evtStopPlaytimeTrackingResult = new UnityWorkshopStopPlaytimeTrackingResultEvent();
            evtStopPlaytimeTrackingResultFailed = new UnityWorkshopStopPlaytimeTrackingResultEvent();
            evtSubmitItemUpdateResult = new UnityWorkshopSubmitItemUpdateResultEvent();
            evtSubmitItemUpdateResultFailed = new UnityWorkshopSubmitItemUpdateResultEvent();
            evtRemoteStorageSubscribeFileResult = new UnityWorkshopRemoteStorageSubscribePublishedFileResultEvent();
            evtRemoteStorageSubscribeFileResultFailed = new UnityWorkshopRemoteStorageSubscribePublishedFileResultEvent();
            evtRemoteStorageUnsubscribeFileResult = new UnityWorkshopRemoteStorageUnsubscribePublishedFileResultEvent();
            evtRemoteStorageUnsubscribeFileResultFailed = new UnityWorkshopRemoteStorageUnsubscribePublishedFileResultEvent();
        }

        /// <summary>
        /// Adds a dependency between the given item and the appid. This list of dependencies can be retrieved by calling GetAppDependencies. This is a soft-dependency that is displayed on the web. It is up to the application to determine whether the item can actually be used or not.
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="appId"></param>
        public void AddAppDependency(PublishedFileId_t fileId, AppId_t appId)
        {
            RegisterSystem();

            var call = SteamUGC.AddAppDependency(fileId, appId);
            m_AddAppDependencyResults.Set(call, HandleAddAppDependencyResult);
        }

        /// <summary>
        /// Adds a workshop item as a dependency to the specified item. If the nParentPublishedFileID item is of type k_EWorkshopFileTypeCollection, than the nChildPublishedFileID is simply added to that collection. Otherwise, the dependency is a soft one that is displayed on the web and can be retrieved via the ISteamUGC API using a combination of the m_unNumChildren member variable of the SteamUGCDetails_t struct and GetQueryUGCChildren.
        /// </summary>
        /// <param name="parentFileId"></param>
        /// <param name="childFileId"></param>
        public void AddDependency(PublishedFileId_t parentFileId, PublishedFileId_t childFileId)
        {
            RegisterSystem();

            var call = SteamUGC.AddDependency(parentFileId, childFileId);
            m_AddUGCDependencyResults.Set(call, HandleAddUGCDependencyResult);
        }

        /// <summary>
        /// Adds a excluded tag to a pending UGC Query. This will only return UGC without the specified tag.
        /// </summary>
        /// <param name="handle">The UGC query handle to customize.</param>
        /// <param name="tagName">The tag that must NOT be attached to the UGC to receive it.</param>
        /// <returns>true upon success. false if the UGC query handle is invalid, if the UGC query handle is from CreateQueryUGCDetailsRequest, or tagName was NULL.</returns>
        /// <remarks>This must be set before you send a UGC Query handle using SendQueryUGCRequest.</remarks>
        public bool AddExcludedTag(UGCQueryHandle_t handle, string tagName)
        {
            RegisterSystem();

            return SteamUGC.AddExcludedTag(handle, tagName);
        }

        /// <summary>
        /// Adds a key-value tag pair to an item. Keys can map to multiple different values (1-to-many relationship).
        /// Key names are restricted to alpha-numeric characters and the '_' character.
        /// Both keys and values cannot exceed 255 characters in length.
        /// Key-value tags are searchable by exact match only.
        /// </summary>
        /// <param name="handle">The workshop item update handle to customize.</param>
        /// <param name="key">The key to set on the item.</param>
        /// <param name="value">A value to map to the key.</param>
        /// <returns></returns>
        public bool AddItemKeyValueTag(UGCUpdateHandle_t handle, string key, string value)
        {
            RegisterSystem();

            return SteamUGC.AddItemKeyValueTag(handle, key, value);
        }

        /// <summary>
        /// Adds an additional preview file for the item.
        /// Then the format of the image should be one that both the web and the application(if necessary) can render, and must be under 1MB.Suggested formats include JPG, PNG and GIF.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="previewFile"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool AddItemPreviewFile(UGCUpdateHandle_t handle, string previewFile, EItemPreviewType type)
        {
            RegisterSystem();

            return SteamUGC.AddItemPreviewFile(handle, previewFile, type);
        }

        /// <summary>
        /// Adds an additional video preview from YouTube for the item.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="videoId">The YouTube video ID ... e.g jHgZh4GV9G0</param>
        /// <returns></returns>
        public bool AddItemPreviewVideo(UGCUpdateHandle_t handle, string videoId)
        {
            RegisterSystem();

            return SteamUGC.AddItemPreviewVideo(handle, videoId);
        }

        /// <summary>
        /// Adds workshop item to the users favorite list
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="fileId"></param>
        public void AddItemToFavorites(AppId_t appId, PublishedFileId_t fileId)
        {
            RegisterSystem();

            var call = SteamUGC.AddItemToFavorites(appId, fileId);
            m_UserFavoriteItemsListChanged.Set(call, HandleUserFavoriteItemsListChanged);
        }

        /// <summary>
        /// Adds a required key-value tag to a pending UGC Query. This will only return workshop items that have a key = pKey and a value = pValue.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddRequiredKeyValueTag(UGCQueryHandle_t handle, string key, string value)
        {
            RegisterSystem();

            return SteamUGC.AddRequiredKeyValueTag(handle, key, value);
        }

        /// <summary>
        /// Adds a required tag to a pending UGC Query. This will only return UGC with the specified tag.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public bool AddRequiredTag(UGCQueryHandle_t handle, string tagName)
        {
            RegisterSystem();

            return SteamUGC.AddRequiredTag(handle, tagName);
        }

        /// <summary>
        /// Creates an empty workshop Item
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="type"></param>
        public void CreateItem(AppId_t appId, EWorkshopFileType type)
        {
            RegisterSystem();

            var call = SteamUGC.CreateItem(appId, type);
            m_CreatedItem.Set(call, HandleCreatedItem);
        }

        /// <summary>
        /// Query for all matching UGC. You can use this to list all of the available UGC for your app.
        /// You must release the handle returned by this function by calling WorkshopReleaseQueryRequest when you are done with it!
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="matchingFileType"></param>
        /// <param name="creatorAppId"></param>
        /// <param name="consumerAppId"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public UGCQueryHandle_t CreateQueryAllRequest(EUGCQuery queryType, EUGCMatchingUGCType matchingFileType, AppId_t creatorAppId, AppId_t consumerAppId, uint page)
        {
            RegisterSystem();

            return SteamUGC.CreateQueryAllUGCRequest(queryType, matchingFileType, creatorAppId, consumerAppId, page);
        }

        /// <summary>
        /// Query for the details of specific workshop items
        /// You must release the handle returned by this function by calling WorkshopReleaseQueryRequest when you are done with it!
        /// </summary>
        /// <param name="fileIds">The list of workshop items to get the details for.</param>
        /// <param name="count">The number of items in the list</param>
        /// <returns></returns>
        public UGCQueryHandle_t CreateQueryDetailsRequest(PublishedFileId_t[] fileIds)
        {
            RegisterSystem();

            return SteamUGC.CreateQueryUGCDetailsRequest(fileIds, (uint)fileIds.GetLength(0));
        }

        /// <summary>
        /// Query for the details of specific workshop items
        /// You must release the handle returned by this function by calling WorkshopReleaseQueryRequest when you are done with it!
        /// </summary>
        /// <param name="fileIds"></param>
        /// <returns></returns>
        public UGCQueryHandle_t CreateQueryDetailsRequest(List<PublishedFileId_t> fileIds)
        {
            RegisterSystem();

            //Specifc to a List this would be most common with Unity developers
            return SteamUGC.CreateQueryUGCDetailsRequest(fileIds.ToArray(), (uint)fileIds.Count);
        }

        /// <summary>
        /// Query for the details of specific workshop items
        /// You must release the handle returned by this function by calling WorkshopReleaseQueryRequest when you are done with it!
        /// </summary>
        /// <param name="fileIds"></param>
        /// <returns></returns>
        public UGCQueryHandle_t CreateQueryDetailsRequest(IEnumerable<PublishedFileId_t> fileIds)
        {
            RegisterSystem();

            //If not an array and not a list but it is some collection of IEnumerable then create a list for it and use that ... not efficent but useful and should be used infrequently
            List<PublishedFileId_t> list = new List<PublishedFileId_t>(fileIds);
            return SteamUGC.CreateQueryUGCDetailsRequest(list.ToArray(), (uint)list.Count);
        }

        /// <summary>
        /// Query UGC associated with a user. You can use this to list the UGC the user is subscribed to amongst other things.
        /// You must release the handle returned by this function by calling WorkshopReleaseQueryRequest when you are done with it!
        /// </summary>
        /// <param name="accountId">The Account ID to query the UGC for. You can use CSteamID.GetAccountID to get the Account ID from a Steamworks ID.</param>
        /// <param name="listType">Used to specify the type of list to get.</param>
        /// <param name="matchingType">Used to specify the type of UGC queried for.</param>
        /// <param name="sortOrder">Used to specify the order that the list will be sorted in.</param>
        /// <param name="creatorAppId">This should contain the App ID of the app where the item was created. This may be different than nConsumerAppID if your item creation tool is a seperate App ID.</param>
        /// <param name="consumerAppId">This should contain the App ID for the current game or application. Do not pass the App ID of the workshop item creation tool if that is a separate App ID!</param>
        /// <param name="page">The page number of the results to receive. This should start at 1 on the first call.</param>
        /// <returns></returns>
        public UGCQueryHandle_t CreateQueryUserRequest(AccountID_t accountId, EUserUGCList listType, EUGCMatchingUGCType matchingType, EUserUGCListSortOrder sortOrder, AppId_t creatorAppId, AppId_t consumerAppId, uint page)
        {
            RegisterSystem();

            return SteamUGC.CreateQueryUserUGCRequest(accountId, listType, matchingType, sortOrder, creatorAppId, consumerAppId, page);
        }

        /// <summary>
        /// Frees a UGC query
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public bool ReleaseQueryRequest(UGCQueryHandle_t handle)
        {
            RegisterSystem();

            return SteamUGC.ReleaseQueryUGCRequest(handle);
        }

        /// <summary>
        /// Requests delete of a UGC item
        /// </summary>
        /// <param name="fileId"></param>
        public void DeleteItem(PublishedFileId_t fileId)
        {
            RegisterSystem();

            var call = SteamUGC.DeleteItem(fileId);
            m_DeleteItem.Set(call, HandleDeleteItem);
        }

        /// <summary>
        /// Request download of a UGC item
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="setHighPriority"></param>
        /// <returns></returns>
        public bool DownloadItem(PublishedFileId_t fileId, bool setHighPriority)
        {
            RegisterSystem();

            return SteamUGC.DownloadItem(fileId, setHighPriority);
        }

        /// <summary>
        /// Request the app dependencies of a UGC item
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetAppDependencies">https://partner.steamgames.com/doc/api/ISteamUGC#GetAppDependencies</see></para>
        /// </summary>
        /// <param name="fileId">The workshop item to get app dependencies for.</param>
        public void GetAppDependencies(PublishedFileId_t fileId)
        {
            RegisterSystem();

            var call = SteamUGC.GetAppDependencies(fileId);
            m_AppDependenciesResult.Set(call, HandleGetAppDependenciesResults);
        }

        /// <summary>
        /// Request the download informaiton of a UGC item
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetItemDownloadInfo">https://partner.steamgames.com/doc/api/ISteamUGC#GetItemDownloadInfo</see></para>
        /// </summary>
        /// <param name="fileId">The workshop item to get the download info for.</param>
        /// <param name="completion">The % complete e.g. 0.5 represents 50% complete</param>
        /// <returns>true if the download information was available; otherwise, false.</returns>
        public bool GetItemDownloadInfo(PublishedFileId_t fileId, out float completion)
        {
            RegisterSystem();

            ulong current;
            ulong total;
            var result = SteamUGC.GetItemDownloadInfo(fileId, out current, out total);
            if (result)
                completion = Convert.ToSingle(current / (double)total);
            else
                completion = 0;
            return result;
        }

        /// <summary>
        /// Request the installation informaiton of a UGC item
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetItemInstallInfo">https://partner.steamgames.com/doc/api/ISteamUGC#GetItemInstallInfo</see></para>
        /// </summary>
        /// <param name="fileId">The item to check</param>
        /// <param name="sizeOnDisk">The size of the item on the disk</param>
        /// <param name="folderPath">The path of the item on the disk</param>
        /// <param name="timeStamp">The date time stamp of the item</param>
        /// <returns>true if the workshop item is already installed.
        /// false in the following cases:
        /// The workshop item has no content.
        /// The workshop item is not installed.</returns>
        public bool GetItemInstallInfo(PublishedFileId_t fileId, out ulong sizeOnDisk, out string folderPath, out DateTime timeStamp)
        {
            RegisterSystem();

            uint iTimeStamp;
            var result = SteamUGC.GetItemInstallInfo(fileId, out sizeOnDisk, out folderPath, 1024, out iTimeStamp);
            timeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            timeStamp = timeStamp.AddSeconds(iTimeStamp);
            return result;
        }

        /// <summary>
        /// Request the installation informaiton of a UGC item
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetItemInstallInfo">https://partner.steamgames.com/doc/api/ISteamUGC#GetItemInstallInfo</see></para>
        /// </summary>
        /// <param name="fileId">The item to check</param>
        /// <param name="sizeOnDisk">The size of the item on the disk</param>
        /// <param name="folderPath">The path of the item on the disk</param>
        /// <param name="folderSize">The size of folder path ... this is the length of the path e.g. 1024 would cover a max length path</param>
        /// <param name="timeStamp">The date time stamp of the item</param>
        /// <returns>true if the workshop item is already installed.
        /// false in the following cases:
        /// folderSize is 0
        /// The workshop item has no content.
        /// The workshop item is not installed.</returns>
        public bool GetItemInstallInfo(PublishedFileId_t fileId, out ulong sizeOnDisk, out string folderPath, uint folderSize, out DateTime timeStamp)
        {
            RegisterSystem();

            uint iTimeStamp;
            var result = SteamUGC.GetItemInstallInfo(fileId, out sizeOnDisk, out folderPath, folderSize, out iTimeStamp);
            timeStamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            timeStamp = timeStamp.AddSeconds(iTimeStamp);
            return result;
        }

        /// <summary>
        /// Gets the current state of a workshop item on this client.
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetItemState">https://partner.steamgames.com/doc/api/ISteamUGC#GetItemState</see></para>
        /// </summary>
        /// <param name="fileId">The workshop item to get the state for.</param>
        /// <returns>Item State flags, use with WorkshopItemStateHasFlag and WorkshopItemStateHasAllFlags</returns>
        public EItemState GetItemState(PublishedFileId_t fileId)
        {
            RegisterSystem();

            return (EItemState)SteamUGC.GetItemState(fileId);
        }

        /// <summary>
        /// Checks if the 'checkFlag' value is in the 'value'
        /// </summary>
        /// <param name="value">The value to check if a state is contained within</param>
        /// <param name="checkflag">The state to see if it is contained within value</param>
        /// <returns>true if checkflag is contained within value</returns>
        public bool ItemStateHasFlag(EItemState value, EItemState checkflag)
        {
            RegisterSystem();

            return (value & checkflag) == checkflag;
        }

        /// <summary>
        /// Cheks if any of the 'checkflags' values are in the 'value'
        /// </summary>
        /// <param name="value">The value to check if a state is contained within</param>
        /// <param name="checkflag">The state to see if it is contained within value</param>
        /// <returns>true if checkflag is contained within value</returns>
        public bool ItemStateHasAllFlags(EItemState value, params EItemState[] checkflags)
        {
            RegisterSystem();

            foreach (var checkflag in checkflags)
            {
                if ((value & checkflag) != checkflag)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the progress of an item update.
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetItemUpdateProgress"/></para>
        /// </summary>
        /// <param name="handle">The update handle to get the progress for.</param>
        /// <param name="completion">The % completion e.g. 0.5 represents 50% complete</param>
        /// <returns></returns>
        public EItemUpdateStatus GetItemUpdateProgress(UGCUpdateHandle_t handle, out float completion)
        {
            RegisterSystem();

            ulong current;
            ulong total;
            var result = SteamUGC.GetItemUpdateProgress(handle, out current, out total);
            if (result != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
                completion = Convert.ToSingle(current / (double)total);
            else
                completion = 0;
            return result;
        }

        /// <summary>
        /// Gets the progress of an the item update
        /// </summary>
        /// <param name="completion"></param>
        /// <returns></returns>
        /// <remarks>
        /// This only works with the <see cref="CreateItem(AppId_t, string, string, string, string, string, List{string}, EWorkshopFileType, ERemoteStoragePublishedFileVisibility, Action{OperationStatus})"/> call.
        /// Manual <see cref="StartItemUpdate(AppId_t, PublishedFileId_t)"/> calls should use <see cref="GetItemUpdateProgress(UGCUpdateHandle_t, out float)"/> and must specify the update handle.
        /// </remarks>
        public EItemUpdateStatus GetItemUpdateProgress(out float completion)
        {
            RegisterSystem();
            if (workingStatus == null || workingStatus.updateHandle == UGCUpdateHandle_t.Invalid)
            {
                completion = 0;
                return EItemUpdateStatus.k_EItemUpdateStatusInvalid;
            }
            else
            {
                ulong current;
                ulong total;
                var result = SteamUGC.GetItemUpdateProgress(workingStatus.updateHandle, out current, out total);
                if (result != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
                    completion = Convert.ToSingle(current / (double)total);
                else
                    completion = 0;
                return result;
            }
        }

        /// <summary>
        /// Returns the number of subscribed UGC items
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetNumSubscribedItems"/></para>
        /// </summary>
        /// <returns>Returns 0 if called from a game server. else returns the number of subscribed items</returns>
        public uint GetNumSubscribedItems()
        {
            RegisterSystem();

            return SteamUGC.GetNumSubscribedItems();
        }

        /// <summary>
        /// Request an additional preview for a UGC item
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetQueryUGCAdditionalPreview"/></para>
        /// </summary>
        /// <param name="handle">The UGC query handle to get the results from.</param>
        /// <param name="index">The index of the item to get the details of.</param>
        /// <param name="previewIndex">The index of the additional preview to get the details of.</param>
        /// <param name="urlOrVideoId">Returns a URL or Video ID by copying it into this string.</param>
        /// <param name="urlOrVideoSize">The size of pchURLOrVideoID in bytes.</param>
        /// <param name="fileName">Returns the original file name. May be set to NULL to not receive this.</param>
        /// <param name="fileNameSize">The size of pchOriginalFileName in bytes.</param>
        /// <param name="type">The type of preview that was returned.</param>
        /// <returns>true upon success, indicates that pchURLOrVideoID and pPreviewType have been filled out.
        /// Otherwise, false if the UGC query handle is invalid, the index is out of bounds, or previewIndex is out of bounds.</returns>
        public bool GetQueryUGCAdditionalPreview(UGCQueryHandle_t handle, uint index, uint previewIndex, out string urlOrVideoId, uint urlOrVideoSize, string fileName, uint fileNameSize, out EItemPreviewType type)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCAdditionalPreview(handle, index, previewIndex, out urlOrVideoId, urlOrVideoSize, out fileName, fileNameSize, out type);
        }

        /// <summary>
        /// Request the child items of a given UGC item
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetQueryUGCChildren"/></para>
        /// </summary>
        /// <param name="handle">The UGC query handle to get the results from.</param>
        /// <param name="index">The index of the item to get the details of.</param>
        /// <param name="fileIds">Returns the UGC children by setting this array.</param>
        /// <param name="maxEntries">The length of pvecPublishedFileID.</param>
        /// <returns>true upon success, indicates that pvecPublishedFileID has been filled out.
        /// Otherwise, false if the UGC query handle is invalid or the index is out of bounds.</returns>
        public bool GetQueryUGCChildren(UGCQueryHandle_t handle, uint index, PublishedFileId_t[] fileIds, uint maxEntries)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCChildren(handle, index, fileIds, maxEntries);
        }

        /// <summary>
        /// Retrieve the details of a key-value tag associated with an individual workshop item after receiving a querying UGC call result.
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetQueryUGCKeyValueTag"/></para>
        /// </summary>
        /// <param name="handle">The UGC query handle to get the results from.</param>
        /// <param name="index">The index of the item to get the details of.</param>
        /// <param name="keyValueTagIndex">The index of the tag to get the details of.</param>
        /// <param name="key">Returns the key by copying it into this string.</param>
        /// <param name="value">Returns the value by copying it into this string.</param>
        /// <returns>true upon success, indicates that pchKey and pchValue have been filled out.
        /// Otherwise, false if the UGC query handle is invalid, the index is out of bounds, or keyValueTagIndex is out of bounds.</returns>
        public bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, out string key, string value)
        {
            RegisterSystem();

            var ret = SteamUGC.GetQueryUGCKeyValueTag(handle, index, keyValueTagIndex, out key, 2048, out value, 2048);
            key = key.Trim();
            value = value.Trim();
            return ret;
        }

        /// <summary>
        /// Retrieve the details of a key-value tag associated with an individual workshop item after receiving a querying UGC call result.
        /// <para><see cref="https://partner.steamgames.com/doc/api/ISteamUGC#GetQueryUGCKeyValueTag"/></para>
        /// </summary>
        /// <param name="handle">The UGC query handle to get the results from.</param>
        /// <param name="index">The index of the item to get the details of.</param>
        /// <param name="keyValueTagIndex">The index of the tag to get the details of.</param>
        /// <param name="key">Returns the key by copying it into this string.</param>
        /// <param name="keySize">The size of key in bytes.</param>
        /// <param name="value">Returns the value by copying it into this string.</param>
        /// <param name="valueSize">The size of value in bytes.</param>
        /// <returns>true upon success, indicates that pchKey and pchValue have been filled out.
        /// Otherwise, false if the UGC query handle is invalid, the index is out of bounds, or keyValueTagIndex is out of bounds.</returns>
        public bool GetQueryUGCKeyValueTag(UGCQueryHandle_t handle, uint index, uint keyValueTagIndex, out string key, uint keySize, string value, uint valueSize)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCKeyValueTag(handle, index, keyValueTagIndex, out key, keySize, out value, valueSize);
        }

        /// <summary>
        /// Request the metadata of a UGC item
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <param name="metadata"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool GetQueryUGCMetadata(UGCQueryHandle_t handle, uint index, out string metadata, uint size)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCMetadata(handle, index, out metadata, size);
        }

        /// <summary>
        /// Request the number of previews of a UGC item
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public uint GetQueryUGCNumAdditionalPreviews(UGCQueryHandle_t handle, uint index)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCNumAdditionalPreviews(handle, index);
        }

        /// <summary>
        /// Request the number of key value tags for a UGC item
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public uint GetQueryUGCNumKeyValueTags(UGCQueryHandle_t handle, uint index)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCNumKeyValueTags(handle, index);
        }

        /// <summary>
        /// Get the preview URL of a UGC item
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <param name="URL"></param>
        /// <param name="urlSize"></param>
        /// <returns></returns>
        public bool GetQueryUGCPreviewURL(UGCQueryHandle_t handle, uint index, out string URL, uint urlSize)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCPreviewURL(handle, index, out URL, urlSize);
        }

        /// <summary>
        /// Fetch the results of a UGC query
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool GetQueryUGCResult(UGCQueryHandle_t handle, uint index, out SteamUGCDetails_t details)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCResult(handle, index, out details);
        }

        /// <summary>
        /// Fetch the statistics of a UGC query
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <param name="statType"></param>
        /// <param name="statValue"></param>
        /// <returns></returns>
        public bool GetQueryUGCStatistic(UGCQueryHandle_t handle, uint index, EItemStatistic statType, out ulong statValue)
        {
            RegisterSystem();

            return SteamUGC.GetQueryUGCStatistic(handle, index, statType, out statValue);
        }

        /// <summary>
        /// Get the file IDs of all subscribed UGC items up to the array size
        /// </summary>
        /// <param name="fileIDs"></param>
        /// <param name="maxEntries"></param>
        /// <returns></returns>
        public uint GetSubscribedItems(PublishedFileId_t[] fileIDs, uint maxEntries)
        {
            RegisterSystem();

            return SteamUGC.GetSubscribedItems(fileIDs, maxEntries);
        }

        public List<PublishedFileId_t> GetSubscribedItems()
        {
            var count = GetNumSubscribedItems();
            var fileIds = new PublishedFileId_t[count];
            if (GetSubscribedItems(fileIds, count) > 0)
            {
                var results = new List<PublishedFileId_t>(fileIds);
                return results;
            }
            else
                return null;
        }
        
        /// <summary>
        /// Get the item vote value of a UGC item
        /// </summary>
        /// <param name="fileId"></param>
        public void GetUserItemVote(PublishedFileId_t fileId)
        {
            RegisterSystem();

            var call = SteamUGC.GetUserItemVote(fileId);
            m_GetUserItemVoteResult.Set(call, HandleGetUserItemVoteResult);
        }

        /// <summary>
        /// Request the removal of app dependency from a UGC item
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="appId"></param>
        public void RemoveAppDependency(PublishedFileId_t fileId, AppId_t appId)
        {
            RegisterSystem();

            var call = SteamUGC.RemoveAppDependency(fileId, appId);
            m_RemoveAppDependencyResult.Set(call, HandleRemoveAppDependencyResult);
        }

        /// <summary>
        /// Request the removal of a dependency from a UGC item
        /// </summary>
        /// <param name="parentFileId"></param>
        /// <param name="childFileId"></param>
        public void RemoveDependency(PublishedFileId_t parentFileId, PublishedFileId_t childFileId)
        {
            RegisterSystem();

            var call = SteamUGC.RemoveDependency(parentFileId, childFileId);
            m_RemoveDependencyResult.Set(call, HandleRemoveDependencyResult);
        }

        /// <summary>
        /// Removes the UGC item from user favorites
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="fileId"></param>
        public void RemoveItemFromFavorites(AppId_t appId, PublishedFileId_t fileId)
        {
            RegisterSystem();

            var call = SteamUGC.RemoveItemFromFavorites(appId, fileId);
            m_UserFavoriteItemsListChanged.Set(call, HandleUserFavoriteItemsListChanged);
        }

        /// <summary>
        /// Remove UGC item key value tags
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveItemKeyValueTags(UGCUpdateHandle_t handle, string key)
        {
            RegisterSystem();

            return SteamUGC.RemoveItemKeyValueTags(handle, key);
        }

        /// <summary>
        /// Removes UGC item preview
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveItemPreview(UGCUpdateHandle_t handle, uint index)
        {
            RegisterSystem();

            return SteamUGC.RemoveItemPreview(handle, index);
        }

        /// <summary>
        /// Requests details of a UGC item
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="maxAgeSeconds"></param>
        public void RequestDetails(PublishedFileId_t fileId, uint maxAgeSeconds)
        {
            RegisterSystem();

            var call = SteamUGC.RequestUGCDetails(fileId, maxAgeSeconds);
            m_SteamUGCRequestUGCDetailsResult.Set(call, HandleRequestDetailsResult);
        }

        /// <summary>
        /// Sends a UGC query
        /// </summary>
        /// <param name="handle"></param>
        public void SendQueryUGCRequest(UGCQueryHandle_t handle)
        {
            RegisterSystem();

            var call = SteamUGC.SendQueryUGCRequest(handle);
            m_SteamUGCQueryCompleted.Set(call, HandleQueryCompleted);
        }

        /// <summary>
        /// Set allow cached responce
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="maxAgeSeconds"></param>
        /// <returns></returns>
        public bool SetAllowCachedResponse(UGCQueryHandle_t handle, uint maxAgeSeconds)
        {
            RegisterSystem();

            return SteamUGC.SetAllowCachedResponse(handle, maxAgeSeconds);
        }

        /// <summary>
        /// Set cloud file name filter
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool SetCloudFileNameFilter(UGCQueryHandle_t handle, string fileName)
        {
            RegisterSystem();

            return SteamUGC.SetCloudFileNameFilter(handle, fileName);
        }

        /// <summary>
        /// Set item content path
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool SetItemContent(UGCUpdateHandle_t handle, string folder)
        {
            RegisterSystem();

            return SteamUGC.SetItemContent(handle, folder);
        }

        /// <summary>
        /// Set item description
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public bool SetItemDescription(UGCUpdateHandle_t handle, string description)
        {
            RegisterSystem();

            return SteamUGC.SetItemDescription(handle, description);
        }

        /// <summary>
        /// Set item metadata
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public bool SetItemMetadata(UGCUpdateHandle_t handle, string metadata)
        {
            RegisterSystem();

            return SteamUGC.SetItemMetadata(handle, metadata);
        }

        /// <summary>
        /// Set item preview
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="previewFile"></param>
        /// <returns></returns>
        public bool SetItemPreview(UGCUpdateHandle_t handle, string previewFile)
        {
            RegisterSystem();

            return SteamUGC.SetItemPreview(handle, previewFile);
        }

        /// <summary>
        /// Set item tags
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool SetItemTags(UGCUpdateHandle_t handle, List<string> tags)
        {
            RegisterSystem();

            return SteamUGC.SetItemTags(handle, tags);
        }

        /// <summary>
        /// Set item title
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public bool SetItemTitle(UGCUpdateHandle_t handle, string title)
        {
            RegisterSystem();

            return SteamUGC.SetItemTitle(handle, title);
        }

        /// <summary>
        /// Set item update language
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool SetItemUpdateLanguage(UGCUpdateHandle_t handle, string language)
        {
            RegisterSystem();

            return SteamUGC.SetItemUpdateLanguage(handle, language);
        }

        /// <summary>
        /// Set item visibility
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="visibility"></param>
        /// <returns></returns>
        public bool SetItemVisibility(UGCUpdateHandle_t handle, ERemoteStoragePublishedFileVisibility visibility)
        {
            RegisterSystem();

            return SteamUGC.SetItemVisibility(handle, visibility);
        }

        /// <summary>
        /// Set item langauge
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool SetLanguage(UGCQueryHandle_t handle, string language)
        {
            RegisterSystem();

            return SteamUGC.SetLanguage(handle, language);
        }

        /// <summary>
        /// Set match any tag
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="anyTag"></param>
        /// <returns></returns>
        public bool SetMatchAnyTag(UGCQueryHandle_t handle, bool anyTag)
        {
            RegisterSystem();

            return SteamUGC.SetMatchAnyTag(handle, anyTag);
        }

        /// <summary>
        /// Set ranked by trend days
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public bool SetRankedByTrendDays(UGCQueryHandle_t handle, uint days)
        {
            RegisterSystem();

            return SteamUGC.SetRankedByTrendDays(handle, days);
        }

        /// <summary>
        /// Set return additional previews
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="additionalPreviews"></param>
        /// <returns></returns>
        public bool SetReturnAdditionalPreviews(UGCQueryHandle_t handle, bool additionalPreviews)
        {
            RegisterSystem();

            return SteamUGC.SetReturnAdditionalPreviews(handle, additionalPreviews);
        }

        /// <summary>
        /// Set return childre
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="returnChildren"></param>
        /// <returns></returns>
        public bool SetReturnChildren(UGCQueryHandle_t handle, bool returnChildren)
        {
            RegisterSystem();

            return SteamUGC.SetReturnChildren(handle, returnChildren);
        }

        /// <summary>
        /// Set return key value tags
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool SetReturnKeyValueTags(UGCQueryHandle_t handle, bool tags)
        {
            RegisterSystem();

            return SteamUGC.SetReturnKeyValueTags(handle, tags);
        }

        /// <summary>
        /// SEt return long description
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="longDescription"></param>
        /// <returns></returns>
        public bool SetReturnLongDescription(UGCQueryHandle_t handle, bool longDescription)
        {
            RegisterSystem();

            return SteamUGC.SetReturnLongDescription(handle, longDescription);
        }

        /// <summary>
        /// Set return metadata
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public bool SetReturnMetadata(UGCQueryHandle_t handle, bool metadata)
        {
            RegisterSystem();

            return SteamUGC.SetReturnMetadata(handle, metadata);
        }

        /// <summary>
        /// Set return IDs only
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="onlyIds"></param>
        /// <returns></returns>
        public bool SetReturnOnlyIDs(UGCQueryHandle_t handle, bool onlyIds)
        {
            RegisterSystem();

            return SteamUGC.SetReturnOnlyIDs(handle, onlyIds);
        }

        /// <summary>
        /// Set return playtime stats
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public bool SetReturnPlaytimeStats(UGCQueryHandle_t handle, uint days)
        {
            RegisterSystem();

            return SteamUGC.SetReturnPlaytimeStats(handle, days);
        }

        /// <summary>
        /// Set return total only
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="totalOnly"></param>
        /// <returns></returns>
        public bool SetReturnTotalOnly(UGCQueryHandle_t handle, bool totalOnly)
        {
            RegisterSystem();

            return SteamUGC.SetReturnTotalOnly(handle, totalOnly);
        }

        /// <summary>
        /// Set search text
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool SetSearchText(UGCQueryHandle_t handle, string text)
        {
            RegisterSystem();

            return SteamUGC.SetSearchText(handle, text);
        }

        /// <summary>
        /// Set user item vote
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="voteUp"></param>
        public void SetUserItemVote(PublishedFileId_t fileID, bool voteUp)
        {
            RegisterSystem();

            var call = SteamUGC.SetUserItemVote(fileID, voteUp);
            m_SetUserItemVoteResult.Set(call, HandleSetUserItemVoteResult);
        }

        /// <summary>
        /// Start item update
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public UGCUpdateHandle_t StartItemUpdate(AppId_t appId, PublishedFileId_t fileID)
        {
            RegisterSystem();

            return SteamUGC.StartItemUpdate(appId, fileID);
        }

        /// <summary>
        /// Start playtime tracking
        /// </summary>
        /// <param name="fileIds"></param>
        /// <param name="count"></param>
        public void StartPlaytimeTracking(PublishedFileId_t[] fileIds, uint count)
        {
            RegisterSystem();

            var call = SteamUGC.StartPlaytimeTracking(fileIds, count);
            m_StartPlaytimeTrackingResult.Set(call, HandleStartPlaytimeTracking);
        }

        /// <summary>
        /// Stop playtime tracking
        /// </summary>
        /// <param name="fileIds"></param>
        /// <param name="count"></param>
        public void StopPlaytimeTracking(PublishedFileId_t[] fileIds, uint count)
        {
            RegisterSystem();

            var call = SteamUGC.StopPlaytimeTracking(fileIds, count);
            m_StopPlaytimeTrackingResult.Set(call, HandleStopPlaytimeTracking);
        }

        /// <summary>
        /// stop playtime tracking for all items
        /// </summary>
        public void StopPlaytimeTrackingForAllItems()
        {
            RegisterSystem();

            var call = SteamUGC.StopPlaytimeTrackingForAllItems();
            m_StopPlaytimeTrackingResult.Set(call, HandleStopPlaytimeTracking);
        }

        /// <summary>
        /// Submit item update
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="changeNote"></param>
        public void SubmitItemUpdate(UGCUpdateHandle_t handle, string changeNote)
        {
            RegisterSystem();

            var call = SteamUGC.SubmitItemUpdate(handle, changeNote);
            m_SubmitItemUpdateResult.Set(call, HandleItemUpdateResult);
        }

        /// <summary>
        /// Subscribe to item
        /// </summary>
        /// <param name="fileId"></param>
        public void SubscribeItem(PublishedFileId_t fileId)
        {
            RegisterSystem();

            var call = SteamUGC.SubscribeItem(fileId);
            m_RemoteStorageSubscribePublishedFileResult.Set(call, HandleSubscribeFileResult);
        }

        /// <summary>
        /// Suspend downloads
        /// </summary>
        /// <param name="suspend"></param>
        public void SuspendDownloads(bool suspend)
        {
            RegisterSystem();

            SteamUGC.SuspendDownloads(suspend);
        }

        /// <summary>
        /// Unsubscribe to item
        /// </summary>
        /// <param name="fileId"></param>
        public void UnsubscribeItem(PublishedFileId_t fileId)
        {
            RegisterSystem();

            var call = SteamUGC.UnsubscribeItem(fileId);
            m_RemoteStorageUnsubscribePublishedFileResult.Set(call, HandleUnsubscribeFileResult);
        }

        /// <summary>
        /// Update item preview file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool UpdateItemPreviewFile(UGCUpdateHandle_t handle, uint index, string file)
        {
            RegisterSystem();

            return SteamUGC.UpdateItemPreviewFile(handle, index, file);
        }

        /// <summary>
        /// Update item preview video
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="index"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public bool UpdateItemPreviewVideo(UGCUpdateHandle_t handle, uint index, string videoId)
        {
            RegisterSystem();

            return SteamUGC.UpdateItemPreviewVideo(handle, index, videoId);
        }

        #region callbacks
        private void HandleAddUGCDependencyResult(AddUGCDependencyResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtAddDependency.Invoke(param);
            else
                evtAddDependencyFailed.Invoke(param);
        }

        private void HandleAddAppDependencyResult(AddAppDependencyResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtAddedAppDependency.Invoke(param);
            else
                evtAddAppDependencyFailed.Invoke(param);
        }

        private void HandleUserFavoriteItemsListChanged(UserFavoriteItemsListChanged_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtFavoriteItemsChanged.Invoke(param);
            else
                evtFavoriteItemsChangeFailed.Invoke(param);
        }

        private void HandleCreatedItem(CreateItemResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtItemCreated.Invoke(param);
            else
                evtItemCreateFailed.Invoke(param);
        }

        private void HandleDeleteItem(DeleteItemResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtItemDeleted.Invoke(param);
            else
                evtItemDeleteFailed.Invoke(param);
        }

        private void HandleDownloadedItem(DownloadItemResult_t param)
        {
            evtItemDownloaded.Invoke(param);
        }

        private void HandleGetAppDependenciesResults(GetAppDependenciesResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtAppDependenciesResults.Invoke(param);
            else
                evtAppDependenciesResultsFailed.Invoke(param);
        }

        private void HandleGetUserItemVoteResult(GetUserItemVoteResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtUserItemVoteResults.Invoke(param);
            else
                evtUserItemVoteResultsFailed.Invoke(param);
        }

        private void HandleRemoveAppDependencyResult(RemoveAppDependencyResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtRemoveAppDependencyResults.Invoke(param);
            else
                evtRemoveAppDependencyResultsFailed.Invoke(param);
        }

        private void HandleRemoveDependencyResult(RemoveUGCDependencyResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtRemoveDependencyResults.Invoke(param);
            else
                evtRemoveDependencyResultsFailed.Invoke(param);
        }

        private void HandleRequestDetailsResult(SteamUGCRequestUGCDetailsResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtRequestDetailsResults.Invoke(param);
            else
                evtRequestDetailsResultsFailed.Invoke(param);
        }

        private void HandleQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtQueryCompelted.Invoke(param);
            else
                evtQueryCompeltedFailed.Invoke(param);
        }

        private void HandleSetUserItemVoteResult(SetUserItemVoteResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtSetUserItemVoteResult.Invoke(param);
            else
                evtSetUserItemVoteResultFailed.Invoke(param);
        }

        private void HandleStartPlaytimeTracking(StartPlaytimeTrackingResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtStartPlaytimeTrackingResult.Invoke(param);
            else
                evtStartPlaytimeTrackingResultFailed.Invoke(param);
        }

        private void HandleStopPlaytimeTracking(StopPlaytimeTrackingResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtStopPlaytimeTrackingResult.Invoke(param);
            else
                evtStopPlaytimeTrackingResultFailed.Invoke(param);
        }

        private void HandleUnsubscribeFileResult(RemoteStorageUnsubscribePublishedFileResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtRemoteStorageUnsubscribeFileResult.Invoke(param);
            else
                evtRemoteStorageUnsubscribeFileResultFailed.Invoke(param);
        }

        private void HandleSubscribeFileResult(RemoteStorageSubscribePublishedFileResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtRemoteStorageSubscribeFileResult.Invoke(param);
            else
                evtRemoteStorageSubscribeFileResultFailed.Invoke(param);
        }

        private void HandleItemUpdateResult(SubmitItemUpdateResult_t param, bool bIOFailure)
        {
            if (!bIOFailure)
                evtSubmitItemUpdateResult.Invoke(param);
            else
                evtSubmitItemUpdateResultFailed.Invoke(param);
        }
        #endregion

        #endregion
    }
}
#endif