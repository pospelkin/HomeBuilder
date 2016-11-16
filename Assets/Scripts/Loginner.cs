using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using HomeBuilder.Core;
using UnityEngine.UI;

public class Loginner : MonoBehaviour
{
    public static AccessToken token = null;
    public static User user         = null;

    public ScreenController screen;

    public InputField login;
    public InputField password;

    private List<User> users;

    public class User
    {
        public readonly string email;
        public readonly string password;

        public User(string e, string p)
        {
            email    = e;
            password = p;
        }

        public bool IsEqual(User u)
        {
            return email == u.email;
        }

        public bool IsEqual(string e)
        {
            return email == e;
        }

        public bool IsAuthorised(string p)
        {
            return password == p;
        }
    }

	public void SignIn()
    {
        if (IsUserCorrect(login.text, password.text))
        {
            user = new User(login.text, password.text);

            OpenHistory();
        }
    }

    private void SiqnInWithFacebook(string id)
    {
        user = new User(id, "1111");

        OpenHistory();
    }

    private void OpenHistory()
    {
        Master.SLIDE = true;
        Master.FLOW = true;

        screen.OpenHistory();
    }

    public void LoginWithFacebook()
    {
        if (FB.IsInitialized)
        {
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, AuthCallback);
        }
    }

    public void ForgotPassword()
    {

    }

    public void CreateAccount()
    {

    }

    void Awake()
    {
        users = new List<User>();
        users.Add(new User("user", "user"));

        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to initialize the Facebook SDK.");
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            token = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            Debug.Log(token.UserId);
            // Print current access token's granted permissions
            foreach (string perm in token.Permissions)
            {
                Debug.Log(perm);
            }

            login.text = token.UserId;
            SiqnInWithFacebook(token.UserId);
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    private bool IsUserCorrect(string email, string password)
    {
        foreach (User u in users)
        {
            if (u.IsEqual(email) && u.IsAuthorised(password)) return true;
        }

        return false;
    }

}
