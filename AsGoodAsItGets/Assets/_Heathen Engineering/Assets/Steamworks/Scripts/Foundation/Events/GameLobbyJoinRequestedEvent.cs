#if !DISABLESTEAMWORKS
using Steamworks;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class GameLobbyJoinRequestedEvent : UnityEvent<GameLobbyJoinRequested_t> { }
}
#endif