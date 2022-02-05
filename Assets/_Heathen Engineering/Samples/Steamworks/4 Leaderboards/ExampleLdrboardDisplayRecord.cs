#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE

using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.UI;
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class ExampleLdrboardDisplayRecord : MonoBehaviour
    {
        public SetUserAvatar userImage;
        public UGUISetUserName userName;
        public UnityEngine.UI.Text score;
        public UnityEngine.UI.Text rank;

        public void SetEntry(LeaderboardEntry entry)
        {
            userImage.LoadAvatar(entry.User);
            userName.SetName(entry.User);
            score.text = entry.Score.ToString();
            rank.text = entry.Rank.ToString();
        }
    }
}
#endif