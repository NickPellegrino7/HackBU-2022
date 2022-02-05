#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
namespace HeathenEngineering.SteamAPI
{
    public interface IWorkshopItemDisplay
    {
        WorkshopReadCommunityItem Data { get; }
        void RegisterData(WorkshopReadCommunityItem data);
    }
}
#endif
