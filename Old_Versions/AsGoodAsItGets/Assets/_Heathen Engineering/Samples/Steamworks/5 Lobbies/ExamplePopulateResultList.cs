#if !DISABLESTEAMWORKS && HE_STEAMCOMPLETE
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

namespace HeathenEngineering.DEMO
{
    [System.Obsolete("This script is for demonstration purposes ONLY")]
    public class ExamplePopulateResultList : MonoBehaviour
    {
        public GameObject template;
        public Transform layoutRoot;

        public HeathenEngineering.SteamworksIntegration.LobbyDataEvent evtJoinCompleted;
        public LobbyDisplayRecord.JoinFailedEvent evtJoinError;

        public void UpdateDisplayList(Lobby[] lobbies)
        {
            //Get all the old entries
            System.Collections.Generic.List<LobbyDisplayRecord> entries = new System.Collections.Generic.List<LobbyDisplayRecord>();
            layoutRoot.GetComponentsInChildren(entries);

            //Add blank entries to meet the result count
            while (entries.Count < lobbies.Length)
            {
                var GO = Instantiate(template, layoutRoot);
                var entry = GO.GetComponent<LobbyDisplayRecord>();
                
                //Echo the events up to this parent controller
                entry.evtJoinCompleted.AddListener(evtJoinCompleted.Invoke);
                entry.evtJoinError.AddListener(evtJoinError.Invoke);

                //Add the new entry to the list
                entries.Add(entry);
            }

            //Remove exces entries to meet the result count
            while(entries.Count > lobbies.Length)
            {
                var target = entries[0];
                entries.RemoveAt(0);
                Destroy(target.gameObject);
            }

            //Set the entries lobby field to match our lobby results.
            //This will reuse entries if available saving some spawn time
            for (int i = 0; i < lobbies.Length; i++)
            {
                //Set the lobby reference
                entries[i].Lobby = lobbies[i];
            }
        }
    }
}
#endif
