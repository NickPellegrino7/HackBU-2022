#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class TagGeneratorValue
    {
        public string name;
        public int weight;

        public override string ToString()
        {
            return name + ":" + weight;
        }
    }
}
#endif