using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TecWolf.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        public bool Moving;
        public Animator[] Animation;

        void Update()
        {

            if (PlayerController.PlayerModelActive)
            {
                Animation[PlayerCharacter.PlayerGender].SetBool("Moving", Moving);
            }
        }
    }
}
