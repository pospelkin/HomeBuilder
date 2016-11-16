using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using UnityEngine.UI;

namespace HomeBuilder.Share
{

    public class ShareManager : MonoBehaviour
    {

        public InputField email;

        public void ShareOnFacebook()
        {
            if (FB.IsInitialized)
            {
                if (FB.IsLoggedIn)
                {
                    FB.ShareLink(new Uri("http://www.theuselessweb.com/"), "I created a house of my dream!");
                }
                else
                {
                    var perms = new List<string>() { "public_profile", "email", "user_friends" };
                    FB.LogInWithReadPermissions(perms, (ILoginResult result) => { if (FB.IsLoggedIn) ShareOnFacebook(); });
                }
            }
            else
            {
                FB.Init(() => { FB.ActivateApp(); ShareOnFacebook(); });
            }
        }

        public void DownloadPDF()
        {
            
        }

        public void SendPDFViaEmail()
        {
            
        }

        public void Close()
        {
            GetComponent<Animator>().SetBool("Open", false);
        }

        public void Open()
        {
            GetComponent<Animator>().SetBool("Open", true);
        }
        
    }


}
