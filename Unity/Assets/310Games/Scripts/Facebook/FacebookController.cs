using System.Collections.Generic;
using UnityEngine;

using Facebook.Unity;

public class FacebookController : MonoBehaviour
{

    private void Start()
    {
        if (!FB.IsInitialized)
        {
            FB.Init();
        }
        else
        {
            FB.ActivateApp();
        }
    }

    public void FacebookLogin()
    {
        List<string> Permission = new List<string>();
        Permission.Add("public_profile");
        Permission.Add("email");

        FB.LogInWithReadPermissions(Permission, OnFacebookLogin);
    }

    private void OnFacebookLogin(ILoginResult Result)
    {
        if (FB.IsLoggedIn)
        {
            AccessToken Token = AccessToken.CurrentAccessToken;
            FirebaseController.FacebookAuth(Token.TokenString);
        }
        else
        {
            Debug.Log("Facebook - Falha ao logar usuário.");
        }
    }

}

