using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase.Database;

using System;

namespace TecWolf.Quest
{
    public class QuestInterface : MonoBehaviour
    {
        public Button ListButton;
        public Button ExitButton;
        public Button RefreshButton;

        public static List<MissionItem> MissionItem = new List<MissionItem>();
        public static List<GameObject> MissionItems = new List<GameObject>();

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
                ListMission(TecWolf.Quest.QuestInterface.MissionItem, TecWolf.Player.PlayerMission.Level);

                StartCoroutine(InstantiateList());
            }
        }

        private void Clear()
        {
            foreach (var item in MissionItems)
            {
                Destroy(item);
            }

            MissionItems.Clear();
            MissionItem.Clear();
        }

        private void Exit()
        {
            Spawned = false;
            Clear();
        }

        IEnumerator InstantiateList()
        {
            yield return new WaitForSecondsRealtime(1f);

            if (MissionItem.Count > 0 && !Spawned)
            {
                for (int i = 0; i < MissionItem.Count; i++)
                {
                    var Object = Instantiate(QuestItem, QuestTransform.transform, false);
                    var Component = Object.GetComponentInChildren<QuestItem>();

                    Component.MissionID = MissionItem[i].MissionID;
                    Component.MissionName = MissionItem[i].Name;
                    Component.MissionType = MissionItem[i].Type;
                    Component.MissionValue = MissionItem[i].Value;
                    Component.MissionTotal = MissionItem[i].Total;
                    Component.MissionComplete = MissionItem[i].Complete;
                    Component.MissionVerify = MissionItem[i].Verify;

                    Component.MissionAchievementOne = MissionItem[i].AchievementOne;
                    Component.MissionAchievementTwo = MissionItem[i].AchievementTwo;

                    Component.Difficulty = MissionItem[i].Difficulty;

                    MissionItems.Add(Object);
                }

                Debug.Log("Missões Listadas!");

                MissionItem.Clear();
                Spawned = true;
            }
        }

        public static void ListMission(List<MissionItem> List, int MonsterID)
        {
            FirebaseDatabase.DefaultInstance.GetReference("/usuarios/" + FirebaseController.UserId + "/missoes/").GetValueAsync().ContinueWith(Task =>
            {
                if (Task.IsFaulted)
                {

                }
                else if (Task.IsCompleted)
                {
                    DataSnapshot Snapshot = Task.Result;

                    foreach (var ChildSnapshot in Snapshot.Children)
                    {
                        if (Convert.ToInt32(ChildSnapshot.Child("nivel").Value) == MonsterID)
                        {
                            var NewMission = new MissionItem();

                            NewMission.MissionID = ChildSnapshot.Key;

                            Debug.Log(ChildSnapshot.Child("objetivo").Value.ToString());

                            NewMission.Name = ChildSnapshot.Child("objetivo").Value.ToString();
                            NewMission.Type = ChildSnapshot.Child("tipo").Value.ToString();
                            NewMission.Value = ChildSnapshot.Child("valor").Value.ToString();
                            NewMission.Total = ChildSnapshot.Child("total").Value.ToString();
                            NewMission.Verify = ChildSnapshot.Child("verificada").Value.ToString();
                            NewMission.Complete = ChildSnapshot.Child("concluida").Value.ToString();

                            NewMission.AchievementOne = ChildSnapshot.Child("conquista").Value.ToString();
                            NewMission.AchievementTwo = ChildSnapshot.Child("conquista_final").Value.ToString();

                            NewMission.Difficulty = ChildSnapshot.Child("dificuldade").Value.ToString();

                            MissionItem.Add(NewMission);
                            Debug.Log("Missão adicionada a Lista de Missões.");
                        }
                    }
                }
            });
        }
    }
}