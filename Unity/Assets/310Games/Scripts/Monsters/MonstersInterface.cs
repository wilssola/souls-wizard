using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TecWolf.Monsters
{
    public class MonstersInterface : MonoBehaviour
    {
        public static List<GameObject> MonsterItems = new List<GameObject>();

        public List<MonsterItem> MonsterItem = new List<MonsterItem>();

        public Button ListButton;
        public Button ExitButton;
        public Button RefreshButton;

        public GameObject QuestItem;

        public Text NoticeText;

        public RectTransform QuestTransform;

        public bool Spawned;

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
                StartCoroutine(InstantiateList());
            }
        }

        private void Clear()
        {
            foreach (var Item in MonsterItems)
            {
                Destroy(Item);
            }

            MonsterItems.Clear();
        }

        private void Exit()
        {
            Spawned = false;
            Clear();
        }

        IEnumerator InstantiateList()
        {
            yield return new WaitForSecondsRealtime(1f);

            if (MonsterItem.Count > 0 && !Spawned)
            {
                for (int i = 0; i < MonsterItem.Count; i++)
                {
                    var Object = Instantiate(QuestItem, QuestTransform.transform, false);
                    var Component = Object.GetComponentInChildren<MonstersItem>();

                    Component.MonsterName.text = MonsterItem[i].MonsterName;

                    Component.MonsterLevel = MonsterItem[i].MonsterLevel;

                    if (Convert.ToInt32(MonsterItem[i].MonsterLevel) < TecWolf.Player.PlayerMission.Level && TecWolf.Player.PlayerMission.Level != 10)
                    {
                        Component.MonsterImage.sprite = MonsterItem[i].MonsterVisible;
                    }
                    else
                    {
                        Component.MonsterImage.sprite = MonsterItem[i].MonsterInvisible;
                    }

                    MonsterItems.Add(Object);
                }
            }

            Debug.Log("Monstros Listados!");

            Spawned = true;
        }
    }
}