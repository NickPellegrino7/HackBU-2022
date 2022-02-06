#if !DISABLESTEAMWORKS
using Steamworks;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class GameServerChangeRequestedEvent : UnityEvent<GameServerChangeRequested_t> { }
}
#endif