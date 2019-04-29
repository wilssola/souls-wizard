using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TecWolf.Video
{
    public class VideoTutorial : MonoBehaviour
    {

        private bool Played;

        public GameObject[] Video;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            if (FirebaseController.CreatedIn && FirebaseController.SignedIn)
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
    }
}
