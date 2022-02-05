#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityLeaderboardRankUpdateEvent : UnityEvent<ExtendedLeaderboardEntry>
    { }
}
#endif
