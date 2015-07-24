using System.Security.Cryptography;
using System.IO;

namespace AOI.Crypto {
    public class AES {
        private byte[] _key;
        private byte[] _salt;
        private int _iterations;
        public AES(string key, string salt = null, int iterations = 1000) {
            this._key = System.Text.Encoding.Default.GetBytes(key);
            this._iterations = iterations;

            if (salt != null) {
                this._salt = System.Text.Encoding.Default.GetBytes(salt);
            } else {
                this._salt = new byte[16];
                for (var i = 0; i < 16; i++) {
                    this._salt[i] = this._key[i];
                }
            }
        }

        public string Encrypt(string data) {
            var input = System.Text.Encoding.Default.GetBytes(data);
            byte[] output = null;

            using (MemoryStream ms = new MemoryStream()) {
                using (RijndaelManaged AES = new RijndaelManaged()) {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(this._key, this._salt, this._iterations);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write)) {
                        cs.Write(input, 0, input.Length);
                        cs.Close();
                    }
                    output = ms.ToArray();
                }
            }

            return System.Text.Encoding.Default.GetString(output);
        }


        public string Decrypt(string data) {
            var input = System.Text.Encoding.Default.GetBytes(data);
            byte[] output = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream()) {
                using (RijndaelManaged AES = new RijndaelManaged()) {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(this._key, this._salt, this._iterations);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write)) {
                        cs.Write(input, 0, input.Length);
                        cs.Close();
                    }
                    output = ms.ToArray();
                }
            }

            return System.Text.Encoding.Default.GetString(output);
        }
    }
}
