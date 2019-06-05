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

        public static string[] MonsterTextsStatic;
        public string[] MonsterTexts;

        private void Start()
        {
            StaticMonsterUI = MonsterUI;

            MonstersModelsStatic = new GameObject[MonstersModels.Length];

            for (int i = 0; i < MonstersModels.Length; i++)
            {
                MonstersModelsStatic[i] = MonstersModels[i];
            }

            MonsterTextsStatic = new string[MonsterTexts.Length];

            for (int i = 0; i < MonsterTexts.Length; i++)
            {
                MonsterTextsStatic[i] = MonsterTexts[i];
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

            MonstersModelsStatic[Player.PlayerMission.Level].SetActive(true);
            
            MonsterNoticeStatic.text = MonsterTextsStatic[Player.PlayerMission.Level];

            System.SystemSound.Effect.PlayOneShot(System.SystemSound.MonsterSoundsStatic[1]);

            FirebaseController.WriteDataInt("/usuarios/" + FirebaseController.UserId, "humor", 2);
        }

        public void HumourButton()
        {
            FirebaseController.WriteDataInt("/usuarios/" + FirebaseController.UserId, "humor", HumourInput.value);
        }
    }
}