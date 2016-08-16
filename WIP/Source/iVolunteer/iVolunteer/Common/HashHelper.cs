using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.IO;

namespace iVolunteer.Common
{
    /// <summary>
    /// autokey encrypt 
    /// source : http://www.codeproject.com/Questions/523323/Encryptingpluspasswordplusinplusc
    /// https://msdn.microsoft.com/en-us/library/wet69s13(v=vs.110).aspx
    /// </summary>
    public static class HashHelper
    {
        public static string Hash(string plainText)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(plainText);
            byte[] inArray = HashAlgorithm.Create("SHA256").ComputeHash(bytes);
            return Convert.ToBase64String(inArray);
        }
    }
}