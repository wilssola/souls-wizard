using UnityEngine;
using UnityEngine.UI;
using System;

namespace TecWolf.Popup
{
    class PopupButton
    {
        private string Text;
        private Color TextColor;
        private Color BackgroundColor;
        private GameObject Object;
        private Transform ParentTransform;
        private GameObject Prefab;
        private Action OnClickEvent;

        public PopupButton()
        {
        }

        public PopupButton(string text, Color textColor, Color backgroundColor, Transform parentTransform, GameObject prefab, Action onClickEvent)
        {
            this.Text = text;
            this.TextColor = textColor;
            this.BackgroundColor = backgroundColor;
            this.ParentTransform = parentTransform;
            this.Prefab = prefab;
            this.OnClickEvent = onClickEvent;
        }

        public void Create()
        {
            Object = (GameObject)GameObject.Instantiate(Prefab);
            Object.transform.SetParent(ParentTransform, false);
            Object.transform.localScale = new Vector3(1, 1, 1);
            Object.GetComponent<Button>().onClick.AddListener(() => { OnClickEvent(); });
            Object.GetComponent<Image>().color = BackgroundColor;
            if (Object.transform.childCount != 0)
            {
                Object.transform.GetChild(0).GetComponent<Text>().text = Text;
                Object.transform.GetChild(0).GetComponent<Text>().color = TextColor;
            }
        }

        public void Destroy()
        {
            if (Object)
            {
                GameObject.Destroy(Object);
            }
        }

    }
}
