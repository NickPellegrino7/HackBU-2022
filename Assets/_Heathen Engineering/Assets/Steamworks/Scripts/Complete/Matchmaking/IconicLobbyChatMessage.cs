#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE && !UNITY_SERVER
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HeathenEngineering.SteamAPI.UI
{
    public class IconicLobbyChatMessage : MonoBehaviour, ILobbyChatMessage
    {
        public SteamUserIconButton PersonaButton;
        public UnityEngine.UI.Text Message;
        public DateTime timeStamp;
        public UnityEngine.UI.Text timeRecieved;
        public string timeFormat = "HH:mm:ss";
        public bool ShowStamp = true;
        public bool AllwaysShowStamp = false;

        [HideInInspector()]
        public LobbyChatMessageData data;

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


        private bool processing = false;
        private int siblingIndex = -1;

        private void Update()
        {
            if (!processing)
                return;

            var index = SelfTransform.GetSiblingIndex();
            if(index != siblingIndex)
            {
                siblingIndex = index;
                UpdatePersonaIconShow();
            }
        }

        private void UpdatePersonaIconShow()
        {
            //If we are a system message then exit now ... system messages never show persona icons
            if (data == null || data.sender == null)
                return;

            if(siblingIndex == 0)
            {
                PersonaButton.gameObject.SetActive(true);
            }
            else
            {
                var go = SelfTransform.parent.GetChild(siblingIndex - 1).gameObject;
                var msg = go.GetComponent<IconicLobbyChatMessage>();
                if(msg.data != null && msg.data.sender != null && msg.data.sender.userData.id == data.sender.userData.id)
                {
                    //The previous record was also from us ... hide the persona icon
                    PersonaButton.gameObject.SetActive(false);
                }
                else
                {
                    //The previous record was from someone or something else ... show the persona icon
                    PersonaButton.gameObject.SetActive(true);
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ShowStamp && !timeRecieved.gameObject.activeSelf)
                timeRecieved.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!AllwaysShowStamp && timeRecieved.gameObject.activeSelf)
                timeRecieved.gameObject.SetActive(false);
        }

        public void RegisterChatMessage(LobbyChatMessageData data)
        {
            this.data = data;

            PersonaButton.gameObject.SetActive(true);
            PersonaButton.LinkSteamUser(data.sender.userData);
            Message.text = data.message;
            timeStamp = data.recievedTime;
            timeRecieved.text = timeStamp.ToString(timeFormat);

            if (ShowStamp && AllwaysShowStamp)
            {
                timeRecieved.gameObject.SetActive(true);
            }
            else
            {
                timeRecieved.gameObject.SetActive(false);
            }
            siblingIndex = SelfTransform.GetSiblingIndex();
            UpdatePersonaIconShow();
            processing = true;
        }

        public void SetMessageText(string sender, string message)
        {
            PersonaButton.gameObject.SetActive(false);
            Message.text = message;
            timeStamp = DateTime.Now;
            timeRecieved.text = timeStamp.ToString(timeFormat);

            if (ShowStamp && AllwaysShowStamp)
            {
                timeRecieved.gameObject.SetActive(true);
            }
            else
            {
                timeRecieved.gameObject.SetActive(false);
            }

            processing = true;
        }
    }
}
#endif