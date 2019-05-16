using UnityEngine;
using UnityEngine.UI;
using System;

namespace TecWolf.Player
{
    public class PlayerInterface : MonoBehaviour
    {

        public Text FramesText, SpeedText;

        public static GameObject PointstopBar, MicInterface;

        public Image LevelBar;
        public Text LevelText;

        private void Start()
        {
            PointstopBar = GameObject.Find("PointstopBar");
            MicInterface = GameObject.Find("Mic (Enabled)");

            PointstopBar.SetActive(false);
            MicInterface.SetActive(false);
        }

        private void Update()
        {
            FramesText.text = (int)(1.0f / Time.smoothDeltaTime) + " FPS"; // SimpleFixDateTime(PlayerController.Date.Day.ToString()) + "/" + SimpleFixDateTime(PlayerController.Date.Month.ToString()) + "/" + FixDateTime(PlayerController.Date.Year.ToString());

            if (PlayerController.Speed != 0)
            {
                SpeedText.text = PlayerController.Speed.ToString().Substring(0, 2).Replace(",", "") + " KM/H";
            }
            else
            {
                SpeedText.text = "0 KM/H";
            }

            int Level = PlayerMission.Level;

            LevelBar.fillAmount = ((Level + 1) * 0.1f);
            LevelText.text = "NÍVEL " + (Level + 1).ToString();
        }

        public string SimpleFixDateTime(string Time)
        {
            if (Convert.ToInt32(Time) < 10)
            {
                Time = "0" + Time;
            }

            return Time;
        }

        public void CallEmergency()
        {
            Application.OpenURL("tel:911");
        }

    }
}