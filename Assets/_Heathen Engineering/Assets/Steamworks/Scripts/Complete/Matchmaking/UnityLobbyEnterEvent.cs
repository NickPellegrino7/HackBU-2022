#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using Steamworks;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityLobbyEnterEvent : UnityEvent<LobbyEnter_t>
    { }

    [Serializable]
    public class UnityLobbyEvent : UnityEvent<Lobby>
    { }
}
#endif
