#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// <para>Represents a Steamworks Leaderboard and manages its entries and quries</para>
    /// <para>To create a new <see cref="LeaderboardObject"/> object in your project right click in a folder in your project and select</para>
    /// <para>Create >> Steamworks >> Player Services >> Leaderboard Data</para>
    /// </summary>
    [CreateAssetMenu(menuName = "Steamworks/Leaderboard Object")]
    public class LeaderboardObject : ScriptableObject
    {
        /// <summary>
        /// Should the board be created if missing on the target app
        /// </summary>
        public bool createIfMissing;
        /// <summary>
        /// If creating a board what sort method should be applied
        /// </summary>
        public ELeaderboardSortMethod sortMethod;
        /// <summary>
        /// If creating a board what display type is it
        /// </summary>
        public ELeaderboardDisplayType displayType;
        /// <summary>
        /// What is the name of the board ... if this is not to be created at run time then this must match the name as it appears in Steamworks
        /// </summary>
        public string leaderboardName;
        [Obsolete("Use maxDetailEntries instead")]
        public int MaxDetailEntries => maxDetailEntries;
        /// <summary>
        /// How many detail entries should be allowed on entries from this board
        /// </summary>
        [FormerlySerializedAs("MaxDetailEntries")]
        public int maxDetailEntries = 0;
        [Obsolete("Use leaderboardId instead")]
        public SteamLeaderboard_t? LeaderboardId => leaderboardId;
        /// <summary>
        /// What is the leaderboard ID ... this is nullable if null then no leaderboard has been connected
        /// </summary>
        [FormerlySerializedAs("LeaderboardId")]
        [HideInInspector]
        public SteamLeaderboard_t? leaderboardId;
        /// <summary>
        /// What is the current player's entry for this board ... this is nullable if null then no etnry was found
        /// </summary>
        [HideInInspector]
        public ExtendedLeaderboardEntry userEntry = null;
        [Obsolete("Use evtBoardFound instead")]
        public UnityEvent BoardFound => evtBoardFound;
        /// <summary>
        /// Occures when the leaderboard is found 
        /// </summary>
        [FormerlySerializedAs("BoardFound")]
        public UnityEvent evtBoardFound = new UnityEvent();
        /// <summary>
        /// Occures when query results return from a query submited to the Steamworks backend
        /// </summary>
        public LeaderboardScoresDownloadedEvent evtQueryResults = new LeaderboardScoresDownloadedEvent();
        /// <summary>
        /// Occures when the players rank is loaded from Steamworks
        /// </summary>
        public UnityLeaderboardRankUpdateEvent evtUserRankLoaded = new UnityLeaderboardRankUpdateEvent();
        /// <summary>
        /// Occures when the players rank changes
        /// </summary>
        public UnityLeaderboardRankChangeEvent evtUserRankChanged = new UnityLeaderboardRankChangeEvent();
        /// <summary>
        /// Occures when the player achieves a new high rank in this board
        /// </summary>
        public UnityLeaderboardRankChangeEvent evtUserNewHighRank = new UnityLeaderboardRankChangeEvent();

        private CallResult<LeaderboardFindResult_t> OnLeaderboardFindResultCallResult;
        private CallResult<LeaderboardScoresDownloaded_t> OnLeaderboardScoresDownloadedCallResult;
        private CallResult<LeaderboardScoreUploaded_t> OnLeaderboardScoreUploadedCallResult;
        private CallResult<LeaderboardUGCSet_t> OnLeaderboardUGCSet;

        /// <summary>
        /// Registers the board on Steamworks creating if configured to do so or locating if not.
        /// </summary>
        public void Register()
        {
            OnLeaderboardFindResultCallResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
            OnLeaderboardScoresDownloadedCallResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
            OnLeaderboardScoreUploadedCallResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);
            OnLeaderboardUGCSet = CallResult<LeaderboardUGCSet_t>.Create();

            if (createIfMissing)
                FindOrCreateLeaderboard(sortMethod, displayType);
            else
                FindLeaderboard();
        }

        private void FindOrCreateLeaderboard(ELeaderboardSortMethod sortMethod, ELeaderboardDisplayType displayType)
        {
            var handle = SteamUserStats.FindOrCreateLeaderboard(leaderboardName, sortMethod, displayType);
            OnLeaderboardFindResultCallResult.Set(handle, OnLeaderboardFindResult);
        }

        private void FindLeaderboard()
        {
            var handle = SteamUserStats.FindLeaderboard(leaderboardName);
            OnLeaderboardFindResultCallResult.Set(handle, OnLeaderboardFindResult);
        }

        /// <summary>
        /// Refreshes the user's entry for this board
        /// </summary>
        public void RefreshUserEntry()
        {
            if (!leaderboardId.HasValue)
            {
                Debug.LogError(name + " Leaderboard Data Object, cannot download scores, the leaderboard has not been initalized and cannot download scores.");
                return;
            }

            CSteamID[] users = new CSteamID[] { SteamUser.GetSteamID() };
            var handle = SteamUserStats.DownloadLeaderboardEntriesForUsers(leaderboardId.Value, users, 1);
            OnLeaderboardScoresDownloadedCallResult.Set(handle, OnLeaderboardUserRefreshRequest);
        }

        /// <summary>
        /// Uploads a score for the player to this board
        /// </summary>
        /// <param name="score"></param>
        /// <param name="method"></param>
        public void UploadScore(int score, ELeaderboardUploadScoreMethod method)
        {
            if (!leaderboardId.HasValue)
            {
                Debug.LogError(name + " Leaderboard Data Object, cannot upload scores, the leaderboard has not been initalized and cannot upload scores.");
                return;
            }

            var handle = SteamUserStats.UploadLeaderboardScore(leaderboardId.Value, method, score, null, 0);
            OnLeaderboardScoreUploadedCallResult.Set(handle, OnLeaderboardScoreUploaded);
        }

        /// <summary>
        /// Uploads a score for the player to this board
        /// </summary>
        /// <param name="score"></param>
        /// <param name="method"></param>
        public void UploadScore(int score, int[] scoreDetails, ELeaderboardUploadScoreMethod method)
        {
            if (!leaderboardId.HasValue)
            {
                Debug.LogError(name + " Leaderboard Data Object, cannot upload scores, the leaderboard has not been initalized and cannot upload scores.");
                return;
            }

            var handle = SteamUserStats.UploadLeaderboardScore(leaderboardId.Value, method, score, scoreDetails, scoreDetails.Length);
            OnLeaderboardScoreUploadedCallResult.Set(handle, OnLeaderboardScoreUploaded);
        }

        /// <summary>
        /// Attempts to save, share and attach an object to the leaderboard
        /// </summary>
        /// <remarks>
        /// Note that this depends on being able to save the file to the User's Remote Storage which is a limited resoruce so use this sparingly.
        /// </remarks>
        /// <param name="fileName">The name the file should be saved as. This must be unique on the user's storage</param>
        /// <param name="JsonObject">A JsonUtility serialisable object, we will serialize this to UTF8 format and then convert to byte[] for you and upload to Steam Remote Storage</param>
        /// <param name="callback">{ LeaderbaordUGCSet_t result, bool error } optional callback that will pass the results to you, if error is true it indicates a failure from Valve.</param>
        public void AttachLeaderboardUGC(string fileName, object JsonObject, Action<LeaderboardUGCSet_t, bool> callback = null)
        {
            if (!leaderboardId.HasValue)
            {
                Debug.LogError(name + " Leaderboard Data Object, cannot attach UGC files, the leaderboard has not been initalized and cannot process.");
                return;
            }
            
            if (RemoteStorageSystem.FileWrite(fileName, JsonObject, System.Text.Encoding.UTF8))
            {
                RemoteStorageSystem.FileShare(fileName, (r, e) =>
                {
                    if (e)
                    {
                        Debug.LogError("[LeaderboardObject].[AttachLeaderboardUGC]: Valve reported a bIOFailure while attempting to share the file.");
                        callback?.Invoke(default, e);
                    }
                    else
                    {
                        var handle = SteamUserStats.AttachLeaderboardUGC(leaderboardId.Value, r.m_hFile);
                        OnLeaderboardUGCSet.Set(handle, (sr, se) =>
                        {
                            if(se)
                            {
                                Debug.LogError("[LeaderboardObject].[AttachLeaderboardUGC]: Valve reported a bIOFailure while attempting to attach the file to the user's entry.");
                            }

                            callback?.Invoke(sr, se);
                        });
                    }
                });
            }
            else
            {
                Debug.LogError("[LeaderboardObject].[AttachLeaderboardUGC]: Valve reported a file write error while attempting to save the file.");
                callback?.Invoke(default, true);
            }
        }

        /// <summary>
        /// Get the top entries for this board
        /// </summary>
        /// <param name="count"></param>
        public void QueryTopEntries(int count, Action<List<ExtendedLeaderboardEntry>, bool> callback = null)
        {
            QueryEntries(ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, count);
        }

        /// <summary>
        /// Get the entries for the player's friends from this board
        /// </summary>
        /// <param name="aroundPlayer"></param>
        public void QueryFriendEntries(int aroundPlayer, Action<List<ExtendedLeaderboardEntry>, bool> callback = null)
        {
            QueryEntries(ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends, -aroundPlayer, aroundPlayer);
        }

        /// <summary>
        /// Get entries for records near the player's record in this board
        /// </summary>
        /// <param name="aroundPlayer"></param>
        public void QueryPeerEntries(int aroundPlayer, Action<List<ExtendedLeaderboardEntry>, bool> callback = null)
        {
            QueryEntries(ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -aroundPlayer, aroundPlayer);
        }

        /// <summary>
        /// Query for entries from this baord
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="rangeStart"></param>
        /// <param name="rangeEnd"></param>
        public void QueryEntries(ELeaderboardDataRequest requestType, int rangeStart, int rangeEnd, Action<List<ExtendedLeaderboardEntry>, bool> callback = null)
        {
            if (!leaderboardId.HasValue)
            {
                Debug.LogError(name + " Leaderboard Data Object, cannot download scores, the leaderboard has not been initalized and cannot download scores.");
                return;
            }

            var handle = SteamUserStats.DownloadLeaderboardEntries(leaderboardId.Value, requestType, rangeStart, rangeEnd);
            OnLeaderboardScoresDownloadedCallResult.Set(handle, (param, bIOFailure) =>
            {
                var resultList = ProcessScoresDownloaded(param, bIOFailure, out bool playerIncluded);
                evtQueryResults.Invoke(new LeaderboardScoresDownloaded() { bIOFailure = bIOFailure, scoreData = resultList, playerIncluded = playerIncluded });
                callback?.Invoke(resultList, bIOFailure);
            });
        }

        private void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t param, bool bIOFailure)
        {
            if (param.m_bSuccess == 0 || bIOFailure)
                Debug.LogError(name + " Leaderboard Data Object, failed to upload score to Steamworks: Success code = " + param.m_bSuccess, this);

            RefreshUserEntry();
        }

        private void OnLeaderboardUserRefreshRequest(LeaderboardScoresDownloaded_t param, bool bIOFailure)
        {
            ProcessScoresDownloaded(param, bIOFailure, out bool _);
        }

        private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t param, bool bIOFailure)
        {
            var resultList = ProcessScoresDownloaded(param, bIOFailure, out bool playerIncluded);
            evtQueryResults.Invoke(new LeaderboardScoresDownloaded() { bIOFailure = bIOFailure, scoreData = resultList, playerIncluded = playerIncluded });
        }

        private List<ExtendedLeaderboardEntry> ProcessScoresDownloaded(LeaderboardScoresDownloaded_t param, bool bIOFailure, out bool playerIncluded)
        {
            var entries = new List<ExtendedLeaderboardEntry>();

            playerIncluded = false;
            ///Check for the current users data in the record set and update accordingly
            if (!bIOFailure)
            {
                var userId = SteamUser.GetSteamID();

                for (int i = 0; i < param.m_cEntryCount; i++)
                {
                    LeaderboardEntry_t buffer;
                    int[] details = null;

                    if (maxDetailEntries < 1)
                        SteamUserStats.GetDownloadedLeaderboardEntry(param.m_hSteamLeaderboardEntries, i, out buffer, details, maxDetailEntries);
                    else
                    {
                        details = new int[maxDetailEntries];
                        SteamUserStats.GetDownloadedLeaderboardEntry(param.m_hSteamLeaderboardEntries, i, out buffer, details, maxDetailEntries);
                    }

                    ExtendedLeaderboardEntry record = new ExtendedLeaderboardEntry();
                    record.entry = buffer;
                    record.details = details;
                    
                    entries.Add(record);

                    if (buffer.m_steamIDUser.m_SteamID == userId.m_SteamID)
                    {
                        playerIncluded = true;
                        if (userEntry != null)
                        {
                            var lc = new LeaderboardRankChangeData()
                            {
                                leaderboardName = leaderboardName,
                                leaderboardId = leaderboardId.Value,
                                newEntry = new ExtendedLeaderboardEntry() { entry = buffer, details = details },
                                oldEntry = userEntry
                            };

                            userEntry = lc.newEntry;

                            evtUserRankLoaded.Invoke(userEntry);
                            evtUserRankChanged.Invoke(lc);

                            if (lc.newEntry.entry.m_nGlobalRank < (lc.oldEntry != null ? lc.oldEntry.entry.m_nGlobalRank : int.MaxValue))
                            {
                                evtUserNewHighRank.Invoke(lc);
                            }
                        }
                        else
                        {
                            userEntry = new ExtendedLeaderboardEntry() { entry = buffer, details = details };
                            evtUserRankLoaded.Invoke(userEntry);
                        }
                    }
                }
            }

            return entries;
        }

        private void OnLeaderboardFindResult(LeaderboardFindResult_t param, bool bIOFailure)
        {
            if (param.m_bLeaderboardFound == 0 || bIOFailure)
            {
                Debug.LogError("Failed to find leaderboard", this);
                return;
            }

            if (param.m_bLeaderboardFound != 0)
            {
                leaderboardId = param.m_hSteamLeaderboard;
                evtBoardFound.Invoke();
                RefreshUserEntry();
            }
        }
    }
}
#endif