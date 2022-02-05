#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES
using UnityEngine;

namespace HeathenEngineering.SteamAPI.Demo
{
    /// <summary>
    /// Demonstrates the use of <see cref="LeaderboardObject"/> objects to update user scores.
    /// </summary>
    public class ExampleLeaderboardScoring : MonoBehaviour
    {
        public LeaderboardObject leaderboardData;

        /// <summary>
        /// Simple keep best score update
        /// </summary>
        /// <param name="score"></param>
        public void UpdateScore(int score)
        {
            //This just sends the current score and lets Steamworks decide if this is better than the previous or not
            leaderboardData.UploadScore(score, global::Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest);
            Debug.Log("Set leaderboard: " + leaderboardData.leaderboardName + " score to: " + score.ToString() + " with instruction to keep the best value (comparing current vs new)");
        }

        /// <summary>
        /// Force a score update e.g. tell Steamworks to use this score value rather or not its better than the current value
        /// </summary>
        /// <param name="score"></param>
        public void ForceUpdateScore(int score)
        {
            //This sends the current score and tells Steamworks to overwrite the old score
            leaderboardData.UploadScore(score, global::Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate);
            Debug.Log("Set leaderboard: " + leaderboardData.leaderboardName + " score to: " + score.ToString() + " with instruction to overwrite the current value");
        }

        /// <summary>
        /// Add this amount to the current score value
        /// </summary>
        /// <param name="score"></param>
        public void AddToScore(int score)
        {
            //This gets whatever the last score was and adds the new score to it ... which is odd for a leaderboard but what you asked for
            int currentScore = leaderboardData.userEntry != null ? leaderboardData.userEntry.entry.m_nScore : 0;
            leaderboardData.UploadScore(currentScore + score, global::Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest);
            Debug.Log("Set leaderboard: " + leaderboardData.leaderboardName + " score to: " + (currentScore + score).ToString() + " with instruction to keep the best value (comparing current vs new)");
        }

        /// <summary>
        /// Opens the Valve documentation to the Leaderboards page.
        /// </summary>
        public void GetHelp()
        {
            Application.OpenURL("https://partner.steamgames.com/doc/features/leaderboards");
        }
    }
}
#endif