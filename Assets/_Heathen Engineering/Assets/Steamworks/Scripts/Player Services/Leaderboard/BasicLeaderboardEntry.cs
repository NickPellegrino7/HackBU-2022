#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && !UNITY_SERVER
using HeathenEngineering.SteamAPI.UI;
using Steamworks;

namespace HeathenEngineering.SteamAPI
{
    public class BasicLeaderboardEntry : HeathenSteamLeaderboardEntry
    {
        public UnityEngine.UI.Text rank;
        public SteamUserFullIcon avatar;
        public string formatString;
        public UnityEngine.UI.Text score;
        public LeaderboardEntry_t data;

        public override void ApplyEntry(ExtendedLeaderboardEntry entry)
        {
            data = entry.entry;
            var userData = SteamSettings.current.client.GetUserData(entry.entry.m_steamIDUser);
            avatar.LinkSteamUser(userData);
            if (!string.IsNullOrEmpty(formatString))
                score.text = entry.entry.m_nScore.ToString(formatString);
            else
                score.text = entry.entry.m_nScore.ToString();

            rank.text = entry.entry.m_nGlobalRank.ToString();
        }
    }
}
#endif
