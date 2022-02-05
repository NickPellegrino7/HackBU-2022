#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class UnityHeathenWorkshopItemQueryEvent : UnityEvent<WorkshopItemQuery>
    { }
}
#endif
