#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using UnityEngine;

namespace HeathenEngineering.SteamAPI
{
    /// <summary>
    /// Base class used by HeathenSteamLeaderboard to represent leaderboard entries
    /// Derive from this class and override the ApplyEntry method to create a custom entry record
    /// </summary>
    public class HeathenSteamLeaderboardEntry : MonoBehaviour
    {
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

        public virtual void ApplyEntry(ExtendedLeaderboardEntry entry)
        { }
    }
}
#endif
