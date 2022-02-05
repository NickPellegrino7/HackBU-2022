#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE

using Steamworks;
using System;

namespace HeathenEngineering.SteamAPI
{
    [Serializable]
    public class ValveItemDefPromoRule
    {
        public ValveItemDefPromoRuleType type = ValveItemDefPromoRuleType.played;
        public AppId_t app;
        public uint minutes;
        public AchievementObject achievment;

        public override string ToString()
        {
            switch(type)
            {
                case ValveItemDefPromoRuleType.manual:
                    return "manual";
                case ValveItemDefPromoRuleType.owns:
                    return "owns:" + app.ToString();
                case ValveItemDefPromoRuleType.played:
                    return "played:" + app.ToString() + "/" + minutes.ToString();
                case ValveItemDefPromoRuleType.achievement:
                    return "ach:" + achievment.achievementId;
                default:
                    return string.Empty;
            }
        }
    }
}
#endif