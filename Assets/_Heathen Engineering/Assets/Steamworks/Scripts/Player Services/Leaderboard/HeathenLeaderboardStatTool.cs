#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    public class HeathenLeaderboardStatTool : MonoBehaviour
    {
        [Header("Settings")]
        public LeaderboardObject LeaderboardObject;
        public StatObject StatObject;
        public ELeaderboardUploadScoreMethod UpdateMethod = ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest;

        [Header("Debug Tools")]
        public bool ShowDebug = false;
        public int StatValue;
        public int UserScore;
        public int UserRank;

        private void Update()
        {
            if(ShowDebug)
            {
                if (LeaderboardObject != null && StatObject != null)
                {
                    StatValue = StatObject.GetIntValue();
                    UserScore = LeaderboardObject.userEntry != null ? LeaderboardObject.userEntry.entry.m_nScore : 0;
                    UserRank = LeaderboardObject.userEntry != null ? LeaderboardObject.userEntry.entry.m_nGlobalRank : 0;
                }
            }
        }

        public void Submit()
        {
            if(LeaderboardObject != null && StatObject != null)
            {
                LeaderboardObject.UploadScore(StatObject.GetIntValue(), UpdateMethod);
            }
        }
    }
}
#endif