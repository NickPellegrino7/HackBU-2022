#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityWorkshopSteamUGCRequestUGCDetailsResultEvent : UnityEvent<SteamUGCRequestUGCDetailsResult_t>
    { }
}
#endif
