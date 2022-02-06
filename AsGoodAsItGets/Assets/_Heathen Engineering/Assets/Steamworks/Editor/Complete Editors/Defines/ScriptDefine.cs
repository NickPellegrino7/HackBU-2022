using System.Collections.Generic;
using UnityEditor;

namespace HeathenEngineering.SteamworksIntegration.GameServices
{
    static class ScriptDefine
    {
        /// <summary>
        /// Add define symbols as soon as Unity gets done compiling.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void AddDefineSymbols()
        {
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            HashSet<string> defines = new HashSet<string>(currentDefines.Split(';'))
            {
                "HE_STEAMCOMPLETE"
            };

            string newDefines = string.Join(";", defines);
            if (newDefines != currentDefines)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, newDefines);
            }
        }
    }
}