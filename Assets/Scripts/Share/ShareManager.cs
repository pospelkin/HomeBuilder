using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using HomeBuilder.Core;
using UnityEngine.UI;
using VoxelBusters.NativePlugins;

namespace HomeBuilder.Share
{

    public class ShareManager : MonoBehaviour
    {

        public InputField email;
        public PDFComposer pdf;

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
            pdf.DownloadPDF(Master.GetInstance().GetCurrent());

            Close();
        }

        public void SendPDFViaEmail()
        {
            Appartment app = Master.GetInstance().GetCurrent();

            MailShareComposer _composer = new MailShareComposer();
            _composer.Subject = app.GetName();
            _composer.Body = "Here is you " + app.GetName() + ".\n\n" + "Yours sincerely,\n" + "HomeBuilder";

            // Set below to true if the body is HTML
            _composer.IsHTMLBody = false;

            // Send array of receivers if required
            _composer.ToRecipients = new string[] { email.text } ;
            //_composer.CCRecipients = m_mailCCRecipients;
            //_composer.BCCRecipients = m_mailBCCRecipients;

            // Add below line if you want to attach screenshot. Else, ignore.
            // _composer.AttachScreenShot();

            // Add below line if you want to attach a file, for ex : image. Else, ignore.
            _composer.AddAttachmentAtPath(pdf.DownloadPDF(app), MIMEType.kPDF);

            // Use below line if you want to add any other attachment format. Else, ignore.
            // _composer.AddAttachment(ATTACHMENT_DATA_IN_BYTE_ARRAY, ATTACHMENT_FILE_NAME, MIME_TYPE);

            // Show share view
            NPBinding.Sharing.ShowView(_composer, FinishedSharing);
        }

        void FinishedSharing(eShareResult res)
        {
            Debug.Log("Done email. " + res);

            Close();
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
