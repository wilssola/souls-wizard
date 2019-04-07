using UnityEngine;

namespace TecWolf.System
{
    public class SystemSound : MonoBehaviour
    {
        public static AudioSource Music;
        public static AudioSource Effect;

        public AudioClip[] Sounds;
        public static AudioClip[] SoundsStatic;

        private void Start()
        {
            SetSound();
        }

        public void SetSound()
        {
            Music = FindObjectsOfType<AudioSource>()[0];
            Effect = FindObjectsOfType<AudioSource>()[1];

            SoundsStatic = new AudioClip[Sounds.Length];

            for (int i = 0; i < Sounds.Length; i++)
            {
                SoundsStatic[i] = Sounds[i];
            }
        }
    }
}
