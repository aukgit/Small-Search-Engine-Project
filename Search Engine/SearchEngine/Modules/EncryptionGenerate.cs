using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SearchEngine.Modules {
    public class EncryptionGenerate {
        //Generate MD5 Code
        public string GenerateCleanMD5(string input) {
            string coded;
            MD5 md5Hash = MD5.Create();
            coded = GetMd5Hash(md5Hash, input);
            return coded;
        }

        public string GetMD5FromFile(string FileName) {
            using (var md5 = MD5.Create()) {
                try {
                    using (var stream = File.OpenRead(FileName)) {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                    }
                } catch (Exception ex) {
                    ExceptionHandle.Handle(ex);
                }               
            }
            return null;
        }

        public string GetMd5Hash(MD5 md5Hash, string input) {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF32.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++) {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        public bool VerifyMd5Hash(MD5 md5Hash, string input, string hash) {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
