using UnityEngine;
using UnityEngine.UI;

namespace TecWolf.Mask
{
    public class MaskInput : MonoBehaviour
    {
        public InputField Input;

        public void InputDate()
        {
            int Number = 0;

            if (int.TryParse(Input.text.Replace("/", ""), out Number))
            {
                if (Input.text.Length == 2 || Input.text.Length == 5)
                {
                    Input.text += "/";

                    Input.caretPosition = Input.text.Length;
                }
            }
            else
            {
                Input.text = "";
            }
        }
    }
}