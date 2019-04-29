using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine;

public class PlayGames : MonoBehaviour {
    
    private void Start()
    {
        // Create client configuration
        PlayGamesClientConfiguration Config = new PlayGamesClientConfiguration.Builder().Build();

        // Enable debugging output (recommended)
        PlayGamesPlatform.DebugLogEnabled = true;

        // Initialize and activate the platform
        PlayGamesPlatform.InitializeInstance(Config);
        PlayGamesPlatform.Activate();

        /*
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.Authenticate(SignInCallback, false);
        }
        else
        {
            // PlayGamesPlatform.Instance.SignOut();
        }
        */

        Social.localUser.Authenticate((bool Success) => {
            Debug.Log("Social: " + Success);
        });
    }

    public void SignInCallback(bool Success)
    {
        if (Success)
        {
            Debug.Log("Play Games: " + Social.localUser.userName);
        }
        else
        {
            Debug.Log("Play Games: " + false);
        }
    }

    public void ShowAchievements()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ShowAchievementsUI();
        }
        else
        {
            Debug.Log("Play Games Achievements: " + false);
        }
    }

    public static void ReportProgress(string ID)
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.ReportProgress(ID, 100.0f, (bool Success) => {
                Debug.Log("Play Games Unlock: " + Success);
            });
        }
    }

    public static void IncrementAchievement(string ID)
    {
        if (Social.localUser.authenticated)
        {
            PlayGamesPlatform.Instance.IncrementAchievement(ID, 1, (bool Success) =>
            {
                Debug.Log("Play Games Increment: " + Success);
            });
        }
    }
}
