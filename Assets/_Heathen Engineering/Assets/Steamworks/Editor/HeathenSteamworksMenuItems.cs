#if !DISABLESTEAMWORKS
using UnityEditor;
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Editors
{
    public class HeathenSteamworksMenuItems
    {
        [MenuItem("Steamworks/Tools/Steamworks.NET Steam API (On GitHub)")]
        public static void SteamworksGetHub()
        {
            Application.OpenURL("https://github.com/rlabrecque/Steamworks.NET/releases");
        }

        [MenuItem("Steamworks/Tools/Mirror Networking API (On Unity Asset Store)")]
        public static void MirrorAssetStore()
        {
            Application.OpenURL("https://assetstore.unity.com/packages/tools/network/mirror-129321");
            Debug.Log("Mirror is one of the integrated networking options support by Heathens Steamworks.\nIf you choose to use Mirror be sure to install Heathen Steamworks's Mirror Integration package after you install Mirror's base package.");
        }

        [MenuItem("Help/Steamworks/Heathen's Documentation")]
        [MenuItem("Steamworks/Help/Heathen's Documentation")]
        public static void HeathenSteamworksDocumentation()
        {
            Application.OpenURL("https://kb.heathenengineering.com/assets/steamworks");
        }

        [MenuItem("Help/Steamworks/Heathen's Discord")]
        [MenuItem("Steamworks/Help/Heathen's Discord")]
        public static void HeathenDiscord()
        {
            Application.OpenURL("https://discord.gg/RMGtDXV");
        }

        [MenuItem("Help/Steamworks/Valve's Documentation")]
        [MenuItem("Steamworks/Help/Valve's Documentation")]
        public static void ValvesDocuments()
        {
            Application.OpenURL("https://partner.steamgames.com/doc/home");
        }

        [MenuItem("Help/Steamworks/Valve's Developer Forums")]
        [MenuItem("Steamworks/Help/Valve's Developer Forums")]
        public static void ValvesForums()
        {
            Application.OpenURL("https://steamcommunity.com/groups/steamworks");
        }

        [MenuItem("Help/Steamworks/Valve's Developer Support")]
        [MenuItem("Steamworks/Help/Valve's Developer Support")]
        public static void ValvesSupport()
        {
            Application.OpenURL("https://partner.steamgames.com/home/contact");
        }
    }
}
#endif