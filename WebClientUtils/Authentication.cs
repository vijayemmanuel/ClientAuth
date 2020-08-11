using System;
using System. Collections.Generic;
using System.Linq;
using System. Text;
using System.Runtime.InteropServices;
using System. Security;

namespace Credentials
{
    /// <summary>
    /// Leverages the windows UI to prompt for a password
    /// </summary>
    internal static class Authentication
    {
        public struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [DllImport("credui")]
        private static extern CredUIReturnCodes CredUIPromptForCredentials (ref CREDUI_INFO creditUR,
            string targetName,
            IntPtr reserved1,
            int iError,
            StringBuilder userName,
            int maxUserName,
            StringBuilder password,
            int maxPassword,
            [MarshalAs (UnmanagedType.Bool)] ref bool pfSave,
            CREDUI FLAGS flags);

        [DllImport("credui")]
        private static extern CredUIReturnCodes CredUIParseUserName(string pszUserName,
            StringBuilder pszuser,
            int ulUserMaxChars,
            StringBuilder pszDomain,
            int ulDomainMaxChars);
                
        [Flags]
        enum CREDUI_FLAGS 
        {
            INCORRECT_PASSWORD = Ox1,
            DO_NOT_PERSIST = 0x2,
            REQUEST_ADMINISTRATOR = Øx4,
            EXCLUDE_CERTIFICATES = 0x8,
            REQUIRE_CERTIFICATE = 0x10,
            SHOW_SAVE_CHECK BOX = 0x40,
            ALWAYS_SHOW_UI = 0x80,
            REQUIRE_SMARTCARD = 0x100,
            PASSWORD_ONLY_OK = 0x200,
            VALIDATE _USERNAME = Ox400,
            COMPLETE_USERNAME = 0x800,
            PERSIST = 0x1000,
            SERVER_CREDENTIAL = 0x4000,
            EXPECT_CONFIRMATION = 0x20000,
            GENERIC_CREDENTIALS = 0x40000,
            USERNAME_TARGET_CREDENTIALS = 0x80000,
            KEEP_USERNAME = 0x100000,
        }
            
        public enum CredUIReturnCodes 
        {
            NO_ERROR = 0,
            ERROR_CANCELLED = 1223,
            ERROR_NO_SUCH_LOGON_SESSION = 1312,
            ERROR_NOT_FOUND = 1168,
            ERROR_INVALID_ACCOUNT_NAME = 1315,
            ERROR_INSUFFICIENT_BUFFER = 122,
            ERROR_INVALID_PARAMETER = 87,
            ERROR_INVALID_FLAGS = 1004,
        }
        /// <summary>
        /// Prompts for password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if no errors.</returns>

        internal static bool PromptForPassword(string targetName, string captionText, string messageText, out string user, out string password, out string domain)
        {
            user = "";
            password = "";
            domain = "";
        
            // Setup the flags and variables
            StringBuilder userPassword = new StringBuilder (256), userID = new StringBuilder(256);
            CREDUI_INF0 credUI = new CREDUI_INFO();
            credUI.cbSize = Marshal.Sizeof(credUI);
            credUI.pszCaptionText = captionText;
            credUI.pszMessageText = messageText;
            bool save = false;
            CREDUI_FLAGS flags = CREDUI_FLAGS.ALWAYS_SHOW_UI | CREDUI_FLAGS.GENERIC_CREDENTIALS | CREDUI_FLAGS.DO_NOT PERSIST;
        
            // Prompt the user
            CredUIReturnCodes returnCode = CredUIPromptForCredentials(ref credUI, targetName, IntPtr.Zero, 0, userID, 256, userPassword, 256, ref save, flags);
            if (returnCode == CredUIReturnCodes.NO_ERROR)
            {
                user = userID.ToString();
                
                // Using SecureString for password
                Char[] bufPasSword = new Char[userPassword.Length];
                userPassword.CopyTo(0, bufPassword, 0, userPassword.Length);

                for (Int32 ci = 0; ci < userPassword.Length; ci++ )
                {
                    password += userPassword [ci];
                    userPassword[ci] = '0'; // To safely remove it from memory
                }
                // Parse the User name for any possible Domain name
                StringBuilder tmpUser = new StringBuilder();
                StringBuilder tmpDomain = new StringBuilder();
                if (CredUIParseUserName (user, tmpUser, 256, tmpDomain, 256) == CredUIReturnCodes.NO_ERROR)
                {
                    user = tmpuser.ToString();
                    domain = tmpDomain.ToString();
                }
            }
            
            return (returnCode == CredUIReturnCodes. NO ERROR);
        }
        
    }
}
