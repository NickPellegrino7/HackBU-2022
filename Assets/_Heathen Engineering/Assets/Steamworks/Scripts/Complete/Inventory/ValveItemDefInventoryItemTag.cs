#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class ValveItemDefInventoryItemTag
    {
        public string category;
        public string tag;

        public override string ToString()
        {
            return category + ":" + tag;
        }
    }
}
#endif