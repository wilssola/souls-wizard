using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterType : MonoBehaviour
{

    public GameObject[] Monsters;

    private bool Checked;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (FirebaseController.SignedIn && /* FirebaseController.CreatedIn && */ !Checked && TecWolf.Player.PlayerMission.Level < Monsters.Length)
        {
            for (int i = 0; i < Monsters.Length; i++)
            {
                if (i != TecWolf.Player.PlayerMission.Level)
                {
                    Monsters[i].SetActive(false);
                }
            }

            if (!TecWolf.Player.PlayerMission.InMission)
            {
                Monsters[TecWolf.Player.PlayerMission.Level].SetActive(true);
            }
            else
            {
                Monsters[TecWolf.Player.PlayerMission.Level].SetActive(false);
            }
        }

        if(TecWolf.Player.PlayerMission.Level == 9)
        {
            foreach(GameObject Go in Monsters)
            {
                Go.SetActive(false);
            }
        }
    }
}
