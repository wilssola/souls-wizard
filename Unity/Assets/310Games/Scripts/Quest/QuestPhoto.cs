using UnityEngine;
using Firebase.Database;

namespace TecWolf.Quest
{
    public class QuestPhoto : MonoBehaviour
    {
        public static void TakePictureMission(int MaxSize, string MissionID, string Local)
        {
            NativeCamera.Permission Permission = NativeCamera.TakePicture((Path) =>
            {
                Debug.Log("Path: " + Path);

                if (Path != null)
                {
                // string Format = System.IO.Path.GetExtension(Path);

                Texture2D Texture = DuplicateTexture(NativeCamera.LoadImageAtPath(Path, 1024));

                    if (Texture != null)
                    {
                        string PhotoName = FirebaseDatabase.DefaultInstance.GetReference("galeria-privada").Push().Key;
                        string CloudPath = "galeria/" + PhotoName + ".png";

                        string DatabaseReference = "/galeria-privada/" + PhotoName;

                        FirebaseController.WriteDataString(DatabaseReference, "usuario", FirebaseController.UserId);
                        FirebaseController.WriteDataString(DatabaseReference, "email", FirebaseController.UserEmail);
                        FirebaseController.WriteDataString(DatabaseReference, "nivel", TecWolf.Player.PlayerMission.Level.ToString());
                        FirebaseController.WriteDataString(DatabaseReference, "local", Local);
                        FirebaseController.WriteDataString(DatabaseReference, "missao", MissionID);
                        FirebaseController.WriteDataString(DatabaseReference, "formato", ".png");

                        // FirebaseStart.UploadFile(Path, CloudPath, Format, DownloadURL);
                        FirebaseController.UploadByte(Texture.EncodeToPNG(), CloudPath, ".png", DatabaseReference);

                        FirebaseController.WriteDataBool("/usuarios/" + FirebaseController.UserId + "/missoes/" + MissionID, "concluida", true);

                        TecWolf.System.SystemSound.Effect.PlayOneShot(TecWolf.System.SystemSound.SoundsStatic[0]);

                        FindObjectOfType<QuestInterface>().Refresh();
                    }

                    Destroy(Texture);
                }
            }, MaxSize);

#if UNITY_EDITOR
            FirebaseController.UploadFile(Application.dataPath + "/Test.jpg", "galeria/Test.jpg", ".jpg", "/galeria-privada/Test");
#endif

            Debug.Log("Permissão: " + Permission);
        }

        public static void GetPictureMission(int MaxSize, string MissionID)
        {
            NativeGallery.Permission Permission = NativeGallery.GetImageFromGallery((Path) =>
            {
                Debug.Log("Path: " + Path);

                if (Path != null)
                {
                // string Format = System.IO.Path.GetExtension(Path);

                Texture2D Texture = DuplicateTexture(NativeCamera.LoadImageAtPath(Path, 1024));

                    if (Texture != null)
                    {
                        string PhotoName = FirebaseDatabase.DefaultInstance.GetReference("galeria-privada").Push().Key;
                        string CloudPath = "galeria/" + PhotoName + ".png";

                        string DatabaseReference = "/galeria-privada/" + PhotoName;

                        FirebaseController.WriteDataString(DatabaseReference, "usuario", FirebaseController.UserId);
                        FirebaseController.WriteDataString(DatabaseReference, "email", FirebaseController.UserEmail);
                        FirebaseController.WriteDataString(DatabaseReference, "nivel", TecWolf.Player.PlayerMission.Level.ToString());
                        FirebaseController.WriteDataString(DatabaseReference, "local", "Nenhum");
                        FirebaseController.WriteDataString(DatabaseReference, "missao", MissionID);
                        FirebaseController.WriteDataString(DatabaseReference, "formato", ".png");

                        // FirebaseStart.UploadFile(Path, CloudPath, Format, DownloadURL);
                        FirebaseController.UploadByte(Texture.EncodeToPNG(), CloudPath, ".png", DatabaseReference);

                        FirebaseController.WriteDataBool("/usuarios/" + FirebaseController.UserId + "/missoes/" + MissionID, "concluida", true);

                        TecWolf.System.SystemSound.Effect.PlayOneShot(TecWolf.System.SystemSound.SoundsStatic[0]);

                        FindObjectOfType<QuestInterface>().Refresh();
                    }

                    Destroy(Texture);
                }
            }, "Selecione uma Imagem correspondente a Missão", maxSize: MaxSize);

            Debug.Log("Permissão: " + Permission);
        }

        public static Texture2D DuplicateTexture(Texture2D Source)
        {
            RenderTexture Render = RenderTexture.GetTemporary(Source.width, Source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);

            Graphics.Blit(Source, Render);
            RenderTexture Previous = RenderTexture.active;
            RenderTexture.active = Render;

            Texture2D ReadableTexture = new Texture2D(Source.width, Source.height);
            ReadableTexture.ReadPixels(new Rect(0, 0, Render.width, Render.height), 0, 0);
            ReadableTexture.Apply();

            RenderTexture.active = Previous;
            RenderTexture.ReleaseTemporary(Render);

            return ReadableTexture;
        }
    }
}
