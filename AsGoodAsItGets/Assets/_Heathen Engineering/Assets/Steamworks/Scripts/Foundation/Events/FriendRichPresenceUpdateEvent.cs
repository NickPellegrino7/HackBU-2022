#if !DISABLESTEAMWORKS
using Steamworks;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class FriendRichPresenceUpdateEvent : UnityEvent<FriendRichPresenceUpdate_t> { }
}
#endif