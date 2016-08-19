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

        public static string GenerateString()
        {
            char[] lowerChar = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] upperChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            char[] number = "0123456789".ToCharArray();

            bool haveLower = false;
            bool haveUpper = false;
            bool haveNumber = false;

            Random random = new Random();
            int length = random.Next(8, 15);

            StringBuilder result = new StringBuilder(length);

            while (!haveLower || !haveNumber || !haveUpper)
            {
                haveLower = false;
                haveUpper = false;
                haveNumber = false;

                result = new StringBuilder(length);

                for (int i = 1; i <= length; i++)
                {
                    int type = random.Next(1, 4);
                    int charPos = 0;
                    switch (type)
                    {
                        case 1:
                            haveLower = true;
                            charPos = random.Next(0, 26);
                            result.Append(lowerChar[charPos]);
                            break;
                        case 2:
                            haveUpper = true;
                            charPos = random.Next(0, 26);
                            result.Append(upperChar[charPos]);
                            break;
                        case 3:
                            haveNumber = true;
                            charPos = random.Next(0, 10);
                            result.Append(number[charPos]);
                            break;
                    }
                }
            }
            return result.ToString();
        }
    }
}