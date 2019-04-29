using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterType : MonoBehaviour {

    public GameObject[] Monsters;

    private bool Checked;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (FirebaseController.SignedIn && FirebaseController.CreatedIn && !Checked)
        {
            foreach (GameObject Go in Monsters)
            {
                Go.SetActive(false);
            }

            if (!TecWolf.Player.PlayerMission.InMission)
            {
                Monsters[TecWolf.Player.PlayerMission.Level].SetActive(true);

                Checked = true;
            }
        }
	}
}
