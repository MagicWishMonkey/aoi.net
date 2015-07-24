using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using AOI.Util.Codecs;
using AOI.Util.IO;

namespace AOI.Util {
    public static class Toolkit {
        //public delegate void FN();
        //public delegate void Callback(object o);

        public static Func<TResult> Curry<T1, TResult>(Func<T1, TResult> function, T1 arg) {
            return () => function(arg);
        }

        public static Func<T2, TResult> Curry<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg) {
            return (b) => function(arg, b);
        }

        public static Func<T2, T3, TResult> Curry<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg) {
            return (b, c) => function(arg, b, c);
        }

        public static JSON JSON {
            get { return new JSON(); }
        }

        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> s, Func<T, U> f) {
            var results = new List<U>();
            foreach (var item in s) {
                //yield return f(item);
                var o = f(item);
                results.Add(o);
            }
            return results;
        }

        //public static string Base36(int num) {
        //    return AOI.Util.Base36Formatter.Encode(num);
        //}

        //public static int UnBase36(string data) {
        //    return Base36Formatter.Decode(data);
        //}



        public static string Base64(string txt) {
            var bytes = Text.ReadBytes(txt);
            return Base64(bytes);
        }
        public static string Base64(byte[] bytes) {
            var txt = System.Convert.ToBase64String(bytes);
            return txt;
        }

        public static string UnBase64(byte[] bytes) {
            var txt = System.Text.Encoding.Unicode.GetString(bytes);
            return UnBase64(txt);
        }
        public static string UnBase64(string txt) {
            var bytes = System.Convert.FromBase64String(txt);
            return Text.ReadString(bytes);
        }

        public static byte[] FromBase64String(byte[] bytes) {
            var txt = Text.ReadString(bytes);
            return System.Convert.FromBase64String(txt);
        }
        public static byte[] FromBase64String(string txt) {
            var bytes = System.Convert.FromBase64String(txt);
            return bytes;
        }


        public static byte[] ToBytes(string txt) {
            var bytes = Text.ReadBytes(txt);
            return bytes;
        }

        public static Directory Directory() {
            var path = System.IO.Directory.GetCurrentDirectory();
            return new Directory(path);
        }
        public static Directory Directory(string path) {
            return new Directory(path);
        }

        public static File File(string path) {
            return new File(path);
        }

        public static string Hash(string data) {
            return AOI.Crypto.Hash.MD5(data);
        }

        public static string GUID(int length = 32){
            if (length <= 32) { 
                var uuid = System.Guid.NewGuid().ToString();
                var txt = AOI.Crypto.Hash.MD5(uuid);
                if (length < 32)
                    txt = txt.Substring(0, length);
                return txt;
            }

            var buffer = new System.Text.StringBuilder();
            while (buffer.Length < length) {
                var uuid = System.Guid.NewGuid().ToString();
                var txt = AOI.Crypto.Hash.MD5(uuid);
                buffer.Append(txt);
            }

            var str = buffer.ToString();
            if (str.Length > length)
                str = str.Substring(0, length);
            return str;
        }


        public static byte[] DumpStream(System.IO.Stream stream) {
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            using (System.IO.BinaryReader br = new System.IO.BinaryReader(stream)) {
                byte[] bytes = br.ReadBytes((int)stream.Length);
                return bytes;
            }
        }


        public static byte[] Pickle(object o) {
            var stream = new System.IO.MemoryStream();
            var formatter = new BinaryFormatter();
            
            try { 
                formatter.Serialize(stream, o);
                return DumpStream(stream);
            } catch (Exception ex) {
                throw ex;
            } finally {
                stream.Close();
                stream.Dispose();
            }
        }

        public static T Unpickle<T>(byte[] data) {
            var value = Unpickle(data);
            return (T)value;
        }
        public static object Unpickle(byte[] data) {
            var stream = new System.IO.MemoryStream(data);
            var formatter = new BinaryFormatter();
            try {
                var o = formatter.Deserialize(stream);
                return o;
            } catch (Exception ex) {
                throw ex;
            } finally {
                stream.Close();
                stream.Dispose();
            }
        }

        #region -------- URI METHODS --------
        /// <summary>
        /// Create a uri instance for the specified path.
        /// </summary>
        /// <param name="path">The path to create the uri from.</param>
        /// <returns>A uri instance.</returns>
        public static Uri ToUri(string path) {
            if (String.IsNullOrEmpty(path))
                return null;
            Uri rez;
            if (Uri.TryCreate(path, UriKind.Absolute, out rez))
                return null;
            return rez;
        }

        /// <summary>
        /// Check and see if the file is a valid uri, return 
        /// true if it is, false otherwise
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>A boolean flag specifying whether or not the path is a valid uri</returns>
        public static bool IsValidUri(string path) {
            if (String.IsNullOrEmpty(path))
                return false;
            Uri rez;
            if (Uri.TryCreate(path, UriKind.Absolute, out rez))
                return true;
            return false;
        }

        /// <summary>
        /// Check and see if the uri points to a remote file.Return 
        /// true if it is, false otherwise
        /// </summary>
        /// <param name="uri">The uri to check</param>
        /// <returns>A boolean flag specifying whether or not the path is a valid web uri</returns>
        public static bool IsWebUri(Uri uri) {
            return IsWebUri(uri.AbsoluteUri);
        }

        /// <summary>
        /// Check and see if the path is a valid web uri, return 
        /// true if it is, false otherwise
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>A boolean flag specifying whether or not the path is a valid web uri</returns>
        public static bool IsWebUri(string path) {
            if (!IsValidUri(path))
                return false;
            path = path.ToLower();
            if (path.StartsWith("http:") || path.StartsWith("https:"))// || path.StartsWith("www."))
                return true;
            return false;
        }
        #endregion

        public static void Override(Dictionary<string, dynamic> a, Dictionary<string, dynamic> b) {
            var keys = b.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++) {
                var key = keys[i];
                var val = b[key];

                dynamic existing = null;
                if (a.TryGetValue(key, out existing)) {
                    if (val is Dictionary<string, dynamic>) {
                        if (existing is Dictionary<string, dynamic>)
                            Override(existing, val);
                    }
                }
                a[key] = val;
            }
            


            //foreach (KeyValuePair<string, dynamic> entry in b) {
            //    var key = entry.Key;
            //    var val = entry.Value;

            //    dynamic existing = null;
            //    if (a.TryGetValue(key, out existing)) {
            //        if (val is IDictionary)
            //            if (existing is IDictionary)
            //                Override(existing, val);
            //            else
            //                a[key] = val;
            //        else
            //            a[key] = val;
            //    } else {
            //        a[key] = val;
            //    }
            //}
        }





        public static Directory CurrentDirectory {
            get {
                var path = System.IO.Directory.GetCurrentDirectory();
                return new Directory(path);
            }
        }


        #region -------- HtmlEncode/HtmlDecode --------
        public static string HtmlDecode(string s) {
            if (s == null) {
                return null;
            }
            if (s.IndexOf('&') < 0) {
                return s;
            }
            StringBuilder sb = new StringBuilder();
            var output = new System.IO.StringWriter(sb);
            HtmlDecode(s, output);
            return sb.ToString();
        }

        public static void HtmlDecode(string s, System.IO.TextWriter output) {
            var _entityEndingChars = new char[] { ';', '&' };

            if (s != null) {
                if (s.IndexOf('&') < 0) {
                    output.Write(s);
                } else {
                    int length = s.Length;
                    for (int i = 0; i < length; i++) {
                        char ch = s[i];
                        if (ch == '&') {
                            int num3 = s.IndexOfAny(_entityEndingChars, i + 1);
                            if ((num3 > 0) && (s[num3] == ';')) {
                                string entity = s.Substring(i + 1, (num3 - i) - 1);
                                if ((entity.Length > 1) && (entity[0] == '#')) {
                                    try {
                                        if ((entity[1] == 'x') || (entity[1] == 'X')) {
                                            ch = (char)int.Parse(entity.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
                                        } else {
                                            ch = (char)int.Parse(entity.Substring(1));
                                        }
                                        i = num3;
                                    } catch (FormatException) {
                                        i++;
                                    } catch (ArgumentException) {
                                        i++;
                                    }
                                } else {
                                    i = num3;
                                    char ch2 = HtmlEntities.Lookup(entity);
                                    if (ch2 != '\0') {
                                        ch = ch2;
                                    } else {
                                        output.Write('&');
                                        output.Write(entity);
                                        output.Write(';');
                                        goto Label_0103;
                                    }
                                }
                            }
                        }
                        output.Write(ch);
                    Label_0103: ;
                    }
                }
            }
        }
      
        public static string HtmlEncode(string s) {
            throw new NotImplementedException();
        }
        #endregion


        #region -------- UrlEncode/UrlDecode --------
        public static string UrlDecode(string s) {
            return UrlDecode(s, Encoding.UTF8);
        }

        public static string UrlDecode(string s, Encoding e) {
            if (String.IsNullOrEmpty(s))
                return s;
            return UrlDecodeStringFromStringInternal(s, e);
        }

        private static string UrlDecodeStringFromStringInternal(string s, Encoding e) {
            int length = s.Length;
            UrlDecoder decoder = new UrlDecoder(length, e);
            for (int i = 0; i < length; i++) {
                char ch = s[i];
                if (ch == '+') {
                    ch = ' ';
                } else if ((ch == '%') && (i < (length - 2))) {
                    if ((s[i + 1] == 'u') && (i < (length - 5))) {
                        int num3 = HexToInt(s[i + 2]);
                        int num4 = HexToInt(s[i + 3]);
                        int num5 = HexToInt(s[i + 4]);
                        int num6 = HexToInt(s[i + 5]);
                        if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0))) {
                            goto Label_0106;
                        }
                        ch = (char)((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                        i += 5;
                        decoder.AddChar(ch);
                        continue;
                    }
                    int num7 = HexToInt(s[i + 1]);
                    int num8 = HexToInt(s[i + 2]);
                    if ((num7 >= 0) && (num8 >= 0)) {
                        byte b = (byte)((num7 << 4) | num8);
                        i += 2;
                        decoder.AddByte(b);
                        continue;
                    }
                }
            Label_0106:
                if ((ch & 0xff80) == 0) {
                    decoder.AddByte((byte)ch);
                } else {
                    decoder.AddChar(ch);
                }
            }
            return decoder.GetString();
        }

        private static int HexToInt(char h) {
            if ((h >= '0') && (h <= '9')) {
                return (h - '0');
            }
            if ((h >= 'a') && (h <= 'f')) {
                return ((h - 'a') + 10);
            }
            if ((h >= 'A') && (h <= 'F')) {
                return ((h - 'A') + 10);
            }
            return -1;
        }

        public static string UrlEncode(string s) {
            return UrlEncode(s, Encoding.UTF8);
        }
        public static string UrlEncode(string s, Encoding e) {
            if (s == null)
                return s;
            return Encoding.ASCII.GetString(UrlEncodeToBytes(s, e));
        }
        public static byte[] UrlEncodeToBytes(string str, Encoding e) {
            if (str == null) {
                return null;
            }
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }
        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue) {
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < count; i++) {
                char ch = (char)bytes[offset + i];
                if (ch == ' ') {
                    num++;
                } else if (!IsSafe(ch)) {
                    num2++;
                }
            }
            if ((!alwaysCreateReturnValue && (num == 0)) && (num2 == 0)) {
                return bytes;
            }
            byte[] buffer = new byte[count + (num2 * 2)];
            int num4 = 0;
            for (int j = 0; j < count; j++) {
                byte num6 = bytes[offset + j];
                char ch2 = (char)num6;
                if (IsSafe(ch2)) {
                    buffer[num4++] = num6;
                } else if (ch2 == ' ') {
                    buffer[num4++] = 0x2b;
                } else {
                    buffer[num4++] = 0x25;
                    buffer[num4++] = (byte)IntToHex((num6 >> 4) & 15);
                    buffer[num4++] = (byte)IntToHex(num6 & 15);
                }
            }
            return buffer;
        }

        internal static char IntToHex(int n) {
            if (n <= 9) {
                return (char)(n + 0x30);
            }
            return (char)((n - 10) + 0x61);
        }

        internal static bool IsSafe(char ch) {
            if ((((ch >= 'a') && (ch <= 'z')) || ((ch >= 'A') && (ch <= 'Z'))) || ((ch >= '0') && (ch <= '9'))) {
                return true;
            }
            switch (ch) {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                case '!':
                    return true;
            }
            return false;
        }
        #endregion

    }
}
