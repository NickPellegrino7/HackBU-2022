#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HeathenEngineering.SteamAPI.UI
{
    public class LobbyChat : MonoBehaviour
    {
        [Header("Settings")]
        public int maxMessages;
        public bool sendOnKeyCode = false;
        public KeyCode SendCode = KeyCode.Return;
        [Header("UI Elements")]
        public UnityEngine.UI.ScrollRect scrollRect;
        public RectTransform collection;
        public UnityEngine.UI.InputField input;        
        [Header("Templates")]
        public GameObject selfMessagePrototype;
        public GameObject othersMessagePrototype;
        public GameObject sysMessagePrototype;

        [Header("Events")]
        public UnityEvent NewMessageRecieved;

        [HideInInspector]
        public List<GameObject> messages;

        private RectTransform _selfTransform;
        public RectTransform SelfTransform
        {
            get
            {
                if (_selfTransform == null)
                    _selfTransform = GetComponent<RectTransform>();
                return _selfTransform;
            }
        }


        private void OnEnable()
        {
            MatchmakingTools.evtChatMessageReceived.AddListener(HandleLobbyChatMessage);          
        }

        private void OnDisable()
        {
            MatchmakingTools.evtChatMessageReceived.RemoveListener(HandleLobbyChatMessage);
        }

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == input.gameObject && Input.GetKeyDown(SendCode))
            {
                SendChatMessage();
            }
        }

        private void HandleLobbyChatMessage(LobbyChatMessageData data)
        {
            var isNewMessage = data.sender.userData.id.m_SteamID != SteamUser.GetSteamID().m_SteamID;
            var prototype = isNewMessage ? othersMessagePrototype : selfMessagePrototype;
            var go = Instantiate(prototype, collection);
            var msg = go.GetComponent<ILobbyChatMessage>();
            msg.RegisterChatMessage(data);

            messages.Add(go);

            Canvas.ForceUpdateCanvases();
            if (messages.Count > maxMessages)
            {
                var firstLine = messages[0];
                messages.Remove(firstLine);
                Destroy(firstLine.gameObject);
            }
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;

            if (isNewMessage)
                NewMessageRecieved.Invoke();

        }

        /// <summary>
        /// Iterates over the messages list and destroys all messages
        /// </summary>
        public void ClearMessages()
        {
            while(messages.Count > 0)
            {
                var target = messages[0];
                messages.RemoveAt(0);
                Destroy(target);
            }
        }

        /// <summary>
        /// Send a chat message over the Steamworks Lobby Chat system
        /// </summary>
        /// <param name="message"></param>
        public void SendChatMessage(string message)
        {
            if (MatchmakingTools.InLobby)
            {
                var errorMessage = string.Empty;

                //If we are trying to parse a bad command let the player know
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    SendSystemMessage("", errorMessage);
                }
                else
                {
                    MatchmakingTools.SendChatMessage(message);
                    input.ActivateInputField();
                }
            }
        }

        public void SendChatMessage()
        {
            if (!string.IsNullOrEmpty(input.text) && MatchmakingTools.InLobby)
            {
                SendChatMessage(input.text);
                input.text = string.Empty;
            }
            else
            {
                if (!MatchmakingTools.InLobby)
                    Debug.LogWarning("Attempted to send a lobby chat message without an established connection");
            }
        }

        /// <summary>
        /// This message is not sent over the network and only appears to this user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        public void SendSystemMessage(string sender, string message)
        {
            var go = Instantiate(sysMessagePrototype, collection);
            var msg = go.GetComponent<ILobbyChatMessage>();
            msg.SetMessageText(sender, message);

            messages.Add(go);

            Canvas.ForceUpdateCanvases();
            if (messages.Count > maxMessages)
            {
                var firstLine = messages[0];
                messages.Remove(firstLine);
                Destroy(firstLine.gameObject);
            }
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
#endif