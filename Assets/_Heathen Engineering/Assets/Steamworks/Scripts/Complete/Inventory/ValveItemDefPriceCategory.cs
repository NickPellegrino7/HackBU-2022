#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class ValveItemDefPriceCategory
    {
        public uint version = 1;
        public ValvePriceCategories price = ValvePriceCategories.VLV100;

        public override string ToString()
        {
            return version.ToString() + ";" + price.ToString();
        }
    }
}
#endif