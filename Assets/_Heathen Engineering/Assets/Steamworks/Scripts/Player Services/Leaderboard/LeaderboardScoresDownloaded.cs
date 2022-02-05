#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System;
using System.Collections.Generic;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct LeaderboardScoresDownloaded
    {
        public bool bIOFailure;
        public bool playerIncluded;
        public List<ExtendedLeaderboardEntry> scoreData;
    }
}
#endif
