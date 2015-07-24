//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Security.Cryptography;

//namespace AOI.Crypto {
//    public class HMAC {
//        private byte[] _key;
//        public HMAC(byte[] key = null) {
//            this._key = key;
//        }

//        #region -------- PUBLIC - MD5 --------
//        public byte[] MD5(string data) {
//            return MD5(System.Text.Encoding.Default.GetBytes(data));
//        }
//        public byte[] MD5(byte[] data) {
//            var hmac = (this._key == null) ? new HMACMD5() : new HMACMD5(this._key);
//            var hmacBytes = hmac.ComputeHash(data);
//            return hmacBytes;
//        }
//        public string MD5String(string data) {
//            return MD5String(System.Text.Encoding.Default.GetBytes(data));
//        }
//        public string MD5String(byte[] data) {
//            var hmac = (this._key == null) ? new HMACMD5() : new HMACMD5(this._key);
//            var hmacBytes = hmac.ComputeHash(data);
//            return System.Text.Encoding.Default.GetString(hmacBytes);
//        }
//        #endregion


//        #region -------- PUBLIC - SHA1 --------
//        public byte[] SHA1(string data) {
//            return SHA1(System.Text.Encoding.Default.GetBytes(data));
//        }
//        public byte[] SHA1(byte[] data) {
//            var hmac = (this._key == null) ? new HMACSHA1() : new HMACSHA1(this._key);
//            var hmacBytes = hmac.ComputeHash(data);
//            return hmacBytes;
//        }
//        public string SHA1String(string data) {
//            return SHA1String(System.Text.Encoding.Default.GetBytes(data));
//        }
//        public string SHA1String(byte[] data) {
//            var hmac = (this._key == null) ? new HMACSHA1() : new HMACSHA1(this._key);
//            var hmacBytes = hmac.ComputeHash(data);
//            return System.Text.Encoding.Default.GetString(hmacBytes);
//        }
//        #endregion
//    }
//}
