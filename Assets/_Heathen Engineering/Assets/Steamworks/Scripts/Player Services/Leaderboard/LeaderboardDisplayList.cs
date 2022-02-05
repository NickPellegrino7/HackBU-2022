#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace HeathenEngineering.SteamAPI.UI
{
    /// <summary>
    /// Used to display the result of a <see cref="LeaderboardObject"/> query
    /// </summary>
    public class LeaderboardDisplayList : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="LeaderboardObject"/> object this list displays
        /// </summary>
        public LeaderboardObject Settings;
        /// <summary>
        /// The prototype to be spawned for each entry downloaded. This should contain a componenet derived from <see cref="HeathenSteamLeaderboardEntry"/>.
        /// </summary>
        [Tooltip("The prototype to be spawned for each entry downloaded. This should contain a componenet derived from HeathenSteamLeaderboardEntry")]
        public GameObject entryPrototype;
        /// <summary>
        /// The collection transform such as a Scroll Rect's 'container', this will be the 'parent' of the spawned entries.
        /// </summary>
        [Tooltip("The collection transform such as a Scroll Rect's 'container', this will be the 'parent' of the spawned entries.")]
        public RectTransform collection;
        /// <summary>
        /// If true and if a scroll rect is found the players record will be scrolled to be as center of the view as possible on load of records.\n\nThis does not apply if the Override On Downloaded event is used.
        /// </summary>
        [Tooltip("If true and if a scroll rect is found the players record will be scrolled to be as center of the view as possible on load of records.\n\nThis does not apply if the Override On Downloaded event is used.")]
        public bool focusPlayer = true;
        /// <summary>
        /// Returns the player's current value for this leaderboard
        /// </summary>
        public ExtendedLeaderboardEntry UserEntry
        {
            get
            {
                if (Settings != null)
                    return Settings.userEntry;
                else
                    return null;
            }
        }

        private RectTransform _selfTransform;
        public RectTransform SelfTransform
        {
            get
            {
                if (_selfTransform == null)
                    _selfTransform = GetComponent<RectTransform>();
                return _selfTransform;
            }
        }

        /// <summary>
        /// Stores a copy of the entries loaded on the last read
        /// This is not automatically updated if Override On Download is used
        /// </summary>
        [HideInInspector]
        public List<ExtendedLeaderboardEntry> entries;
        
        private UnityEngine.UI.ScrollRect scrollRect;

        private void Start()
        {
            scrollRect = collection.GetComponentInParent<UnityEngine.UI.ScrollRect>();

            if (Settings != null)
                RegisterBoard(Settings);
        }

        /// <summary>
        /// Registers the given board to the List behaviour and registeres on the related events
        /// </summary>
        /// <param name="data"></param>
        public void RegisterBoard(LeaderboardObject data)
        {
            if(Settings != null)
            {
                Settings.evtQueryResults.RemoveListener(HandleQuerryResult);
            }
            Settings = data;
            Settings.evtQueryResults.AddListener(HandleQuerryResult);
        }

        private void HandleQuerryResult(LeaderboardScoresDownloaded scores)
        {
            float playerPosition = 1;
            if (scores.bIOFailure)
            {
                Debug.LogError("Failed to download score from Steamworks", this);
                return;
            }

            entries = scores.scoreData;

            //Clear the collection if we have one
            if (collection != null)
            {
                var dList = new List<GameObject>();
                foreach (Transform t in collection)
                {
                    dList.Add(t.gameObject);
                }

                while (dList.Count > 0)
                {
                    var t = dList[0];
                    dList.RemoveAt(0);
                    Destroy(t);
                }
            }

            var userId = SteamUser.GetSteamID();
            
            foreach(var record in entries)
            {
                if(focusPlayer)
                {
                    if(userId.m_SteamID == record.entry.m_steamIDUser.m_SteamID)
                    {
                        playerPosition = entries.IndexOf(record) / (float)entries.Count;
                    }
                }

                if (entryPrototype != null && collection != null)
                {
                    var go = Instantiate(entryPrototype, collection);
                    var entryDisplay = go.GetComponent<HeathenSteamLeaderboardEntry>();
                    if (record != null)
                    {
                        entryDisplay.SelfTransform.localPosition = Vector3.zero;
                        entryDisplay.SelfTransform.localRotation = Quaternion.identity;
                        entryDisplay.SelfTransform.localScale = Vector3.one;
                        entryDisplay.ApplyEntry(record);
                    }
                }
            }

            if (focusPlayer && scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = playerPosition;
                Canvas.ForceUpdateCanvases();
            }
        }
        
        /// <summary>
        /// Upload a score to the boeard for the current player
        /// </summary>
        /// <param name="score"></param>
        /// <param name="method"></param>
        public void UploadScore(int score, ELeaderboardUploadScoreMethod method)
        {
            Settings.UploadScore(score, method);
        }

        /// <summary>
        /// Upload a score to the boeard for the current player
        /// </summary>
        /// <param name="score"></param>
        /// <param name="method"></param>
        public void UploadScore(int score, int[] scoreDetails, ELeaderboardUploadScoreMethod method)
        {
            Settings.UploadScore(score, scoreDetails, method);
        }

        /// <summary>
        /// Get entries from the leaderboard
        /// </summary>
        /// <param name="requestType">type of data request, when downloading leaderboard entries</param>
        /// <param name="rangeStart">the result index to start at</param>
        /// <param name="rangeEnd">the result index to end at</param>
        public void QueryEntries(ELeaderboardDataRequest requestType, int rangeStart, int rangeEnd)
        {
            Settings.QueryEntries(requestType, rangeStart, rangeEnd);
        }

        /// <summary>
        /// Return the top entries on the board
        /// </summary>
        /// <param name="count">How many entries to return</param>
        public void QueryTopEntries(int count)
        {
            Settings.QueryTopEntries(count);
        }

        /// <summary>
        /// Return entries near the players current score
        /// </summary>
        /// <param name="aroundPlayer">the number entries around the player to return</param>
        public void QueryPeerEntries(int aroundPlayer)
        {
            Settings.QueryPeerEntries(aroundPlayer);
        }

        /// <summary>
        /// Return entries from the player's friends on this board
        /// </summary>
        /// <param name="aroundPlayer"></param>
        public void QueryFriendEntries(int aroundPlayer)
        {
            Settings.QueryFriendEntries(aroundPlayer);
        }

        /// <summary>
        /// Refresh the players own entry on this board
        /// </summary>
        public void RefreshUserEntry()
        {
            Settings.RefreshUserEntry();
        }
    }
}
#endif