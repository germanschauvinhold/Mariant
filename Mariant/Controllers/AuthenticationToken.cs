using System;

namespace WebAsistida.lib
{
    public class AuthenticationToken
    {
        public static String Make(String WebSession_id, String WebUser_id, String[] WebSessionRoles, String WebUser_EMail)
        {
            return "";
        }

        public static void Parse(string authToken, out String WebSession_id, out String WebUser_id, out String[] WebSessionRoles, out String WebUser_EMail)
        {

            String authVal = CryptEngine.Decrypt(authToken, true);

            String[] credentials = authVal.Split(new[] { ':' });

            WebSession_id = null;
            WebUser_id = null;
            WebSessionRoles = null;
            WebUser_EMail = null;

            if (credentials.Length < 4 || string.IsNullOrEmpty(credentials[0])
                || string.IsNullOrEmpty(credentials[1])) return;

            WebSession_id = credentials[0];
            WebUser_id = credentials[1];
            String rolestring = credentials[2];
            WebSessionRoles = rolestring.Split(new[] { ',' });
            WebUser_EMail = credentials[3];
        }
    }
}