#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public struct LobbyMetadata
    {
        public List<MetadataRecord> Records;

        public string this[string dataKey]
        {
            get
            {
                if (Records == null || Records.Count < 1)
                    return string.Empty;
                else
                    return Records.FirstOrDefault(p => p.key == dataKey).value;
            }
            set
            {
                if(string.IsNullOrEmpty(dataKey))
                {
                    throw new IndexOutOfRangeException("Attempted to store Value = '" + value + "' in an empty key. The key must be a non-empty string.");
                }

                if (Records == null)
                    Records = new List<MetadataRecord>();

                if (Records.Count < 1 || !Records.Exists(p => p.key == dataKey))
                {
                    Records.Add(new MetadataRecord() { key = dataKey, value = value });
                }
                else
                {
                    Records.RemoveAll(p => p.key == dataKey);
                    Records.Add(new MetadataRecord() { key = dataKey, value = value });
                }
            }
        }
    }
}
#endif