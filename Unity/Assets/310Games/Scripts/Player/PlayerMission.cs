using UnityEngine;

namespace TecWolf.Player
{
    public class PlayerMission : MonoBehaviour
    {
        public static int Level = 9;
        public static int Difficulty = 0;

        public static bool InMission = true;
        public static bool LevelChange;

        public static string FinalAchievement;

        private void Update()
        {
            UpdateLevel();
        }

        public void UpdateLevel()
        {
            if (FirebaseController.SignedIn)
            {
                FirebaseController.CheckUserCharacter(false);
                FirebaseController.CheckMission(Level);
            }

            if (LevelChange)
            {
                int LocalLevel = Level;
                System.SystemInterface.Alert("Você passou para o Nível " + (LocalLevel + 1).ToString() + ".");
                LevelChange = false;
            }
        }
    }
}