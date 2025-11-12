using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos.util
{
    public class cEncriptacion
    {

        public cEncriptacion()
        {
        }



        /// <summary>
        /// Encript InClear Text using RC4 method using EncriptionKey
        /// Put the result into CryptedText 
        /// </summary>
        /// <returns>true if success, else false</returns>
        public bool mtdEncrypt()
        {

            // toRet is used to store function retcode
            bool booToRet = true;
            // indexes used below
            long i = 0;
            long j = 0;
            // Put input string in temporary byte array
            Encoding encTexto = Encoding.Default;


            try
            {
                byte[] bytInput = encTexto.GetBytes(this.strM_sInClearText);

                // Output byte array
                byte[] bytOutput = new byte[bytInput.Length];

                // Local copy of m_nBoxLen
                byte[] n_LocBox = new byte[lonM_nLength];
                this.bytM_nBox.CopyTo(n_LocBox, 0);

                //	Len of Chipher
                long ChipherLen = bytInput.Length + 1;

                // Run Alghoritm

                for (long offset = 0; offset < bytInput.Length; offset++)
                {
                    i = (i + 1) % lonM_nLength;
                    j = (j + n_LocBox[i]) % lonM_nLength;
                    byte temp = n_LocBox[i];
                    n_LocBox[i] = n_LocBox[j];
                    n_LocBox[j] = temp;
                    byte a = bytInput[offset];
                    byte b = n_LocBox[(n_LocBox[i] + n_LocBox[j]) % lonM_nLength];
                    bytOutput[offset] = (byte)((int)a ^ (int)b);
                }


                // Put result into output string ( CryptedText )
                char[] outarrchar = new char[encTexto.GetCharCount(bytOutput, 0, bytOutput.Length)];
                encTexto.GetChars(bytOutput, 0, bytOutput.Length, outarrchar, 0);
                this.strM_sCryptedText = new string(outarrchar);
            }
            catch
            {
                // error occured - set retcode to false.
                booToRet = false;
            }

            // return retcode
            return (booToRet);
        }

        /// <summary>
        /// Decript CryptedText into InClearText using EncriptionKey
        /// </summary>
        /// <returns>true if success else false</returns>
        public bool mtdDecrypt()
        {

            // toRet is used to store function retcode
            bool booToRet = true;


            try
            {
                this.strM_sInClearText = this.strM_sCryptedText;
                strM_sCryptedText = string.Empty;

                if (booToRet = mtdEncrypt())
                    strM_sInClearText = strM_sCryptedText;
            }
            catch
            {
                // error occured - set retcode to false.
                booToRet = false;
            }

            // return retcode
            return booToRet;
        }


        /// <summary>
        /// Get or set Encryption Key
        /// </summary>
        public string EncryptionKey
        {
            get
            {
                return (this.strM_sEncryptionKey);
            }
            set
            {
                // assign value only if it is a new value
                if (this.strM_sEncryptionKey != value)
                {
                    this.strM_sEncryptionKey = value;

                    // Used to populate m_nBox
                    long index2 = 0;

                    // Create two different encoding 
                    Encoding ascii = Encoding.Default;
                    Encoding unicode = Encoding.Default;

                    // Perform the conversion of the encryption key from unicode to ansi
                    byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicode.GetBytes(this.strM_sEncryptionKey));

                    // Convert the new byte[] into a char[] and then to string
                    char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
                    ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
                    this.strM_sEncryptionKeyAscii = new string(asciiChars);

                    // Populate m_nBox
                    long KeyLen = strM_sEncryptionKey.Length;

                    // First Loop
                    for (long count = 0; count < lonM_nLength; count++)
                    {
                        this.bytM_nBox[count] = (byte)count;
                    }

                    // Second Loop
                    for (long count = 0; count < lonM_nLength; count++)
                    {
                        index2 = (index2 + bytM_nBox[count] + asciiChars[count % KeyLen]) % lonM_nLength;
                        byte temp = bytM_nBox[count];
                        bytM_nBox[count] = bytM_nBox[index2];
                        bytM_nBox[index2] = temp;
                    }
                }
            }
        }

        public string mtdInClearText
        {
            get
            {
                return (this.strM_sInClearText);
            }
            set
            {
                // assign value only if it is a new value
                if (this.strM_sInClearText != value)
                {
                    this.strM_sInClearText = value;
                }
            }
        }

        public string CryptedText
        {
            get
            {
                return (this.strM_sCryptedText);
            }
            set
            {
                // assign value only if it is a new value
                if (this.strM_sCryptedText != value)
                    this.strM_sCryptedText = value;
            }
        }



        // Encryption Key  - Unicode & Ascii version
        private string strM_sEncryptionKey = string.Empty;
        private string strM_sEncryptionKeyAscii = string.Empty;

        // It is related to Encryption Key
        protected byte[] bytM_nBox = new byte[lonM_nLength];

        // Len of nBox
        static public long lonM_nLength = 255;

        // In Clear Text
        private string strM_sInClearText = string.Empty;
        private string strM_sCryptedText = string.Empty;



        public static string Base64_Encode(string str)
        {
            string cambiocaracteres = str.Replace("*", "softrisk1");
            string encodedStr = Convert.ToBase64String(Encoding.Default.GetBytes(cambiocaracteres));

            //byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(str);
            return encodedStr;
        }

        //Decodificar base64
        public static string Base64_Decode(string str)
        {
            try


            {
                string cambiocaracteres = Encoding.Default.GetString(Convert.FromBase64String(str));
                string decodificado = cambiocaracteres.Replace("softrisk1", "*");
                //return System.Text.Encoding.Default.GetString(decbuff);
                //var base64EncodedBytes = System.Convert.FromBase64String(str);
                // return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                return decodificado;
            }
            catch
            {
                //si se envia una cadena si codificación base64, mandamos vacio
                return "";
            }
        }

        private static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearData, 0, clearData.Length);
            cs.Close();
            byte[] encryptedData = ms.ToArray();
            return encryptedData;
        }
        public static string Encrypt(string Data, string Password, int Bits)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(Data);
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x0, 0x1, 0x2, 0x1C, 0x1D, 0x1E, 0x3, 0x4, 0x5, 0xF, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
            if (Bits == 128)
            {
                byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else if (Bits == 192)
            {
                byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else if (Bits == 256)
            {
                byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                return Convert.ToBase64String(encryptedData);
            }
            else
            {
                return String.Concat(Bits);
            }

        }
        private static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherData, 0, cipherData.Length);
            cs.Close();
            byte[] decryptedData = ms.ToArray();
            return decryptedData;
        }
        public static string Decrypt(string Data, string Password, int Bits)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(Data);
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Password, new byte[] { 0x0, 0x1, 0x2, 0x1C, 0x1D, 0x1E, 0x3, 0x4, 0x5, 0xF, 0x20, 0x21, 0xAD, 0xAF, 0xA4 });
                if (Bits == 128)
                {
                    byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(16), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else if (Bits == 192)
                {
                    byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(24), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else if (Bits == 256)
                {
                    byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16));
                    return System.Text.Encoding.Unicode.GetString(decryptedData);
                }
                else
                {
                    return String.Concat(Bits);
                }
            }
            catch (Exception ex)
            {
                return String.Concat(Bits);
            }
        }


        public static string DesCifradoData(string textToDecrypt)
        {

            try
            {
                //string textToDecrypt = "6+PXxVWlBqcUnIdqsMyUHA==";
                string ToReturn = "";
                string publickey = "12345678";
                string secretkey = "87654321";
                byte[] privatekeyByte = { };
                privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
                inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    Encoding encoding = Encoding.UTF8;
                    ToReturn = encoding.GetString(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ae)
            {
                throw new Exception(ae.Message, ae.InnerException);
            }

        }

        public static string CifradoData(string textToEncrypt)
        {
            try
            {
                //string textToEncrypt = "WaterWorld";
                string ToReturn = "";
                string publickey = "12345678";
                string secretkey = "87654321";
                byte[] secretkeyByte = { };
                secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
                byte[] publickeybyte = { };
                publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
                MemoryStream ms = null;
                CryptoStream cs = null;
                byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
                using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
                {
                    ms = new MemoryStream();
                    cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                    cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                    cs.FlushFinalBlock();
                    ToReturn = Convert.ToBase64String(ms.ToArray());
                }
                return ToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }

        }



    }
}
