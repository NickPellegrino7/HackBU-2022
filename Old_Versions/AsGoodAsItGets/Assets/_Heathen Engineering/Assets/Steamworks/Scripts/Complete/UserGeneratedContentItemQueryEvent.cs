#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using System;
using UnityEngine.Events;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public class UserGeneratedContentItemQueryEvent : UnityEvent<UgcQuery>
    { }
}
#endif
