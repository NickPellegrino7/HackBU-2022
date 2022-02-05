#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using Steamworks;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityLeaderboardEntryUpdateEvent : UnityEvent<LeaderboardEntry_t>
    { }
}
#endif
