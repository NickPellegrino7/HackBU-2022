#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public class WorkshopItemInstalledEvent : UnityEvent<ItemInstalled_t>
    { }
}
#endif
