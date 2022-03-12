#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public class InventoryChangedEvent : UnityEvent<InventoryChangeRecord>
    { }
}
#endif