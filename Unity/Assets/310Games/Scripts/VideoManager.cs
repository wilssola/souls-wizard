using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Video;

using System;


namespace TecWolf.Video
{
    public class VideoManager : MonoBehaviour
    {

        private bool Played, CutscenePlayed;

        [Header("Tutorial")]
        public GameObject[] Video;

        [Header("Cutscene")]
        public GameObject[] CutsceneObject;
        public VideoPlayer CutscenePlayer;
        public VideoClip[] CutsceneClip;

        public GameObject Music;

        public static bool CutscenePlay;

        // Use this for initialization
        void Start()
        {
            CutscenePlayer.loopPointReached += CheckOver;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(CutscenePlay);

            if(CutscenePlay)
            {
                foreach (GameObject Go in CutsceneObject)
                {
                    Go.SetActive(true);
                }

                if (!CutscenePlayer.isPlaying)
                {
                    CutscenePlayer.clip = CutsceneClip[Player.PlayerCharacter.PlayerGender];
                    CutscenePlayer.Play();
                }

                Music.SetActive(false);
            }
            else
            {
                foreach (GameObject Go in CutsceneObject)
                {
                    Go.SetActive(false);
                }

                Music.SetActive(true);
            }

            if (FirebaseController.CreatedIn && FirebaseController.SignedIn && !CutscenePlay)
            {
                if (Player.PlayerMission.InMission || Player.PlayerMission.Level > 0)
                {
                    Played = true;
                }

                if (!Played && !Player.PlayerMission.InMission && Player.PlayerMission.Level == 0)
                {
                    foreach(GameObject Go in Video)
                    {
                        Go.SetActive(true);
                    }
                }
                else if (Played)
                {
                    foreach (GameObject Go in Video)
                    {
                        Go.SetActive(false);
                    }
                }
            }
        }

        private void CheckOver(VideoPlayer Component)
        {
            CutscenePlay = false;
        }

        public void Skip()
        {
            CutscenePlay = false;
        }
    }
}
