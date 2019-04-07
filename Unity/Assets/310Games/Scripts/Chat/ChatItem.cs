using UnityEngine;
using UnityEngine.UI;

namespace TecWolf.Chat
{
    public class ChatItem : MonoBehaviour
    {
        public Text ChannelNumber, ChannelTitle, ChannelCreator;
        public Button ChannelButton;

        public delegate void ClickAction(Channel Value);
        public static event ClickAction OnClickOpen;

        public Channel ChannelReference;

        private void Start()
        {
            ChannelButton.onClick.AddListener(delegate
            {
                OnClickOpen(ChannelReference);
            });
        }

        public void SetChannelInfos()
        {
            ChannelTitle.text = ChannelReference.Title;
            ChannelCreator.text = ChannelReference.Creator;
        }
    }
}
