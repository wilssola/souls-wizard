using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Collections;

namespace TecWolf.Popup
{
    public class PopupManager : MonoBehaviour
    {
        private GameObject CanvasPrefab;
        private GameObject ButtonPrefab;
        private GameObject EventSystem;
        private string Message;
        private Color MessageColor;
        private Color BackgroundColor;
        private List<PopupButton> Buttons = new List<PopupButton>();
        private static PopupManager InstancePrivate;

        public static PopupManager Instance
        {
            get
            {
                if (InstancePrivate == null)
                    InstancePrivate = GameObject.FindObjectOfType<PopupManager>();
                return InstancePrivate;
            }
        }

        void Awake()
        {
            CanvasPrefab = (GameObject)Instantiate(Resources.Load("TecWolf/Popup/Canvas", typeof(GameObject)));
            CanvasPrefab.GetComponent<CanvasGroup>().alpha = 0.0f;
            CanvasPrefab.GetComponent<CanvasGroup>().interactable = false;
            CanvasPrefab.GetComponent<CanvasGroup>().blocksRaycasts = false;
            CanvasPrefab.transform.name = "Popup";
            ButtonPrefab = (GameObject)Resources.Load("TecWolf/Popup/Button", typeof(GameObject));

            EventSystem[] Objects = (EventSystem[])GameObject.FindObjectsOfType<EventSystem>();
            if (Objects.Length == 0)
            {
                EventSystem = new GameObject("PopupEventSystem", typeof(EventSystem));
                EventSystem.AddComponent<StandaloneInputModule>().forceModuleActive = true;
            }
            else
            {
                Objects[0].gameObject.GetComponent<StandaloneInputModule>().forceModuleActive = true;
            }

        }

        public void CreatePopup(string Message)
        {
            CreatePopup(Message, Color.white, Color.black);
        }

        public void CreatePopup(string Message, Color MessageColor, Color BackgroundColor)
        {
            DestroyPreviousButtons();
            this.Message = Message;
            this.MessageColor = MessageColor;
            this.BackgroundColor = BackgroundColor;
        }

        public void AddButton(string ButtonText, Action OnClickEvent)
        {
            AddButton(ButtonText, Color.black, Color.white, OnClickEvent);
        }

        public void AddButton(string ButtonText, Color ButtonTextColor, Color ButtonBackgroundColor, Action OnClickEvent)
        {
            Buttons.Add(new PopupButton(ButtonText, ButtonTextColor ,ButtonBackgroundColor, CanvasPrefab.transform.GetChild(0), ButtonPrefab, delegate { OnClickEvent(); HidePopup(); }));
        }

        public void ShowPopup()
        {
            foreach(PopupButton Button in Buttons)
            {
                Button.Create();
            }
            SetPanel();
            StartCoroutine(ShowPopup(CanvasPrefab.GetComponent<CanvasGroup>(), 0.5f));
        }

        public void HidePopup()
        {
            StartCoroutine(HidePopup(CanvasPrefab.GetComponent<CanvasGroup>(), 0.5f));
        }

        IEnumerator ShowPopup(CanvasGroup CanvasGroup, float Second)
        {
            CanvasPrefab.GetComponent<Image>().raycastTarget = true;
            CanvasGroup.alpha = 0.0f;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = true;
            while (CanvasGroup.alpha < 1.0f)
            {
                CanvasGroup.alpha += Time.deltaTime * (1.0f / Second); ;
                yield return null;
            }
            CanvasGroup.alpha = 1.0f;
            CanvasGroup.interactable = true;
        }
        IEnumerator HidePopup(CanvasGroup CanvasGroup, float Second)
        {
            CanvasGroup.interactable = false;
            CanvasGroup.alpha = 1.0f;
            while (CanvasGroup.alpha > .0f)
            {
                CanvasGroup.alpha -= Time.deltaTime * (1.0f / Second);
                yield return null;
            }
            CanvasGroup.alpha = 0.0f;
            CanvasPrefab.GetComponent<Image>().raycastTarget = false;
            CanvasGroup.blocksRaycasts = false;
        }

        private void SetPanel()
        {
            CanvasPrefab.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = Message;
            CanvasPrefab.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().color = MessageColor;
            CanvasPrefab.transform.GetChild(0).GetComponent<Image>().color = BackgroundColor;
        }
        private void DestroyPreviousButtons()
        {
            foreach(PopupButton Button in Buttons)
            {
                Button.Destroy();
            }
            Buttons.Clear();
        }
    }
}