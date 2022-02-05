﻿#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;

namespace HeathenEngineering.SteamworksIntegration
{
    public struct WorkshopItemDataCreateStatus
    {
        public bool hasError;
        public string errorMessage;
        public PublishedFileId_t? ugcFileId;
        public CreateItemResult_t? createItemResult;
        public SubmitItemUpdateResult_t? submitItemUpdateResult;
    }
}
#endif