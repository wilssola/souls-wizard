using UnityEngine;

namespace TecWolf.Player
{
    public class PlayerCharacter : MonoBehaviour
    {
        public static int PlayerGender;
        public static int PlayerSkin;
        public static Color[] PlayerColorStatic;
        public Color[] PlayerColor;

        public GameObject[] PlayerModelsFemale;
        public GameObject[] PlayerModelsMale;

        public GameObject[] PlayerModelsFemaleSkin;
        public GameObject[] PlayerModelsMaleSkin;

        public static Material[] PlayerSkinMaterialStatic;
        public Material[] PlayerSkinMaterial;

        static public bool Collision;

        private void Start()
        {
            PlayerColorStatic = new Color[PlayerColor.Length];
            PlayerSkinMaterialStatic = new Material[PlayerSkinMaterial.Length];
        }

        private void Update()
        {
            if (!Collision)
            {
                ApplyCharacter();
            }
        }

        public void ApplyCharacter()
        {
            for (int i = 0; i < PlayerColor.Length; i++)
            {
                PlayerColorStatic[i] = PlayerColor[i];
            }
            for (int i = 0; i < PlayerSkinMaterial.Length; i++)
            {
                PlayerSkinMaterialStatic[i] = PlayerSkinMaterial[i];
            }

            foreach (Material Go in PlayerSkinMaterial)
            {
                Go.SetColor("_Color", PlayerColorStatic[PlayerSkin]);
            }

            if (Player.PlayerMission.Level == 0 && PlayerController.PlayerModelActive)
            {

                foreach (GameObject Go in PlayerModelsMale)
                {
                    if (PlayerGender == 0)
                    {
                        if (!Go.activeSelf)
                        {
                            Go.SetActive(true);
                        }
                    }
                    else
                    {
                        Go.SetActive(false);
                    }
                }

                foreach (GameObject Go in PlayerModelsFemale)
                {
                    if (PlayerGender == 1)
                    {
                        if (!Go.activeSelf)
                        {
                            Go.SetActive(true);
                        }
                    }
                    else
                    {
                        Go.SetActive(false);
                    }
                }

                DisableModelsSkins();
            }
            else if(PlayerController.PlayerModelActive)
            {

                foreach (GameObject Go in PlayerModelsMaleSkin)
                {
                    if (PlayerGender == 0)
                    {
                        if (!Go.activeSelf)
                        {
                            Go.SetActive(true);
                        }
                    }
                    else
                    {
                        Go.SetActive(false);
                    }
                }

                foreach (GameObject Go in PlayerModelsFemaleSkin)
                {
                    if (PlayerGender == 1)
                    {
                        if (!Go.activeSelf)
                        {
                            Go.SetActive(true);
                        }
                    }
                    else
                    {
                        Go.SetActive(false);
                    }
                }

                DisableModels();
            }

            if(!PlayerController.PlayerModelActive)
            {

                DisableModels();
                DisableModelsSkins();

            }
        }

        private void DisableModels()
        {
            foreach (GameObject Go in PlayerModelsMale)
            {
                Go.SetActive(false);
            }

            foreach (GameObject Go in PlayerModelsFemale)
            {
                Go.SetActive(false);
            }
        }

        private void DisableModelsSkins()
        {
            foreach (GameObject Go in PlayerModelsMaleSkin)
            {
                Go.SetActive(false);
            }

            foreach (GameObject Go in PlayerModelsFemaleSkin)
            {
                Go.SetActive(false);
            }
        }
    }
}