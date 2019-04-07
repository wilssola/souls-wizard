using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TecWolf.Settings
{
    public class SettingsInterface : MonoBehaviour
    {
        public static bool Buildings = false;
        public static bool Sounds = true;

        private void Start()
        {
            Sounds = Convert.ToBoolean(PlayerPrefs.GetInt("Sounds", 1));
            Buildings = Convert.ToBoolean(PlayerPrefs.GetInt("Buildings", 0));
        }

        public void Update()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SignOut();
            }
        }

        public void SignOut()
        {
            FirebaseController.AuthQuit();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Sound()
        {
            Sounds = !Sounds;

            PlayerPrefs.SetInt("Sounds", Convert.ToInt32(Sounds));

            AudioSource[] Audio = FindObjectsOfType<AudioSource>();

            foreach (AudioSource Go in Audio)
            {
                Go.enabled = Sounds;
            }
        }

        public void Building()
        {
            Buildings = !Buildings;

            PlayerPrefs.SetInt("Buildings", Convert.ToInt32(Buildings));
        }
    }
}
