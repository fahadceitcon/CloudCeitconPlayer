using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Ceitcon_Data.Utilities
{
    //Test code for Designer
    //XElement text = XElement.Load(@"C:\Workspace\Ceitcon\Ceitcon DS\Ceitcon DS Designer Prototype\bin\Debug\Projects\6\6.cdp");
    //string cryptedText = Crypt.Encrypt(text.ToString());
    //System.IO.File.WriteAllText(@"C:\Workspace\Ceitcon\Ceitcon DS\Ceitcon DS Designer Prototype\bin\Debug\Projects\6\8.cdp", cryptedText);

    //Test code for Player
    //cryptedText = System.IO.File.ReadAllText(@"C:\Workspace\Ceitcon\Ceitcon DS\Ceitcon DS Designer Prototype\bin\Debug\Projects\6\8.cdp");
    //string openText = Crypt.Decrypt(cryptedText);
    //XmlDocument xmlDoc = new XmlDocument();
    //xmlDoc.LoadXml(openText);
    //xmlDoc.Save(@"C:\Workspace\Ceitcon\Ceitcon DS\Ceitcon DS Designer Prototype\bin\Debug\Projects\6\8.cdp");


    internal static class Crypt
    {
        //For each player/customer
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        //On Designer side
        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        //On Player side
        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
    }
}