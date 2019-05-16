using System.Collections.Generic;
using UnityEngine;
using System;

namespace TecWolf.Monster
{
    public class MonsterMission : MonoBehaviour
    {
        public int MonsterID = 0;
        public string MonsterName = "Monstro";
        public static List<Mission> Missions = new List<Mission>();

        private bool Spawned;

        private void OnMouseUpAsButton()
        {
            if (FirebaseController.SignedIn && !TecWolf.Player.PlayerMission.InMission)
            {
                if (!Spawned)
                {
                    FirebaseController.GetMission(Missions, MonsterID);
                }
                else if (Missions.Count > 0 && Spawned)
                {
                    for (int i = 0; i < Missions.Count; i++)
                    {
                        string Path = "/usuarios/" + FirebaseController.UserId + "/missoes/" + Missions[i].MissionID;

                        FirebaseController.WriteDataString(Path, "objetivo", Missions[i].Name);
                        FirebaseController.WriteDataInt(Path, "tipo", Convert.ToInt32(Missions[i].Type));
                        FirebaseController.WriteDataInt(Path, "valor", Convert.ToInt32(Missions[i].Value));

                        FirebaseController.WriteDataInt(Path, "nivel", Convert.ToInt32(Missions[i].Monster));

                        FirebaseController.WriteDataString(Path, "conquista", Missions[i].AchievementOne);
                        FirebaseController.WriteDataString(Path, "conquista_final", Missions[i].AchievementTwo);

                        FirebaseController.WriteDataInt(Path, "dificuldade", Convert.ToInt32(Missions[i].Difficulty));

                        FirebaseController.WriteDataInt(Path, "total", 0);
                        FirebaseController.WriteDataBool(Path, "concluida", false);
                        FirebaseController.WriteDataBool(Path, "verificada", false);

                        MonsterInterface.MonsterName = MonsterName;
                        // MonsterInterface.MonsterColor = GetComponent<MeshRenderer>().material.GetColor("_Color");
                    }

                    MonsterInterface.StaticMonsterUI.SetActive(true);
                    MonsterInterface.Show();

                    Debug.Log("Missões Adquiridas!");

                    Missions.Clear();

                    if (GetComponent<MonsterController>().First != true)
                    {
                        gameObject.SetActive(false);
                    }
                }

                Spawned = true;
            }
        }
    }
}
