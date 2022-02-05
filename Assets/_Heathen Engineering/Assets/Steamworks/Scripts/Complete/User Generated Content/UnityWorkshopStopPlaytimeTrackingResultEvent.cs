#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityWorkshopStopPlaytimeTrackingResultEvent : UnityEvent<StopPlaytimeTrackingResult_t>
    { }
}
#endif
