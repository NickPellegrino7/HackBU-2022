#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using Steamworks;
using System;

namespace HeathenEngineering.SteamworksIntegration
{
    [Serializable]
    public struct LobbyChatMsg
    {
        public Lobby lobby;
        public UserData sender;
        public byte[] data;
        public DateTime recievedTime;
        public string Message => ToString();
        public override string ToString()
        {
            return System.Text.Encoding.UTF8.GetString(data);
        }

        public T FromJson<T>() => UnityEngine.JsonUtility.FromJson<T>(ToString());
    }

}
#endif