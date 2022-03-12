#if !DISABLESTEAMWORKS
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class GameOverlayActivatedEvent : UnityEvent<bool> { }
}
#endif