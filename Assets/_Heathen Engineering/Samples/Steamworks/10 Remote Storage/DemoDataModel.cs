#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    [CreateAssetMenu(menuName = "Steamworks/DEMO/DataModel")]
    public class DemoDataModel : SteamworksIntegration.DataModel<DemoDataModelType>
    { }
}
#endif
