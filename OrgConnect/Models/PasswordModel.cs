using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OrgConnect.Models
{
    public static class PasswordModel
    {
        public static string Hash(string password)
        {
            throw new InvalidOperationException(
                "Password saving is disabled until Argon2id is configured."
            );
        }
    }
}