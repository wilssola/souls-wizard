using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.Threading.Tasks;
using Firebase.Database;

namespace TecWolf.Gallery
{
    public class GalleryInterface : MonoBehaviour
    {

        public Button ListButton;
        public Button ExitButton;
        public Button RefreshButton;

        public static List<ImageItem> ImageItem = new List<ImageItem>();
        public static List<GameObject> ImageItems = new List<GameObject>();

        public RectTransform QuestTransform;
        public GameObject GalleryItem;
        public Text NoticeText;
        public bool Spawned;
        public int Type;

        private void Awake()
        {
            List();
        }

        private void Start()
        {
            ListButton.onClick.AddListener(delegate
            {
                List();
            });

            ExitButton.onClick.AddListener(delegate
            {
                Exit();
            });

            RefreshButton.onClick.AddListener(delegate
            {
                Refresh();
            });
        }

        private void Update()
        {
            if (Spawned)
            {
                NoticeText.text = "";
                RefreshButton.enabled = true;
            }
            else
            {
                RefreshButton.enabled = false;
                NoticeText.text = "PROCURANDO...";
                RefreshButton.transform.Rotate(Vector3.forward * Time.deltaTime * 100);
            }
        }

        public void Refresh()
        {
            Clear();

            List();

            Spawned = false;
        }

        public void List()
        {
            Clear();

            if (FirebaseController.SignedIn)
            {
                if (Type == 0)
                {
                    ListGallery(ImageItem, "nivel", TecWolf.Player.PlayerMission.Level.ToString());
                }
                else if (Type == 1)
                {
                    ListGallery(ImageItem, "local", TecWolf.Quest.QuestPointstop.PointstopID);
                }

                StartCoroutine(InstantiateList());
            }
        }

        private void Clear()
        {
            foreach (var Go in ImageItems)
            {
                Destroy(Go);
            }

            ImageItems.Clear();
            ImageItem.Clear();
        }

        private void Exit()
        {
            Spawned = false;
            Clear();
        }

        IEnumerator InstantiateList()
        {
            yield return new WaitForSecondsRealtime(1f);

            if (ImageItem.Count > 0 && !Spawned)
            {
                int RandomImage = Random.Range(0, ImageItem.Count);

                int Total = ImageItem.Count;

                if (Total > 8)
                {
                    Total = RandomImage + 8;
                }
                else
                {
                    RandomImage = 0;
                }

                for (int i = 0; i < ImageItem.Count; i++)
                {
                    var Object = Instantiate(GalleryItem, QuestTransform.transform, false);
                    var Component = Object.GetComponentInChildren<GalleryItem>();

                    Component.ImageID = ImageItem[i].ImageID;
                    Component.Mission = ImageItem[i].Mission;
                    Component.Local = ImageItem[i].Local;
                    Component.User = ImageItem[i].User;
                    Component.Link = ImageItem[i].Link;
                    Component.Format = ImageItem[i].Format;

                    Debug.Log("Missão Listada!");

                    ImageItems.Add(Object);
                }

                ImageItem.Clear();
                Spawned = true;
            }
        }

        public static void ListGallery(List<ImageItem> List, string Type, string Value)
        {
            FirebaseDatabase.DefaultInstance.GetReference("galeria-publica").GetValueAsync().ContinueWith(Task =>
            {
                if (Task.IsFaulted)
                {

                }
                else if (Task.IsCompleted)
                {
                    DataSnapshot Snapshot = Task.Result;

                    foreach (var ChildSnapshot in Snapshot.Children)
                    {
                        if (ChildSnapshot.Child(Type).Value.ToString() == Value)
                        {
                            var NewGallery = new ImageItem();

                            NewGallery.ImageID = ChildSnapshot.Key;

                            Debug.Log("Imagem obtida com sucesso.");

                            NewGallery.Link = ChildSnapshot.Child("link").Value.ToString();
                            NewGallery.Local = ChildSnapshot.Child("local").Value.ToString();
                            NewGallery.Mission = ChildSnapshot.Child("missao").Value.ToString();
                            NewGallery.User = ChildSnapshot.Child("usuario").Value.ToString();
                            NewGallery.Format = ChildSnapshot.Child("formato").Value.ToString();
                            NewGallery.Level = ChildSnapshot.Child("nivel").Value.ToString();

                            ImageItem.Add(NewGallery);
                            Debug.Log("Imagem obtida com sucesso.");
                        }
                    }
                }
            });
        }
    }
}