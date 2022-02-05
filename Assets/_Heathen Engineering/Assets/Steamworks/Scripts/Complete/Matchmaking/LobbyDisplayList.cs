#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace HeathenEngineering.SteamAPI
{
    public class LobbyDisplayList : MonoBehaviour
    {
        [FormerlySerializedAs("Filter")]
        public LobbyQueryParameters filter;
        public LobbyRecordBehvaiour recordPrototype;
        public Transform collection;
        [FormerlySerializedAs("OnSearchStarted")]
        [FormerlySerializedAs("onSearchStarted")]
        public UnityEvent evtSearchStarted;
        [FormerlySerializedAs("OnSearchCompleted")]
        [FormerlySerializedAs("onSearchCompleted")]
        public UnityEvent evtSearchCompleted;
        [FormerlySerializedAs("OnLobbySelected")]
        [FormerlySerializedAs("onLobbySelected")]
        public UnitySteamIdEvent evtLobbySelected;

        private void HandleOnSelected(CSteamID lobbyId)
        {
            //Pass the event on
            evtLobbySelected.Invoke(lobbyId);
        }

        public void BrowseLobbies()
        {
            MatchmakingTools.FindLobbies(filter, (r, e) =>
            {
                if (SteamSettings.current.isDebugging)
                {
                    if (e)
                        Debug.Log("Failed to run lobby search, Steam returned bIOFailure = true.");
                    else
                        Debug.Log("Lobby list results found " + r.Count + " records!");
                }

                if (!e)
                {
                    foreach (Transform tran in collection)
                    {
                        Destroy(tran.gameObject);
                    }

                    foreach (var l in r)
                    {
                        var go = Instantiate(recordPrototype.gameObject, collection);
                        var rec = go.GetComponent<LobbyRecordBehvaiour>();
                        rec.SetLobby(l);
                        rec.OnSelected.AddListener(HandleOnSelected);
                    }
                    evtSearchCompleted.Invoke();
                }
            });
            evtSearchStarted.Invoke();
        }

        public void ClearLobbies()
        {
            while (collection.childCount > 0)
            {
                var target = collection.GetChild(0);
                var go = target.gameObject;
                go.SetActive(false);
                target.parent = null;
                Destroy(go);
            }
        }
    }
}
#endif