
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics; // To get the HWND of the Main Window of the current process
using System.Runtime.InteropServices;
using System.Security; // To convert from/to managed and un-managed data

namespace WebClientUtils
{
    class WebCredentialServices
    {
        static String AuthenticatedUser = "Unknown";

        WebCredentialServices () {}

        static public void SetAuthenticatedUser (String userName)
        {
        if ( (userName != null) && (userName.Length > 0) )
            AuthenticatedUser = userName;
        else
            AuthenticatedUser = "Ünknown";
        }

        static public String GetAuthenticatedUser()
        {
            return AuthenticatedUser;
        }

        static public NetworkCredential GetUserCredentials(String inHostName)
        {
            NetworkCredential nwCred = null;
            
            // Create the Message and the Caption texts
            String CaptionText = String.Format("Connect to (0)", inHostName);
            String MessageText = String. Format("Connecting to (0}", inHostName);
            string user=null;
            string password = null;
            string domain = null;
            
            bool bRes = Credentials.Authentication.PromptForPassword(inHostName, CaptionText, MessageText, out user, out password, out domain);

            // Check the return value of the log-on dialog
            if ( bRes )
            {
                if ( (user.Length > 0) && (password.Length > 0) )
                {
                    if ( domain.Length > 0 )
                        nwCred = new NetworkCredential(user, password, domain);
                    else
                        nwCred = new NetworkCredential(user, password);

                    SetAuthenticatedUser (user);
                }
            } 
            return nwCred;
        }
    }
}



