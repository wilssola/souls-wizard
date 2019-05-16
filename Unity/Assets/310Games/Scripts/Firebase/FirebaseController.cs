using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Firebase;
using Firebase.Auth;
using Firebase.Storage;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections;
using UnityEngine.SceneManagement;

public class FirebaseController : MonoBehaviour
{
    public static DatabaseReference DatabaseReference;

    public static FirebaseStorage Storage;
    public static StorageReference StorageReference;
    public static string DownloadUrl;

    public static FirebaseAuth Auth;
    public static FirebaseUser User;
    public static Uri UserPhoto;
    public static bool SignedIn, CreatedIn;
    public static string UserId, UserEmail, UserName;

    public AchievementManager Manager;

    public static AchievementManager ManagerStatic;

    public static bool WritedInt;

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(Task =>
        {
            var DependencyStatus = Task.Result;

            if (DependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Não foi possível resolver todas as dependências do Firebase: " + DependencyStatus);
                Application.Quit();
            }
        });

        ManagerStatic = Manager;
    }

    public void InitializeFirebase()
    {
#if UNITY_EDITOR
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://soulswizard.firebaseio.com/");
#endif
        DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        Auth = FirebaseAuth.DefaultInstance;
        Auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);

        Storage = FirebaseStorage.DefaultInstance;
        StorageReference = Storage.GetReferenceFromUrl("gs://soulswizard.appspot.com");

        // Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        // Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
    }

    public static void WriteDataInt(string Directory, string Child, int Value)
    {
        FirebaseDatabase.DefaultInstance.GetReference(Directory).Child(Child).SetValueAsync(Value).ContinueWith(Task =>
        {
            if (Task.IsCompleted)
            {
                WritedInt = true;

                Debug.Log("Escrita 5: " + WritedInt);
            }
            else
            {
                WritedInt = false;

                Debug.Log("Escrita 6: " + WritedInt);
            }
        });
    }
    public static void WriteDataFloat(string Directory, string Child, float Value)
    {
        FirebaseDatabase.DefaultInstance.GetReference(Directory).Child(Child).SetValueAsync(Value);
    }
    public static void WriteDataString(string Directory, string Child, string Value)
    {
        FirebaseDatabase.DefaultInstance.GetReference(Directory).Child(Child).SetValueAsync(Value);
    }
    public static void WriteDataBool(string Directory, string Child, bool Value)
    {
        FirebaseDatabase.DefaultInstance.GetReference(Directory).Child(Child).SetValueAsync(Value);
    }

    public void Update()
    {
        /*
        FirebaseDatabase.DefaultInstance.GetReference("conectado").GetValueAsync().ContinueWith(Task =>
        {
            DataSnapshot Snapshot = Task.Result;

            if (Convert.ToBoolean(Snapshot.Value) != true)
            {
                Debug.Log("Desconectado do Firebase Realtime");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.Log("Conectado ao Firebase Realtime");
            }
        });
        */
    }

    public static void CheckUserCharacter(bool FirstTime)
    {
        FirebaseDatabase.DefaultInstance.GetReference("/usuarios/" + UserId + "/personagem/").GetValueAsync().ContinueWith(Task =>
        {
            if (Task.IsFaulted)
            {

            }
            else if (Task.IsCompleted)
            {
                DataSnapshot Snapshot = Task.Result;

                // if (FirstTime)
                // {
                Debug.Log("Personagem Criado: " + Snapshot.Child("criado").Value.ToString());

                CreatedIn = Convert.ToBoolean(Snapshot.Child("criado").Value);
                // }

                if (CreatedIn)
                {
                    TecWolf.Player.PlayerMission.Level = Convert.ToInt32(Snapshot.Child("nivel").Value);
                    TecWolf.Player.PlayerCharacter.PlayerSkin = Convert.ToInt32(Snapshot.Child("cor").Value);
                    TecWolf.Player.PlayerCharacter.PlayerGender = Convert.ToInt32(Snapshot.Child("sexo").Value);
                }

                // Debug.Log("Nível: " + TecWolf.Player.PlayerMission.Level);
                // Debug.Log("Sexo: " + PlayerCharacter.PlayerGender);
                // Debug.Log("Cor: " + PlayerCharacter.PlayerSkin);
            }
        });

        FirebaseDatabase.DefaultInstance.GetReference("/usuarios/" + UserId).GetValueAsync().ContinueWith(Task =>
        {
            if (Task.IsFaulted)
            {

            }
            else if (Task.IsCompleted)
            {
                DataSnapshot Snapshot = Task.Result;

                bool LevelEqual = false;

                /* 
                if (Convert.ToBoolean(Snapshot.Child("tipo").Value) == false)
                {
                    LevelEqual = true;
                }
                */

                foreach (var ChildSnapshot in Snapshot.Child("tipo").Children)
                {
                    if (Convert.ToInt32(ChildSnapshot.Value) == TecWolf.Player.PlayerMission.Level)
                    {
                        LevelEqual = true;
                    }
                }

                if (!LevelEqual && TecWolf.Player.PlayerMission.Level < 9)
                {
                    if (CreatedIn)
                    {
                        WriteDataInt("/usuarios/" + UserId + "/personagem/", "nivel", TecWolf.Player.PlayerMission.Level + 1);
                    }
                }

                LevelEqual = false;

                TecWolf.Player.PlayerMission.Difficulty = Convert.ToInt32(Snapshot.Child("dificuldade").Value);
            }
        });
    }

    public static void GetChannelsDatabase(List<Channel> List)
    {
        FirebaseDatabase.DefaultInstance.GetReference("canais").GetValueAsync().ContinueWith(Task =>
        {
            if (Task.IsFaulted || !Task.IsCompleted || Task.IsCanceled)
            {
                return;
            }

            DataSnapshot Snapshot = Task.Result;

            if (Snapshot != null && Snapshot.ChildrenCount > 0)
            {
                foreach (var ChildSnapshot in Snapshot.Children)
                {
                    if (List.Any(channelNew => channelNew.ChanneId == ChildSnapshot.Key)) continue;
                    var NewChannel = new Channel();

                    NewChannel.ChanneId = ChildSnapshot.Key;
                    NewChannel.Creator = ChildSnapshot.Child("criador").Value.ToString();
                    NewChannel.Date = ChildSnapshot.Child("timestamp").Value.ToString();
                    NewChannel.Title = ChildSnapshot.Child("titulo").Value.ToString();

                    NewChannel.Messages = new List<Messages>();
                    List.Add(NewChannel);
                }
                TecWolf.Chat.ChatInterface.SpawnChannelsButtons();
            }
        });
    }

    public static void CreaterChannelDataBase(string Title, string Creator)
    {
        var ChannelID = FirebaseDatabase.DefaultInstance.GetReference("canais").Push().Key;

        Dictionary<string, object> ChannelUpdate = new Dictionary<string, object>();
        Dictionary<string, object> Timestamp = new Dictionary<string, object>();

        Timestamp[".sv"] = "timestamp";

        ChannelUpdate["canais/" + ChannelID + "/criador"] = Creator;
        ChannelUpdate["canais/" + ChannelID + "/titulo"] = Title;
        ChannelUpdate["canais/" + ChannelID + "/timestamp/"] = Timestamp;

        FirebaseDatabase.DefaultInstance.RootReference.UpdateChildrenAsync(ChannelUpdate);
    }

    public static void CreateMessage(string ChannelID, string Creator, string Text)
    {
        var MessageID = FirebaseDatabase.DefaultInstance.GetReference("canais").Child(ChannelID).Child("mensagens").Push().Key;

        Dictionary<string, object> MessageUpdate = new Dictionary<string, object>();
        Dictionary<string, object> Timestamp = new Dictionary<string, object>();

        Timestamp[".sv"] = "timestamp";

        MessageUpdate["mensagens/" + MessageID + "/criador"] = Creator;
        MessageUpdate["mensagens/" + MessageID + "/mensagem"] = Text;
        MessageUpdate["mensagens/" + MessageID + "/timestamp/"] = Timestamp;

        FirebaseDatabase.DefaultInstance.RootReference.Child("canais").Child(ChannelID).UpdateChildrenAsync(MessageUpdate);
    }

    public static void ObservingChatMessages(Channel CurrentChannel)
    {
        FirebaseDatabase.DefaultInstance.GetReference("canais").Child(CurrentChannel.ChanneId).Child("mensagens").ValueChanged += (object Sender, ValueChangedEventArgs EventArgs) =>
        {
            if (EventArgs.DatabaseError != null)
            {
                Debug.LogError(EventArgs.DatabaseError.Message);
                return;
            }
            if (EventArgs.Snapshot != null && EventArgs.Snapshot.ChildrenCount > 0)
            {
                foreach (var ChildSnapshot in EventArgs.Snapshot.Children)
                {
                    if (CurrentChannel.Messages.Any(contentMessage => contentMessage.MessageID == ChildSnapshot.Key)) continue;
                    var NewMessage = new Messages();
                    NewMessage.MessageID = ChildSnapshot.Key;
                    NewMessage.SenderID = ChildSnapshot.Child("criador").Value.ToString();
                    NewMessage.Text = ChildSnapshot.Child("mensagem").Value.ToString();
                    CurrentChannel.Messages.Add(NewMessage);
                }
                TecWolf.Chat.ChatInterface.SpawnChatMessages();
            }
        };
    }

    public void ObservingChatWriting()
    {

    }

    public static void ListGallery(List<ImageItem> List, string Type, string Value)
    {
        FirebaseDatabase.DefaultInstance.GetReference("galeria-publica").GetValueAsync().ContinueWith((Task<DataSnapshot> Task) =>
        {
            if (Task.IsFaulted)
            {

            }
            else if (Task.IsCompleted)
            {
                DataSnapshot Snapshot = Task.Result;

                foreach (var ChildSnapshot in Snapshot.Children)
                {
                    if (ChildSnapshot.Child(Type).Value.ToString() == Value)
                    {
                        var NewGallery = new ImageItem();

                        NewGallery.ImageID = ChildSnapshot.Key;

                        NewGallery.Link = ChildSnapshot.Child("link").Value.ToString();
                        NewGallery.Local = ChildSnapshot.Child("local").Value.ToString();
                        NewGallery.Mission = ChildSnapshot.Child("missao").Value.ToString();
                        NewGallery.User = ChildSnapshot.Child("usuario").Value.ToString();
                        NewGallery.Format = ChildSnapshot.Child("formato").Value.ToString();
                        NewGallery.Level = ChildSnapshot.Child("nivel").Value.ToString();

                        List.Add(NewGallery);
                        Debug.Log("Imagem obtida com sucesso.");
                    }
                }
            }
        });
    }

    public static void GetMission(List<Mission> List, int MonsterID)
    {
        FirebaseDatabase.DefaultInstance.GetReference("missoes").GetValueAsync().ContinueWith(Task =>
        {
            if (Task.IsFaulted)
            {

            }
            else if (Task.IsCompleted)
            {
                DataSnapshot Snapshot = Task.Result;

                foreach (var ChildSnapshot in Snapshot.Children)
                {
                    if (Convert.ToInt32(ChildSnapshot.Child("nivel").Value) == MonsterID && Convert.ToInt32(ChildSnapshot.Child("dificuldade").Value) == TecWolf.Player.PlayerMission.Difficulty)
                    {
                        var NewMission = new Mission();

                        NewMission.MissionID = FirebaseDatabase.DefaultInstance.GetReference("missoes").Push().Key;

                        NewMission.Name = ChildSnapshot.Child("objetivo").Value.ToString();
                        NewMission.Type = ChildSnapshot.Child("tipo").Value.ToString();
                        NewMission.Value = ChildSnapshot.Child("valor").Value.ToString();
                        NewMission.Monster = ChildSnapshot.Child("nivel").Value.ToString();


                        Debug.Log("Missão 1");

                        NewMission.AchievementOne = ChildSnapshot.Child("conquista").Value.ToString();
                        NewMission.AchievementTwo = ChildSnapshot.Child("conquista_final").Value.ToString();

                        NewMission.Difficulty = ChildSnapshot.Child("dificuldade").Value.ToString();

                        Debug.Log("Missão 2");

                        List.Add(NewMission);
                        Debug.Log("Missão obtida com sucesso.");
                    }
                }
            }
        });
    }

    public static void CheckMission(int MonsterID)
    {
        int VerifiedMissions = 0;
        int TotalMissions = 0;

        int LevelID = 0;

        FirebaseDatabase.DefaultInstance.GetReference("/usuarios/" + UserId + "/missoes/").GetValueAsync().ContinueWith(Task =>
        {
            if (Task.IsFaulted)
            {

            }
            else if (Task.IsCompleted)
            {
                DataSnapshot Snapshot = Task.Result;

                foreach (var ChildSnapshot in Snapshot.Children)
                {
                    if (Convert.ToInt32(ChildSnapshot.Child("nivel").Value) == MonsterID)
                    {
                        if (Convert.ToBoolean(ChildSnapshot.Child("concluida").Value) == true)
                        {
                            // PlayGames.ReportProgress(ChildSnapshot.Child("conquista").ToString());

                            if (ChildSnapshot.Child("conquista").Value.ToString() != "" || ChildSnapshot.Child("conquista").Value.ToString() != null)
                            {
                                ManagerStatic.UnlockAchievement((AchievementID)System.Enum.Parse(typeof(AchievementID), ChildSnapshot.Child("conquista").Value.ToString()));
                            }

                            TecWolf.Player.PlayerMission.FinalAchievement = ChildSnapshot.Child("conquista_final").Value.ToString();
                        }

                        if (Convert.ToBoolean(ChildSnapshot.Child("verificada").Value) == true)
                        {
                            VerifiedMissions = VerifiedMissions + 1;
                        }
                        else
                        {
                            if (Convert.ToInt32(ChildSnapshot.Child("tipo").Value) == 2)
                            {
                                if (Convert.ToInt32(ChildSnapshot.Child("total").Value) >= Convert.ToInt32(ChildSnapshot.Child("valor").Value))
                                {
                                    WriteDataBool("/usuarios/" + UserId + "/missoes/" + ChildSnapshot.Key, "verificada", true);
                                    WriteDataBool("/usuarios/" + UserId + "/missoes/" + ChildSnapshot.Key, "concluida", true);
                                }
                                else
                                {
                                    if (TecWolf.Player.PlayerController.Distance <= 100)
                                    {
                                        WriteDataInt("/usuarios/" + UserId + "/missoes/" + ChildSnapshot.Key, "total", Convert.ToInt32(ChildSnapshot.Child("total").Value) + Convert.ToInt32(TecWolf.Player.PlayerController.Distance));
                                    }
                                }
                            }
                        }

                        TotalMissions = TotalMissions + 1;

                        // Debug.Log("Nível Missões: " + LevelID);
                        // Debug.Log("Nível Monstro: " + MonsterID);
                    }
                }

                // Debug.Log("Total: " + (TotalMissions).ToString());
                // Debug.Log("Verificadas: " + (VerifiedMissions).ToString());

                // Debug.Log(TecWolf.Player.PlayerMission.LevelChange);

                if ((VerifiedMissions == TotalMissions) && (TotalMissions > 0 && VerifiedMissions > 0) && (TecWolf.Player.PlayerMission.Level == MonsterID))
                {
                    Debug.Log("Escrita 1: " + WritedInt);

                    TecWolf.Monster.MonsterFinalInterface.StaticMonsterUI.SetActive(true);
                    TecWolf.Monster.MonsterFinalInterface.Show();

                    TecWolf.System.SystemSound.Effect.PlayOneShot(TecWolf.System.SystemSound.MonsterSoundsStatic[0]);

                    // PlayGames.ReportProgress(TecWolf.Player.PlayerMission.FinalAchievement);

                    TecWolf.Player.PlayerMission.Level = TecWolf.Player.PlayerMission.Level + 1;
                    TecWolf.System.SystemInterface.Alert("Você passou para o Nível " + (TecWolf.Player.PlayerMission.Level + 1).ToString());

                    if (!WritedInt)
                    {
                        WriteDataInt("/usuarios/" + UserId + "/personagem/", "nivel", TecWolf.Player.PlayerMission.Level);
                    }

                    Debug.Log("Escrita 2: " + WritedInt);

                    TecWolf.Player.PlayerMission.LevelChange = false;

                    if (TecWolf.Player.PlayerMission.FinalAchievement != "" || TecWolf.Player.PlayerMission.FinalAchievement != null)
                    {
                        ManagerStatic.UnlockAchievement((AchievementID)System.Enum.Parse(typeof(AchievementID), TecWolf.Player.PlayerMission.FinalAchievement));
                    }

                    Debug.Log("Escrita 3: " + WritedInt);
                }

                if (TotalMissions > 0)
                {
                    TecWolf.Player.PlayerMission.InMission = true;
                }
                else
                {
                    TecWolf.Player.PlayerMission.InMission = false;
                }

                TotalMissions = 0;
                VerifiedMissions = 0;
            }

            if (WritedInt)
            {
                WritedInt = false;
                Debug.Log("Escrita 4: " + WritedInt);
            }
        });
    }

    public static void ListMission(List<MissionItem> List, int MonsterID)
    {
        FirebaseDatabase.DefaultInstance.GetReference("/usuarios/" + UserId + "/missoes/").GetValueAsync().ContinueWith(Task =>
        {
            if (Task.IsFaulted)
            {

            }
            else if (Task.IsCompleted)
            {
                DataSnapshot Snapshot = Task.Result;

                foreach (var ChildSnapshot in Snapshot.Children)
                {
                    if (Convert.ToInt32(ChildSnapshot.Child("nivel").Value) == MonsterID)
                    {
                        var NewMission = new MissionItem();

                        NewMission.MissionID = ChildSnapshot.Key;

                        NewMission.Name = ChildSnapshot.Child("objetivo").Value.ToString();
                        NewMission.Type = ChildSnapshot.Child("tipo").Value.ToString();
                        NewMission.Value = ChildSnapshot.Child("valor").Value.ToString();
                        NewMission.Total = ChildSnapshot.Child("total").Value.ToString();
                        NewMission.Verify = ChildSnapshot.Child("verificada").Value.ToString();
                        NewMission.Complete = ChildSnapshot.Child("concluida").Value.ToString();

                        NewMission.AchievementOne = ChildSnapshot.Child("conquista").Value.ToString();
                        NewMission.AchievementTwo = ChildSnapshot.Child("conquista_final").Value.ToString();

                        NewMission.Difficulty = ChildSnapshot.Child("dificuldade").Value.ToString();

                        List.Add(NewMission);
                        Debug.Log("Missão adicionada a Lista de Missões.");

                        ManagerStatic.UnlockAchievement((AchievementID)System.Enum.Parse(typeof(AchievementID), ChildSnapshot.Child("conquista").Value.ToString()));
                    }
                }
            }
        });
    }

    public static void GetMonsters(List<Monster> List)
    {
        FirebaseDatabase.DefaultInstance.GetReference("monstros").GetValueAsync().ContinueWith(Task =>
        {
            if (Task.IsFaulted)
            {

            }
            else if (Task.IsCompleted)
            {
                DataSnapshot Snapshot = Task.Result;

                foreach (var ChildSnapshot in Snapshot.Children)
                {
                    var NewMonster = new Monster();

                    NewMonster.MonsterID = ChildSnapshot.Key;

                    NewMonster.MonsterName = ChildSnapshot.Child("nome").Value.ToString();
                    NewMonster.MonsterLevel = ChildSnapshot.Child("nivel").Value.ToString();

                    List.Add(NewMonster);
                }
            }
        });
    }

    public void AuthStateChanged(object Sender, EventArgs EventArgs)
    {
        if (Auth.CurrentUser != User)
        {
            SignedIn = User != Auth.CurrentUser && Auth.CurrentUser != null;

            if (!SignedIn && User != null)
            {
                Debug.Log("Firebase Auth - " + User.UserId + " deslogado.");

                TecWolf.System.SystemInterface.Alert("Deslogado com sucesso.");

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            User = Auth.CurrentUser;

            if (SignedIn)
            {
                Debug.Log("Firebase Auth - " + User.UserId + " logado.");

                TecWolf.System.SystemInterface.Alert("Logado com sucesso.");

                UserId = User.UserId ?? "";
                UserName = User.DisplayName ?? "";
                UserEmail = User.Email ?? "";

                // UserPhoto = User.PhotoUrl ?? null;

                CheckUserCharacter(true);
            }
        }
    }

    public static void CreateAuth(string Email, string Password, string Name)
    {
        Auth.CreateUserWithEmailAndPasswordAsync(Email, Password).ContinueWith(Task =>
        {
            if (Task.IsCanceled)
            {
                Debug.LogError("Firebase Auth - CreateUserWithEmailAndPasswordAsync foi cancelado.");
                TecWolf.System.SystemInterface.Alert("A criação da conta foi cancelada.");
                return;
            }

            if (Task.IsFaulted)
            {
                Debug.LogError("Firebase Auth - CreateUserWithEmailAndPasswordAsync encontrou um erro: " + Task.Exception);
                TecWolf.System.SystemInterface.Alert("Erro ao criar conta, verifique Email e Senha.");
                return;
            }

            User = Task.Result;
            AuthName(Name);

            Debug.LogFormat("Firebase Auth - Usuário criado com sucesso: {0} ({1})", User.DisplayName, User.UserId);
            TecWolf.System.SystemInterface.Alert("Conta criada com sucesso.");
        });
    }

    public static void LoginAuth(string Email, string Password)
    {
        Auth.SignInWithEmailAndPasswordAsync(Email, Password).ContinueWith(task =>
        {

            if (task.IsCanceled)
            {
                Debug.LogError("Firebase Auth - SignInWithEmailAndPasswordAsync foi cancelado.");
                TecWolf.System.SystemInterface.Alert("O login da conta foi cancelado.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Auth - SignInWithEmailAndPasswordAsync encontrou um erro: " + task.Exception);
                TecWolf.System.SystemInterface.Alert("Erro ao logar conta, verifique Email e Senha.");
                return;
            }

            User = task.Result;

            Debug.LogFormat("Firebase Auth - Usuário logado com sucesso: {0} ({1})", User.DisplayName, User.UserId);
            TecWolf.System.SystemInterface.Alert("Conta logada com sucesso.");
        });
    }

    public static void GoogleAuth(string GoogleTokenID, string GoogleAccessToken)
    {
        Credential Credential = GoogleAuthProvider.GetCredential(GoogleTokenID, GoogleAccessToken);

        Auth.SignInWithCredentialAsync(Credential).ContinueWith(Task =>
        {
            if (Task.IsCanceled)
            {
                Debug.LogError("Firebase Auth - SignInWithCredentialAsync foi cancelado.");
                TecWolf.System.SystemInterface.Alert("O login da conta foi cancelado.");
                return;
            }
            if (Task.IsFaulted)
            {
                Debug.LogError("Firebase Auth - SignInWithCredentialAsync encontrou um erro: " + Task.Exception);
                TecWolf.System.SystemInterface.Alert("Erro ao logar conta, verifique Email e Senha.");
                return;
            }

            User = Task.Result;

            Debug.LogFormat("Firebase Auth - Usuário logado com sucesso: {0} ({1})", User.DisplayName, User.UserId);
            TecWolf.System.SystemInterface.Alert("Conta logada com sucesso.");
        });
    }

    public static void FacebookAuth(string AccessToken)
    {
        Credential Credential = FacebookAuthProvider.GetCredential(AccessToken);

        Auth.SignInWithCredentialAsync(Credential).ContinueWith(Task =>
        {
            if (Task.IsCanceled)
            {
                Debug.LogError("Firebase Auth - SignInWithCredentialAsync foi cancelado.");
                TecWolf.System.SystemInterface.Alert("O login da conta foi cancelado.");
                return;
            }
            if (Task.IsFaulted)
            {
                Debug.LogError("Firebase Auth - SignInWithCredentialAsync encontrou um erro: " + Task.Exception);
                TecWolf.System.SystemInterface.Alert("Erro ao logar conta, verifique Email e Senha.");
                return;
            }

            User = Task.Result;

            Debug.LogFormat("Firebase Auth - Usuário logado com sucesso: {0} ({1})", User.DisplayName, User.UserId);
            TecWolf.System.SystemInterface.Alert("Conta logada com sucesso.");
        });
    }

    public static void AnonymouslyAuth()
    {
        Auth.SignInAnonymouslyAsync().ContinueWith(Task =>
        {
            if (Task.IsCanceled)
            {
                Debug.LogError("Firebase Auth - SignInAnonymouslyAsync foi cancelado.");
                TecWolf.System.SystemInterface.Alert("O login da conta foi cancelado.");
                return;
            }
            if (Task.IsFaulted)
            {
                Debug.LogError("Firebase Auth - SignInAnonymouslyAsync encontrou um erro: " + Task.Exception);
                TecWolf.System.SystemInterface.Alert("Erro ao logar conta, verifique Email e Senha.");
                return;
            }

            User = Task.Result;

            Debug.LogFormat("Firebase Auth - Usuário logado com sucesso: {0} ({1})", User.DisplayName, User.UserId);
            TecWolf.System.SystemInterface.Alert("Conta logada com sucesso.");
        });
    }

    public static void AuthName(string Name)
    {
        FirebaseUser User = Auth.CurrentUser;

        if (User != null)
        {
            UserProfile Profile = new UserProfile
            {
                DisplayName = Name,
                // PhotoUrl = new System.Uri(""),
            };
            User.UpdateUserProfileAsync(Profile).ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("Firebase Auth - UpdateUserProfileAsync foi cancelado.");
                    // TecWolf.System.SystemInterface.Alert("A atualização de conta foi cancelada.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("Firebase Auth - UpdateUserProfileAsync encontrou um erro: " + task.Exception);
                    // TecWolf.System.SystemInterface.Alert("Erro ao atualizar conta.");
                    return;
                }

                Debug.Log("Firebase Auth - Usuário atualizado com sucesso.");
                // TecWolf.System.SystemInterface.Alert("Conta atualizada com sucesso.");
            });
        }
    }

    public static void AuthVerification()
    {
        if (User != null)
        {
            User.SendEmailVerificationAsync().ContinueWith(Task =>
            {
                if (Task.IsCanceled)
                {
                    Debug.LogError("Firebase Auth - SendEmailVerificationAsync foi cancelada.");
                    TecWolf.System.SystemInterface.Alert("A verificação de conta por Email foi cancelada.");
                    return;
                }
                if (Task.IsFaulted)
                {
                    Debug.LogError("Firebase Auth - SendEmailVerificationAsync encontrou um erro: " + Task.Exception);
                    TecWolf.System.SystemInterface.Alert("Erro ao enviar verificação de conta por Email.");
                    return;
                }

                Debug.Log("Firebase Auth - Email enviado com sucesso.");
                TecWolf.System.SystemInterface.Alert("Verificação de conta por Email enviada com sucesso.");
            });
        }
    }

    public static void AuthQuit()
    {
        Auth.SignOut();

        Debug.Log("Firebase Auth - Usuário deslogado com sucesso.");
        TecWolf.System.SystemInterface.Alert("Conta deslogada com sucesso.");
    }

    /*
    public void OnTokenReceived(object Sender, Firebase.Messaging.TokenReceivedEventArgs EventArgs)
    {
        Debug.Log("Firebase Messaging - Token de Registro Recebido: " + EventArgs.Token);
    }

    public void OnMessageReceived(object Sender, Firebase.Messaging.MessageReceivedEventArgs EventArgs)
    {
        Debug.Log("Firebase Messaging - Recebida uma nova mensagem de: " + EventArgs.Message.From);
    }
    */

    public static void UploadByte(byte[] File, string Path, string Type, string Database)
    {
        StorageReference FileReference = StorageReference.Child(Path);

        var Metadata = new MetadataChange();
        Metadata.ContentType = "image/" + Type.Replace(".", "");

        FileReference.PutBytesAsync(File, Metadata).ContinueWith(Task =>
        {
            if (Task.IsFaulted || Task.IsCanceled)
            {
                Debug.Log(Task.Exception.ToString());
                TecWolf.System.SystemInterface.Alert("Arquivo não enviada com sucesso, tente novamente.");
            }
            else
            {
                TecWolf.System.SystemInterface.Alert("Arquivo enviado com sucesso.");

                FileReference.GetDownloadUrlAsync().ContinueWith(TaskDownload =>
                {
                    if (TaskDownload.IsCompleted)
                    {
                        Debug.Log("Download URL: " + TaskDownload.Result);
                        DownloadUrl = TaskDownload.Result.ToString();

                        FirebaseController.WriteDataString(Database, "link", FirebaseController.DownloadUrl);
                    }
                });
            }
        });
    }

    public static void UploadFile(string File, string Path, string Type, string Database)
    {
        StorageReference FileReference = StorageReference.Child(Path);

        var Metadata = new MetadataChange();
        Metadata.ContentType = "image/" + Type.Replace(".", "");

        FileReference.PutFileAsync(File, Metadata).ContinueWith((Task) =>
        {
            if (Task.IsFaulted || Task.IsCanceled)
            {
                Debug.Log(Task.Exception.ToString());
                TecWolf.System.SystemInterface.Alert("Foto não enviada com sucesso, tente novamente.");
            }
            else
            {
                TecWolf.System.SystemInterface.Alert("Foto enviada com sucesso.");

                FileReference.GetDownloadUrlAsync().ContinueWith(TaskDownload =>
                {
                    if (TaskDownload.IsCompleted)
                    {
                        Debug.Log("Download URL: " + TaskDownload.Result);
                        DownloadUrl = TaskDownload.Result.ToString();

                        FirebaseController.WriteDataString(Database, "link", FirebaseController.DownloadUrl);
                    }
                });
            }
        });
    }
}

[Serializable]
public struct Channel
{
    public string ChanneId;
    public string Creator;
    public string Title;
    public string Date;
    public List<Messages> Messages;
}
[Serializable]
public struct Messages
{
    public string MessageID;
    public string SenderID;
    public string Text;
}

[Serializable]
public struct Mission
{
    public string MissionID;
    public string Name;
    public string Type;
    public string Value;
    public string Monster;

    public string AchievementOne;
    public string AchievementTwo;

    public string Difficulty;
}
[Serializable]
public struct MissionItem
{
    public string MissionID;
    public string Name;
    public string Type;
    public string Value;
    public string Total;
    public string Complete;
    public string Verify;

    public string AchievementOne;
    public string AchievementTwo;

    public string Difficulty;
}

[Serializable]
public struct ImageItem
{
    public string ImageID;
    public string Link;
    public string Local;
    public string User;
    public string Mission;
    public string Format;
    public string Level;
}

[Serializable]
public struct Monster
{
    public string MonsterID;
    public string MonsterName;
    public string MonsterLevel;
}
[Serializable]
public struct MonsterItem
{
    public string MonsterName;
    public string MonsterLevel;
    public Sprite MonsterVisible;
    public Sprite MonsterInvisible;
}