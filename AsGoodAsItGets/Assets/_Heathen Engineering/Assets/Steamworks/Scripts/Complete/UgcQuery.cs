﻿#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    public class UgcQuery : IDisposable
    {
        public UGCQueryHandle_t handle;
        public uint matchedRecordCount = 0;
        public uint pageCount = 1;
        private bool isAllQuery = false;
        private bool isUserQuery = false;
        private EUserUGCList listType;
        private EUGCQuery queryType;
        private EUGCMatchingUGCType matchingType;
        private EUserUGCListSortOrder sortOrder;
        private AppId_t creatorApp;
        private AppId_t consumerApp;
        private AccountID_t account;
        private uint _Page = 1;
        public uint Page { get { return _Page; } private set { _Page = value; } }
        private UnityAction<UgcQuery> callback;

        public CallResult<SteamUGCQueryCompleted_t> m_SteamUGCQueryCompleted;

        public List<UGCCommunityItem> ResultsList = new List<UGCCommunityItem>();

        public UgcQuery()
        {
            m_SteamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(HandleQueryCompleted);
        }

        public static UgcQuery Create(EUGCQuery queryType, EUGCMatchingUGCType matchingType, AppId_t creatorApp, AppId_t consumerApp)
        {
            UgcQuery nQuery = new UgcQuery
            {
                matchedRecordCount = 0,
                pageCount = 1,
                isAllQuery = true,
                isUserQuery = false,
                queryType = queryType,
                matchingType = matchingType,
                creatorApp = creatorApp,
                consumerApp = consumerApp,
                Page = 1,
                handle = API.UserGeneratedContent.Client.CreateQueryAllRequest(queryType, matchingType, creatorApp, consumerApp, 1)
            };

            return nQuery;
        }

        public static UgcQuery Create(IEnumerable<PublishedFileId_t> fileIds)
        {
            var list = new List<PublishedFileId_t>(fileIds);
            UgcQuery nQuery = new UgcQuery
            {
                matchedRecordCount = 0,
                pageCount = 1,
                isAllQuery = true,
                isUserQuery = false,
                Page = 1,
                handle = API.UserGeneratedContent.Client.CreateQueryDetailsRequest(list.ToArray())
            };

            return nQuery;
        }

        public static UgcQuery Create(AccountID_t account, EUserUGCList listType, EUGCMatchingUGCType matchingType, EUserUGCListSortOrder sortOrder, AppId_t creatorApp, AppId_t consumerApp)
        {
            UgcQuery nQuery = new UgcQuery
            {
                matchedRecordCount = 0,
                pageCount = 1,
                isAllQuery = false,
                isUserQuery = true,
                listType = listType,
                sortOrder = sortOrder,
                matchingType = matchingType,
                creatorApp = creatorApp,
                consumerApp = consumerApp,
                account = account,
                Page = 1,
                handle = API.UserGeneratedContent.Client.CreateQueryUserRequest(account, listType, matchingType, sortOrder, creatorApp, consumerApp, 1)
            };

            return nQuery;
        }

        /// <summary>
        /// Set item langauge
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool SetLanguage(string language) => API.UserGeneratedContent.Client.SetLanguage(handle, language);

        /// <summary>
        /// Set match any tag
        /// </summary>
        /// <param name="anyTag"></param>
        /// <returns></returns>
        public bool SetMatchAnyTag(bool anyTag) => API.UserGeneratedContent.Client.SetMatchAnyTag(handle, anyTag);

        /// <summary>
        /// Set ranked by trend days
        /// </summary>
        /// <param name="days"></param>
        /// <returns></returns>
        public bool SetRankedByTrendDays(uint days) => API.UserGeneratedContent.Client.SetRankedByTrendDays(handle, days);

        /// <summary>
        /// Set return additional previews
        /// </summary>
        /// <param name="additionalPreviews"></param>
        /// <returns></returns>
        public bool SetReturnAdditionalPreviews(bool additionalPreviews) => API.UserGeneratedContent.Client.SetReturnAdditionalPreviews(handle, additionalPreviews);

        /// <summary>
        /// Set return childre
        /// </summary>
        /// <param name="returnChildren"></param>
        /// <returns></returns>
        public bool SetReturnChildren(bool returnChildren) => API.UserGeneratedContent.Client.SetReturnChildren(handle, returnChildren);

        /// <summary>
        /// Set return key value tags
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool SetReturnKeyValueTags(bool tags) => API.UserGeneratedContent.Client.SetReturnKeyValueTags(handle, tags);

        /// <summary>
        /// SEt return long description
        /// </summary>
        /// <param name="longDescription"></param>
        /// <returns></returns>
        public bool SetReturnLongDescription(bool longDescription) =>  API.UserGeneratedContent.Client.SetReturnLongDescription(handle, longDescription);

        /// <summary>
        /// Set return metadata
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public bool SetReturnMetadata(bool metadata) =>  API.UserGeneratedContent.Client.SetReturnMetadata(handle, metadata);

        /// <summary>
        /// Set return IDs only
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="onlyIds"></param>
        /// <returns></returns>
        public bool SetReturnOnlyIDs(bool onlyIds) => API.UserGeneratedContent.Client.SetReturnOnlyIDs(handle, onlyIds);

        /// <summary>
        /// Set return playtime stats
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public bool SetReturnPlaytimeStats(uint days) => API.UserGeneratedContent.Client.SetReturnPlaytimeStats(handle, days);

        /// <summary>
        /// Set return total only
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="totalOnly"></param>
        /// <returns></returns>
        public bool SetReturnTotalOnly(bool totalOnly) => API.UserGeneratedContent.Client.SetReturnTotalOnly(handle, totalOnly);

        /// <summary>
        /// Set search text
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool SetSearchText(string text) => API.UserGeneratedContent.Client.SetSearchText(handle, text);

        public bool SetNextPage() =>  SetPage((uint)Mathf.Clamp((int)Page + 1, 1, int.MaxValue));

        public bool SetPreviousPage() => SetPage((uint)Mathf.Clamp((int)Page - 1, 1, int.MaxValue));

        public bool SetPage(uint page)
        {
            Page = page > 0 ? page : 1;
            if (isAllQuery)
            {
                ReleaseHandle();
                handle = API.UserGeneratedContent.Client.CreateQueryAllRequest(queryType, matchingType, creatorApp, consumerApp, Page);
                matchedRecordCount = 0;
                return true;
            }
            else if (isUserQuery)
            {
                ReleaseHandle();
                handle = API.UserGeneratedContent.Client.CreateQueryUserRequest(account, listType, matchingType, sortOrder, creatorApp, consumerApp, Page);
                matchedRecordCount = 0;
                return true;
            }
            else
            {
                Debug.LogError("Pages are not supported by detail queries e.g. searching for specific file Ids");
                return false;
            }
        }

        public bool Execute(UnityAction<UgcQuery> callback)
        {
            if(handle == UGCQueryHandle_t.Invalid)
            {
                Debug.LogError("Invalid handle, you must call CreateAll");
                return false;
            }

            ResultsList.Clear();
            this.callback = callback;
            API.UserGeneratedContent.Client.SendQueryUGCRequest(handle, HandleQueryCompleted);

            return true;
        }

        private void HandleQueryCompleted(SteamUGCQueryCompleted_t param, bool bIOFailure)
        {
            if (!bIOFailure)
            {
                if (param.m_eResult == EResult.k_EResultOK)
                {
                    matchedRecordCount = param.m_unTotalMatchingResults;

                    pageCount = (uint)Mathf.Clamp((int)matchedRecordCount / 50, 1, int.MaxValue);
                    if (pageCount * 50 < matchedRecordCount)
                        pageCount++;

                    for (int i = 0; i < param.m_unNumResultsReturned; i++)
                    {
                        SteamUGCDetails_t details;
                        API.UserGeneratedContent.Client.GetQueryResult(param.m_handle, (uint)i, out details);
                        var nRecord = new UGCCommunityItem(details);
                        ResultsList.Add(nRecord);
                    }
                    ReleaseHandle();
                    if (callback != null)
                        callback.Invoke(this);
                }
                else
                {
                    Debug.LogError("HeathenWorkitemQuery|HandleQueryCompleted Unexpected results, state = " + param.m_eResult.ToString());
                }
            }
            else
            {
                Debug.LogError("HeathenWorkitemQuery|HandleQueryCompleted failed.");
            }
        }

        public void ReleaseHandle()
        {
            if (handle != UGCQueryHandle_t.Invalid)
            {
                API.UserGeneratedContent.Client.ReleaseQueryRequest(handle);
                handle = UGCQueryHandle_t.Invalid;
            }
        }

        public void Dispose()
        {
            try
            {
                ReleaseHandle();
            }
            catch { }
        }
    }
}
#endif