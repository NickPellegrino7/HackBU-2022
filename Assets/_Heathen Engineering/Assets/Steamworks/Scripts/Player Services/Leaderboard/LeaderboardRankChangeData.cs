#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;

namespace HeathenEngineering.SteamAPI
{
    public struct LeaderboardRankChangeData
    {
        public string leaderboardName;
        public SteamLeaderboard_t leaderboardId;
        public ExtendedLeaderboardEntry oldEntry;
        public ExtendedLeaderboardEntry newEntry;
        public int rankDelta
        {
            get
            {
                if (oldEntry != null)
                    return newEntry.entry.m_nGlobalRank - oldEntry.entry.m_nGlobalRank;
                else
                    return newEntry.entry.m_nGlobalRank;
            }
        }

        public int scoreDeta
        {
            get
            {
                if (oldEntry != null)
                    return newEntry.entry.m_nScore - oldEntry.entry.m_nScore;
                else
                    return newEntry.entry.m_nScore;
            }
        }
    }
}
#endif