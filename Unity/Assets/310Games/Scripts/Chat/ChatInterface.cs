using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TecWolf.Chat
{
    /// <summary>
    /// Interface para o Chat do jogo.
    /// </summary>
    public class ChatInterface : MonoBehaviour
    {
        public RectTransform ChannelPainel, ChatPainel, CreaterChannelPainel;

        private Vector2 CreaterChannelPainelPos;
        private bool CreaterChannelPainelISOpen = false;

        public static Channel ChannelReferenceSelected;

        public Text UserName;
        public Text ChatName;

        public InputField NewChannelName;
        public InputField ChatInput;

        public Button OpenCreateChannelButton;
        public Button CreateChannelButton;
        public Button RefreshChannelButton;

        public Button SendMessages;

        public Button ReturnChat;

        public GameObject ChannelContent;
        public GameObject ChatContent;

        public static GameObject ChannelContentStatic;
        public static GameObject ChatContentStatic;

        public GameObject ItemChannelPrefab;
        public GameObject ItemMessagePrefab;

        public static GameObject ItemChannelPrefabStatic;
        public static GameObject ItemMessagePrefabStatic;

        public static List<GameObject> ChannelItems = new List<GameObject>();
        public static List<GameObject> MessageItems = new List<GameObject>();
        public static List<string> MessageItemsTest = new List<string>();
        public static List<Channel> Channels = new List<Channel>();

        private void Start()
        {
            ItemChannelPrefabStatic = ItemChannelPrefab;
            ItemMessagePrefabStatic = ItemMessagePrefab;

            ChannelContentStatic = ChannelContent;
            ChatContentStatic = ChatContent;

            ConfigButtons();
        }

        private void OnEnable()
        {
            ChatItem.OnClickOpen += OpenChatPainel;
        }

        /// <summary>
        /// Setar as configurações de todos os botões do Chat.
        /// </summary>
        public void ConfigButtons()
        {
            OpenCreateChannelButton.onClick.AddListener(delegate
            {
                CreaterChannelPainelISOpen = !CreaterChannelPainelISOpen;
                CreaterChannelPainel.gameObject.SetActive(CreaterChannelPainelISOpen);
            });

            CreateChannelButton.onClick.AddListener(delegate
            {
                if (!string.IsNullOrEmpty(NewChannelName.text))
                {
                    FirebaseController.CreaterChannelDataBase(NewChannelName.text, FirebaseController.UserName);
                    NewChannelName.text = "";
                    FirebaseController.GetChannelsDatabase(Channels);

                    CreaterChannelPainelISOpen = !CreaterChannelPainelISOpen;
                    CreaterChannelPainel.gameObject.SetActive(CreaterChannelPainelISOpen);
                }
            });

            RefreshChannelButton.onClick.AddListener(delegate
            {
                FirebaseController.GetChannelsDatabase(Channels);
            });

            SendMessages.onClick.AddListener(delegate
            {
                if (!string.IsNullOrEmpty(ChatInput.text))
                {
                    FirebaseController.CreateMessage(ChannelReferenceSelected.ChanneId, FirebaseController.UserName, ChatInput.text);
                    ChatInput.text = "";
                }
            });

            ReturnChat.onClick.AddListener(delegate
            {
                foreach (var item in MessageItems) Destroy(item);
                MessageItems.Clear();
                ChatPainel.gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// Obter canais do Chat a partir do Firebase.
        /// </summary>
        public void UpdateChannelsFromDataBase()
        {
            FirebaseController.GetChannelsDatabase(Channels);
        }

        /// <summary>
        /// Instaciar os canais disponiveis no Chat.
        /// </summary>
        public static void SpawnChannelsButtons()
        {
            for (int i = 0; i < Channels.Count; i++)
            {
                if (ChannelItems.Any(channelButton => channelButton.GetComponent<ChatItem>().ChannelReference.ChanneId == Channels[i].ChanneId)) continue;
                var button = Instantiate(ItemChannelPrefabStatic, ChannelContentStatic.transform, false);
                var buttonComp = button.GetComponent<ChatItem>();
                buttonComp.ChannelReference = Channels[i];
                buttonComp.SetChannelInfos();
                buttonComp.ChannelNumber.text = i.ToString();
                ChannelItems.Add(button);
            }
        }

        /// <summary>
        /// Instaciar as mensagens enviadas no Chat.
        /// </summary>
        public static void SpawnChatMessages()
        {
            for (int i = 0; i < ChannelReferenceSelected.Messages.Count; i++)
            {
                if (MessageItemsTest.Any(messageItem => messageItem == ChannelReferenceSelected.Messages[i].Text)) continue;
                MessageItemsTest.Add(ChannelReferenceSelected.Messages[i].Text);
                Debug.Log(ChannelReferenceSelected.Messages[i].Text);

                var message = Instantiate(ItemMessagePrefabStatic, ChatContentStatic.transform, false);
                var messageComp = message.GetComponentInChildren<Text>();
                messageComp.text = ChannelReferenceSelected.Messages[i].SenderID + ": " + ChannelReferenceSelected.Messages[i].Text;
                MessageItems.Add(message);
            }
        }

        /// <summary>
        /// Limpar tudo que foi instanciado no Chat.
        /// </summary>
        public void ClearAplication()
        {
            foreach (var item in ChannelItems)
            {
                Destroy(item);
            }
            foreach (var item in MessageItems)
            {
                Destroy(item);
            }
            ChannelItems.Clear();
            MessageItems.Clear();
        }

        /// <summary>
        /// Abrir o painel do Chat.
        /// </summary>
        private void OpenChatPainel(Channel Value)
        {
            ChatPainel.gameObject.SetActive(true);
            ChannelReferenceSelected = Value;
            FirebaseController.ObservingChatMessages(ChannelReferenceSelected);
            ChatName.text = ChannelReferenceSelected.Title;
        }

    }
}
