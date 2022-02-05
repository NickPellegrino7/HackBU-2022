#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class InventoryItemPointerCount
    {
        public InventoryItemPointer Item;
        public uint Count;
        
        public override string ToString()
        {
            return Item.definitionID.m_SteamItemDef + "x" + Count;
        }
    }
}
#endif