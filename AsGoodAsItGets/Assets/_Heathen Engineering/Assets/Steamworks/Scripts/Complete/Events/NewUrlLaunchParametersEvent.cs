﻿#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [System.Serializable]
    public class NewUrlLaunchParametersEvent : UnityEvent<NewUrlLaunchParameters_t> { }
}
#endif