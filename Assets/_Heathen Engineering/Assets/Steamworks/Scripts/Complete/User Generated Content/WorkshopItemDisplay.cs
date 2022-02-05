#if !DISABLESTEAMWORKS && HE_STEAMPLAYERSERVICES && HE_STEAMCOMPLETE
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HeathenEngineering.SteamAPI
{
    public class WorkshopItemDisplay : MonoBehaviour, IWorkshopItemDisplay, IPointerEnterHandler, IPointerExitHandler
    {
        [HideInInspector]
        public UnityEngine.UI.RawImage PreviewImage;
        public UnityEngine.UI.Text Title;
        public Vector3 TipOffset;
        public GameObject TipRoot;
        public Transform TipTransform;
        public UnityEngine.UI.Text Description;
        public CanvasGroup toggleGroup;
        public UnityEngine.UI.Toggle Subscribed;
        public UnityEngine.UI.Image ScoreImage;

        public WorkshopReadCommunityItem Data
        {
            get;
            private set;
        }

        private bool loading = false;
        private bool hasMouse = false;

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


        public void RegisterData(WorkshopReadCommunityItem data)
        {
            loading = true;
            Data = data;
            PreviewImage.texture = Data.previewImage;
            Title.text = Data.title;
            Subscribed.isOn = Data.isSubscribed;
            ScoreImage.fillAmount = Data.voteScore;

            loading = false;
        }

        private void Update()
        {
            if (PreviewImage.texture != Data.previewImage)
                PreviewImage.texture = Data.previewImage;

            if(hasMouse)
            {
                //We have the pointer so keep the tip with us even if we scroll
                TipTransform.position = SelfTransform.position + TipOffset;

                toggleGroup.alpha = 1;
                toggleGroup.interactable = true;
            }
            else
            {
                toggleGroup.alpha = 0;
                toggleGroup.interactable = false;
            }
        }

        public void SetSubscribe(bool subscribed)
        {
            if (loading)
                return;

            if(subscribed)
            {
                SteamUGC.SubscribeItem(Data.fileId);
            }
            else
            {
                SteamUGC.UnsubscribeItem(Data.fileId);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hasMouse = true;
            //Locate the tip to be to our right at our level
            TipTransform.position = SelfTransform.position + TipOffset;
            Description.text = Data.description.Replace("[b]", "<b>").Replace("[/b]", "</b>").Replace("[table]", "").Replace("[tr]", "").Replace("[td]", "").Replace("[/table]", "").Replace("[/tr]", "").Replace("[/td]", "").Replace("[h1]", "<b>").Replace("[/h1]", "</b>");
            TipRoot.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hasMouse = false;
            //Hide the tip
            TipRoot.SetActive(false);
        }
    }
}
#endif