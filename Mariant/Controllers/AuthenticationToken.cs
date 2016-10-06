using System;

namespace WebAsistida.lib
{
    public class AuthenticationToken
    {
        public static String Make(String RepSession_id, String RepUser_id, String[] RepSessionRoles, String RepUser_EMail)
        {
            return "";
        }

        public static void Parse(string authToken, out String RepSession_id, out String RepUser_id, out String[] RepSessionRoles, out String RepUser_EMail)
        {

            String authVal = CryptEngine.Decrypt(authToken, true);

            String[] credentials = authVal.Split(new[] { ':' });

            RepSession_id = null;
            RepUser_id = null;
            RepSessionRoles = null;
            RepUser_EMail = null;

            if (credentials.Length < 4 || string.IsNullOrEmpty(credentials[0])
                || string.IsNullOrEmpty(credentials[1])) return;

            RepSession_id = credentials[0];
            RepUser_id = credentials[1];
            String rolestring = credentials[2];
            RepSessionRoles = rolestring.Split(new[] { ',' });
            RepUser_EMail = credentials[3];
        }
    }
}