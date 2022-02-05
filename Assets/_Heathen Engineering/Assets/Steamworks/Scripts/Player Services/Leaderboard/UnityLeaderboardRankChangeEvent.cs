#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityLeaderboardRankChangeEvent : UnityEvent<LeaderboardRankChangeData>
    { }
}
#endif
