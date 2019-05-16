using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

namespace TecWolf.Quest
{
    public class QuestItem : MonoBehaviour
    {
        public Text QuestName, QuestValue, QuestComplete, QuestVerify;
        public Button TextButton, GalleryButton, CameraButton;
        public InputField TextInput;

        public string MissionID, MissionName, MissionType, MissionValue, MissionTotal, MissionComplete, MissionVerify, MissionAchievementOne, MissionAchievementTwo, Difficulty;

        private void Start()
        {
            TextButton.onClick.AddListener(delegate
            {
                SetText();
            });

            GalleryButton.onClick.AddListener(delegate
            {
                QuestPhoto.GetPictureMission(4096, MissionID);
            });

            CameraButton.onClick.AddListener(delegate
            {
                QuestPhoto.TakePictureMission(4096, MissionID, "Nenhum");
            });

            QuestName.text = MissionName;

            if (Convert.ToBoolean(MissionComplete))
            {
                QuestComplete.text = "Concluída";
            }
            else
            {
                QuestComplete.text = "Não Concluída";

                switch (Convert.ToInt32(MissionType))
                {
                    case 0:
                        GalleryButton.gameObject.SetActive(true);
                        CameraButton.gameObject.SetActive(true);
                        break;
                    case 1:
                        TextInput.gameObject.SetActive(true);
                        TextButton.gameObject.SetActive(true);
                        break;
                    case 2:
                        QuestValue.gameObject.SetActive(true);
                        break;
                    case 3:
                        GalleryButton.gameObject.SetActive(true);
                        CameraButton.gameObject.SetActive(true);

                        TextInput.gameObject.SetActive(true);
                        TextButton.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
            }

            if (Convert.ToBoolean(MissionVerify))
            {
                QuestVerify.text = "Verificada";
            }
            else
            {
                QuestVerify.text = "Aguardando";
            }

            if (Convert.ToInt32(MissionType) == 2)
            {
                QuestValue.text = MissionValue + "m" + " de " + MissionTotal + "m";
            }
            else
            {
                QuestValue.text = "";
            }
        }

        public void SetText()
        {
            if (TextInput.text != "")
            {
                string TextName = FirebaseDatabase.DefaultInstance.GetReference("galeria-privada").Push().Key;

                string Path = "/textos-privado/" + TextName;

                FirebaseController.WriteDataString(Path, "usuario", FirebaseController.UserId);
                FirebaseController.WriteDataString(Path, "local", "Nenhum");
                FirebaseController.WriteDataString(Path, "missao", MissionID);

                FirebaseController.WriteDataString(Path, "nivel", TecWolf.Player.PlayerMission.Level.ToString());
                FirebaseController.WriteDataString(Path, "email", FirebaseController.UserEmail);

                FirebaseController.WriteDataString(Path, "texto", TextInput.text);

                FirebaseController.WriteDataBool("/usuarios/" + FirebaseController.UserId + "/missoes/" + MissionID, "concluida", true);

                TecWolf.System.SystemSound.Effect.PlayOneShot(TecWolf.System.SystemSound.SoundsStatic[0]);

                FindObjectOfType<QuestInterface>().Refresh();
            }
        }

        public void OpenMic()
        {
            Player.PlayerInterface.MicInterface.SetActive(true);
        }
    }
}
