using UnityEngine;
using TecWolf.Popup;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Firebase.Database;

using UnityEngine.SceneManagement;

namespace TecWolf.System
{
    public class SystemInterface : MonoBehaviour
    {
        public GameObject PlayerUI;
        public GameObject SystemUI;
        public GameObject AuthUI;
        public GameObject LoadUI;
        public GameObject AuthLoginUI;
        public GameObject AuthCreateUI;
        public GameObject AuthCharacterUI;

        public Image LoadBar;

        public InputField AuthLoginEmail;
        public InputField AuthLoginPassword;

        public InputField AuthCreateName;
        public InputField AuthCreateEmail;
        public InputField AuthCreatePassword;

        public Dropdown AuthCharacterColor;
        public Image[] AuthCharacterButtons;
        public Color[] AuthCharacterColors;
        public Material[] AuthCharacterSkin;

        private int Sexo, Cor;
        private bool CharacterOpened;

        private bool Loaded;
        private float LoadTimer, LoadTimerLimit = 10f;

        public QuestionItem[] Questions;

        public Toggle MainType;

        public GameObject Type;
        public Toggle[] CurrentTyper;

        public Dropdown Difficulty;

        private void Update()
        {
            LoadTimer += Time.deltaTime;

            if (LoadTimer >= LoadTimerLimit && !Loaded)
            {
                LoadUI.SetActive(false);
                AuthUI.SetActive(true);

                Loaded = true;
            }

            LoadBar.fillAmount = LoadTimer * LoadTimerLimit / 100f;

            if (FirebaseController.SignedIn)
            {
                AuthLoginUI.SetActive(false);
                AuthCreateUI.SetActive(false);

                if (!CharacterOpened)
                {
                    StartCoroutine(OpenCharacter());
                    CharacterOpened = true;
                }
            }

            if (FirebaseController.SignedIn && FirebaseController.CreatedIn && Loaded)
            {
                SystemUI.SetActive(false);
            }

            if (MainType.isOn)
            {
                Type.gameObject.SetActive(false);
            }
            else
            {
                Type.gameObject.SetActive(true);
            }


            if (FirebaseController.SignedIn && FirebaseController.CreatedIn && Player.PlayerMission.Level == 10)
            {
                // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        public void AuthLoginButton()
        {
            if (AuthLoginEmail.text != "" && AuthLoginPassword.text != "")
            {
                FirebaseController.LoginAuth(AuthLoginEmail.text, AuthLoginPassword.text);
            }
        }

        public void AuthCreateButton()
        {
            if (AuthCreateEmail.text != "" && AuthCreatePassword.text != "" && AuthCreateName.text != "")
            {
                FirebaseController.CreateAuth(AuthCreateEmail.text, AuthCreatePassword.text, AuthCreateName.text);
            }
        }

        public void AuthCharacterButton(int sexo)
        {
            foreach (Image Go in AuthCharacterButtons)
            {
                Go.color = Color.white;
            }

            AuthCharacterButtons[sexo].color = Color.gray;

            Sexo = sexo;
            Player.PlayerCharacter.PlayerGender = sexo;

            FirebaseController.WriteDataInt("/usuarios/" + FirebaseController.UserId + "/personagem/", "sexo", sexo);
        }

        public void AuthCharacterSkinButton()
        {
            Cor = AuthCharacterColor.value;
            Player.PlayerCharacter.PlayerSkin = AuthCharacterColor.value;

            FirebaseController.WriteDataInt("/usuarios/" + FirebaseController.UserId + "/personagem/", "cor", AuthCharacterColor.value);
        }

        public void AuthCharacterCreateButton()
        {
            if (FirebaseController.SignedIn && !FirebaseController.CreatedIn)
            {
                string Path = "/usuarios/" + FirebaseController.UserId + "/personagem/";
                string QuestionPath = "/usuarios/" + FirebaseController.UserId + "/informacao/";

                FirebaseController.WriteDataInt(Path, "sexo", Sexo);
                FirebaseController.WriteDataInt(Path, "cor", Cor);
                FirebaseController.WriteDataInt(Path, "nivel", 0);
                FirebaseController.WriteDataBool(Path, "criado", true);

                if(Questions[0].QuestionInput.text.Length != 10)
                {
                    Questions[0].QuestionInput.text = "";
                }

                foreach (var Go in Questions)
                {
                    string Key = FirebaseDatabase.DefaultInstance.GetReference("/usuarios/" + FirebaseController.UserId + "/informacao/").Push().Key;

                    FirebaseController.WriteDataString(QuestionPath + Key, "pergunta", Go.QuestionText.text);

                    if (Go.QuestionInput.text != "")
                    {
                        FirebaseController.WriteDataString(QuestionPath + Key, "resposta", Go.QuestionInput.text);
                    }
                    else
                    {
                        FirebaseController.WriteDataString(QuestionPath + Key, "resposta", "Sem Resposta");
                    }
                }

                if (!MainType.isOn)
                {
                    for (int i = 0; i < CurrentTyper.Length; i++)
                    {
                        if (CurrentTyper[i].isOn)
                        {
                            int TypeValue = CurrentTyper[i].GetComponent<TypeItem>().MonsterLevel;
                            string Key = FirebaseDatabase.DefaultInstance.GetReference("/usuarios/" + FirebaseController.UserId + "/tipo/").Push().Key;

                            FirebaseController.WriteDataInt("/usuarios/" + FirebaseController.UserId + "/tipo/", Key, TypeValue);
                        }
                    }
                }
                else
                {
                    FirebaseController.WriteDataBool("/usuarios/" + FirebaseController.UserId, "tipo", false);
                }

                if (AuthCreateName.text != "")
                {
                    FirebaseController.WriteDataString("/usuarios/" + FirebaseController.UserId, "nome", AuthCreateName.text);
                }
                else
                {
                    FirebaseController.WriteDataString("/usuarios/" + FirebaseController.UserId, "nome", FirebaseController.UserName);
                }

                FirebaseController.WriteDataInt("/usuarios/" + FirebaseController.UserId, "dificuldade", Difficulty.value);

                FirebaseController.AuthVerification();
            }
        }

        public static void Alert(string Text)
        {
            PopupManager.Instance.CreatePopup(Text);
            PopupManager.Instance.AddButton("OK", delegate
            {
                PopupManager.Instance.HidePopup();
            });
            PopupManager.Instance.ShowPopup();
        }

        public void AlertUI(string Text)
        {
            Alert(Text);
        }

        public static void EmailPopup()
        {
            PopupManager.Instance.CreatePopup("Por favor, verifique seu Email!");
            PopupManager.Instance.AddButton("OK", delegate
            {
                SystemInterface.SendEmail();
            });
            PopupManager.Instance.AddButton("SAIR", delegate
            {
                PopupManager.Instance.HidePopup();
            });
            PopupManager.Instance.ShowPopup();
        }

        public static void SendEmail()
        {
            Application.OpenURL("mailto:email@soulswizard.com.br");
        }

        public IEnumerator OpenCharacter()
        {
            LoadUI.SetActive(true);
            AuthLoginUI.SetActive(false);
            AuthCreateUI.SetActive(false);
            AuthCharacterUI.SetActive(false);

            yield return new WaitForSecondsRealtime(5f);

            if (!FirebaseController.CreatedIn)
            {
                LoadUI.SetActive(false);
                AuthCharacterUI.SetActive(true);
            }
        }
    }
}
