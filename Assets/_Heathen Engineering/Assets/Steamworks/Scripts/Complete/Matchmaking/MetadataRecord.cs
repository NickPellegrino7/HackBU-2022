#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct MetadataRecord
    {
        public string key;
        public string value;
    }
}
#endif