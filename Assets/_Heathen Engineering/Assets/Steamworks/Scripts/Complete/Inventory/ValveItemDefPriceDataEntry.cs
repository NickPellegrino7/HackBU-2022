#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class ValveItemDefPriceDataEntry
    {
        public string currencyCode = "EUR";
        public uint value = 100;

        public override string ToString()
        {
            return currencyCode + value.ToString("000");
        }
    }
}
#endif