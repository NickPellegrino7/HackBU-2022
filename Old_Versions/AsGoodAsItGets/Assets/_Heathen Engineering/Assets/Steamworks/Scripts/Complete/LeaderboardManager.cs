#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    public class LeaderboardManager : MonoBehaviour
    {
        public LeaderboardObject leaderboard;

        private LeaderboardEntry _lastKnownUserEntry;
        public LeaderboardEntry LastKnownUserEntry
        {
            get => _lastKnownUserEntry;
            private set
            {
                _lastKnownUserEntry = value;
                evtUserEntryUpdated.Invoke(value);
            }
        }

        public UserEntryEvent evtUserEntryUpdated;
        public EntryResultsEvent evtQueryCompleted;
        public UnityEvent evtQueryError;
        public UnityEvent evtUploadError;
        
        public void RefreshUserEntry()
        {
            leaderboard.GetUserEntry((r, e) =>
            {
                if (!e)
                    LastKnownUserEntry = r;
                else
                    evtQueryError.Invoke();
            });
        }

        public void GetTopEntries(int count)
        {
            leaderboard.GetEntries(Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, 0, count, (r, e) =>
            {
                if (!e)
                {
                    var user = r.FirstOrDefault(p => p.entry.m_steamIDUser == Steamworks.SteamUser.GetSteamID());
                    if (user != default)
                        LastKnownUserEntry = user;

                    evtQueryCompleted.Invoke(r);
                }
            });
        }

        public void GetNearbyEntries(int beforeUser, int afterUser)
        {
            leaderboard.GetEntries(Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, -beforeUser, afterUser, (r, e) =>
            {
                if (!e)
                {
                    var user = r.FirstOrDefault(p => p.entry.m_steamIDUser == Steamworks.SteamUser.GetSteamID());
                    if (user != default)
                        LastKnownUserEntry = user;

                    evtQueryCompleted.Invoke(r);
                }
            });
        }

        public void GetAllFriendsEntries()
        {
            leaderboard.GetEntries(Steamworks.ELeaderboardDataRequest.k_ELeaderboardDataRequestFriends, 0, 0, (r, e) =>
            {
                if (!e)
                {
                    var user = r.FirstOrDefault(p => p.entry.m_steamIDUser == Steamworks.SteamUser.GetSteamID());
                    if (user != default)
                        LastKnownUserEntry = user;

                    evtQueryCompleted.Invoke(r);
                }
            });
        }

        public void GetUserEntries(IEnumerable<UserData> users)
        {
            leaderboard.GetEntries(users.ToArray(), (r, e) =>
            {
                if (!e)
                {
                    var user = r.FirstOrDefault(p => p.entry.m_steamIDUser == Steamworks.SteamUser.GetSteamID());
                    if (user != default)
                        LastKnownUserEntry = user;

                    evtQueryCompleted.Invoke(r);
                }
            });
        }

        public void UploadScore(int score)
        {
            leaderboard.UploadScore(score, Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, (r, e) =>
            {
                if (!e)
                {
                    RefreshUserEntry();
                }
                else
                    evtUploadError.Invoke();
            });
        }

        public void UploadScore(int score, int[] details)
        {
            leaderboard.UploadScore(score, details, Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, (r, e) =>
            {
                if (!e)
                {
                    RefreshUserEntry();
                }
                else
                    evtUploadError.Invoke();
            });
        }

        public void ForceScore(int score)
        {
            leaderboard.UploadScore(score, Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, (r, e) =>
            {
                if (!e)
                {
                    RefreshUserEntry();
                }
                else
                    evtUploadError.Invoke();
            });
        }

        public void ForceScore(int score, int[] details)
        {
            leaderboard.UploadScore(score, details, Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, (r, e) =>
            {
                if (!e)
                {
                    RefreshUserEntry();
                }
                else
                    evtUploadError.Invoke();
            });
        }

        [Serializable]
        public class UserEntryEvent : UnityEvent<LeaderboardEntry> { }
        [Serializable]
        public class EntryResultsEvent : UnityEvent<LeaderboardEntry[]> { }
    }
}
#endif