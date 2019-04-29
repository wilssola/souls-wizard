using UnityEngine;
using UnityEngine.UI;

namespace TecWolf.Monster
{
    public class MonsterFinalInterface : MonoBehaviour
    {
        public Dropdown HumourInput;
        public Text MonsterNotice;
        public static Text MonsterNoticeStatic;

        public GameObject MonsterUI;
        public static GameObject StaticMonsterUI;

        public GameObject[] MonstersModels;
        public static GameObject[] MonstersModelsStatic;

        public static string MonsterName;
        public static Color MonsterColor;

        private void Start()
        {
            StaticMonsterUI = MonsterUI;

            MonstersModelsStatic = new GameObject[MonstersModels.Length];

            for (int i = 0; i < MonstersModels.Length; i++)
            {
                MonstersModelsStatic[i] = MonstersModels[i];
            }

            MonsterNoticeStatic = MonsterNotice;
        }

        public static void Show()
        {
            foreach (GameObject Go in MonstersModelsStatic)
            {
                Go.SetActive(false);
            }

            // MonstersModelsStatic[Player.PlayerMission.Level].GetComponent<MeshRenderer>().material.SetColor("_Color", MonsterColor);

            if (Player.PlayerMission.Level > 0)
            {
                MonstersModelsStatic[Player.PlayerMission.Level - 1].SetActive(true);
            }
            else
            {
                MonstersModelsStatic[Player.PlayerMission.Level].SetActive(true);
            }

            // MonsterNoticeStatic.text = MonsterName + " FOI SUMONADO";

            System.SystemSound.Effect.PlayOneShot(System.SystemSound.SoundsStatic[2]);
        }

        public void HumourButton()
        {
            FirebaseController.WriteDataInt("/usuarios/" + FirebaseController.UserId, "humor", HumourInput.value);
        }
    }
}