﻿#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class LobbyResponceEvent : UnityEvent<Steamworks.EChatRoomEnterResponse> { }
}
#endif