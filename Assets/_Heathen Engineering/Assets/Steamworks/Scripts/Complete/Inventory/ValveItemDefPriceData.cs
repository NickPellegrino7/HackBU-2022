#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using System.Collections.Generic;
using System.Text;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class ValveItemDefPriceData
    {
        public uint version = 1;
        public List<ValveItemDefPriceDataEntry> values;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var v in values)
            {
                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append(v.ToString());
            }

            return version.ToString() + ";" + sb.ToString();
        }
    }
}
#endif