#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE

using HeathenEngineering.SteamworksIntegration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    /// <summary>
    /// This is for demonstration purposes only
    /// </summary>
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class Scene4Behaviour : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.InputField scoreValue;
        [SerializeField]
        private UnityEngine.UI.Text scoreLabel;
        [SerializeField]
        private LeaderboardManager ldrboardManager;
        [SerializeField]
        private Transform recordRoot;
        [SerializeField]
        private GameObject recordTempalte;

        private List<GameObject> records = new List<GameObject>();

        private void Start()
        {
            StartCoroutine(RefreshUserScore());
        }

        private IEnumerator RefreshUserScore()
        {
            yield return new WaitUntil(() => SteamSettings.Initialized);
            ldrboardManager.RefreshUserEntry();
        }

        public void OpenKnowledgeBaseUserData()
        {
            Application.OpenURL("https://kb.heathenengineering.com/assets/steamworks");
        }

        public void UpdateScore()
        {
            if (int.TryParse(scoreValue.text, out int value))
                ldrboardManager.UploadScore(value, new int[] { 4, 2, 42 });
        }

        public void ReportTop10()
        {
            ldrboardManager.GetTopEntries(10);
        }

        public void ReportAroundPlayer()
        {
            ldrboardManager.GetNearbyEntries(5, 5);
        }

        public void UserScoreUpdated(LeaderboardEntry entry)
        {
            if(entry == null)
                scoreLabel.text = "Score: NA\nRank: NA\nDetails: NULL";
            else if (entry.details == null)
                scoreLabel.text = "Score: " + entry.Score.ToString() + "\nRank: " + entry.Rank.ToString() + "\nDetails: NULL";
            else
            {
                string details = "{ ";
                for (int i = 0; i < entry.details.Length; i++)
                {
                    if (i == 0)
                        details += entry.details[i].ToString();
                    else
                        details += ", " + entry.details[i].ToString();
                }

                details += " }";
                scoreLabel.text = "Score: " + entry.Score.ToString() + "\nRank: " + entry.Rank.ToString() + "\nDetails: " + details;
            }
        }

        public void HandleBoardQuery(LeaderboardEntry[] entries)
        {
            while(records.Count > 0)
            {
                var record = records[0];
                records.RemoveAt(0);
                Destroy(record);
            }

            foreach (var entry in entries)
            {
                var go = Instantiate(recordTempalte, recordRoot);

                records.Add(go);

                var comp = go.GetComponent<ExampleLdrboardDisplayRecord>();
                comp.SetEntry(entry);
            }
        }
    }
}
#endif