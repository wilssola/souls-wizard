using UnityEngine;

namespace TecWolf.System
{
    public class SystemSound : MonoBehaviour
    {
        public static AudioSource Music;
        public static AudioSource Effect;

        public AudioClip[] Sounds;
        public static AudioClip[] SoundsStatic;

        public AudioClip[] MonsterSounds;
        public AudioClip[] MonsterSoundsFinal;
        public static AudioClip[] MonsterSoundsStatic;

        private void Start()
        {
            SetSound();
        }

        private void Update()
        {
            if (Player.PlayerMission.Level < MonsterSoundsStatic.Length)
            {
                MonsterSoundsStatic[0] = MonsterSounds[Player.PlayerMission.Level];
                MonsterSoundsStatic[1] = MonsterSoundsFinal[Player.PlayerMission.Level];
            }
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

            MonsterSoundsStatic = new AudioClip[MonsterSounds.Length];
        }
    }
}
