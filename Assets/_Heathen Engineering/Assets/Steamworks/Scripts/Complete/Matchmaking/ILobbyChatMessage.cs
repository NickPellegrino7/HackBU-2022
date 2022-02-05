#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
namespace HeathenEngineering.SteamAPI.UI
{
    public interface ILobbyChatMessage
    {
        void RegisterChatMessage(LobbyChatMessageData data);
        void SetMessageText(string sender, string message);
    }
}
#endif
