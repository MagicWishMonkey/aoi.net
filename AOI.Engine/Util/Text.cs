using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
namespace AOI.Util {
    public sealed class Text {
        #region -------- PUBLIC - ReadLine --------
        public static string ReadLine(string txt, int lineIX) {
            if (String.IsNullOrEmpty(txt))
                return "";
            if (lineIX < 0)
                lineIX = 0;

            StringReader reader = new StringReader(txt);
            string line = reader.ReadLine();
            for (int i = 1; i < lineIX; i++) {
                if (String.IsNullOrEmpty(line))
                    return "";
                line = reader.ReadLine();
            }

            return line;
        }
        #endregion

        #region -------- PUBLIC - ReadBytes/ReadString --------
        public static byte[] ReadBytes(string txt) {
            if (txt == null || txt.Length == 0) return new byte[0];
            return System.Text.Encoding.Default.GetBytes(txt);
        }
        public static string ReadString(byte[] data) {
            if (data == null || data.Length == 0) return "";
            return System.Text.Encoding.Default.GetString(data);
        }
        #endregion

        #region -------- PUBLIC - ToHex/ToBytes/FromBytes --------
        public static string ToHex(byte[] data) {
            return Hex.ToString(data);
        }
        public static byte[] ToBytes(string txt) {
            if (String.IsNullOrEmpty(txt))
                return new byte[0];//throw new ArgumentNullException("The txt parameter is null/empty!");
            return System.Text.Encoding.Default.GetBytes(txt);
        }

        public static string FromBytes(byte[] data) {
            return FromBytes(data, System.Text.Encoding.Default);
        }
        public static string FromBytes(byte[] data, System.Text.Encoding encoding) {
            if (data == null || data.Length == 0)
                return "";//    throw new ArgumentNullException("The data parameter is null/empty!");

            if (encoding == null)
                encoding = System.Text.Encoding.Default;

            return encoding.GetString(data);
        }
        #endregion

        #region -------- PUBLIC - Capitalize --------
        public static string Capitalize(string txt) {
            if (String.IsNullOrEmpty(txt)) return "";
            if (txt.Length == 1) return txt.ToUpper();
            txt = (txt[0].ToString().ToUpper() + txt.Substring(1));
            return txt;
        }
        #endregion

        #region -------- PUBLIC - IsInt --------
        //public static int TryParseInt(string txt, int defaultValue) {
        //    int val = defaultValue;
        //    Int32.TryParse(txt, out val);
        //    return val;
        //}

        public static bool IsInt(string txt) {
            int val = 0;
            return Int32.TryParse(txt, out val);
        }
        #endregion

        #region -------- SmartCapitalize --------
        public static string SmartCapitalize(string txt) {
            if (String.IsNullOrEmpty(txt)) return "";

            int[] indexes = FindIndicesOf(txt, '-');
            txt = txt.Replace("-", " ");
            string[] parts = txt.ToLower().Split(' ');
            System.Text.StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < parts.Length; i++) {
                string part = parts[i];
                if (part.Length == 0) {
                    buffer.Append(" ");
                } else if (part[0] != ' ') {
                    string block = part;
                    block = part[0].ToString().ToUpper();
                    if (part.Length > 1)
                        block += part.Substring(1);
                    buffer.Append(block);
                } else {
                    buffer.Append(part);
                }
                if (i < (parts.Length - 1))
                    buffer.Append(" ");
            }
            foreach (int ix in indexes) {
                buffer[ix] = '-';
            }
            return buffer.ToString();
        }
        public static int[] FindIndicesOf(string txt, char findChar) {
            List<int> indexes = new List<int>();
            if (String.IsNullOrEmpty(txt)) return indexes.ToArray();
            int offset = 0;
            int ix = txt.IndexOf(findChar);
            while (ix > -1) {
                offset += ix;
                indexes.Add(offset);
                txt = txt.Substring(ix + 1);
                ix = txt.IndexOf(findChar);
            }
            return indexes.ToArray();
        }
        #endregion

        #region -------- PUBLIC - FilterExtraneousSpaces --------
        public static string FilterExtraneousSpaces(string txt) {
            if (String.IsNullOrEmpty(txt)) return "";
            while (txt.IndexOf("  ") > -1) {
                txt = txt.Replace("  ", " ");
            }
            return txt.Trim();
        }
        #endregion

        #region -------- PUBLIC - StripPunctuation --------
        public static string StripPunctuation(string txt, string replacementChar = "", params char[] omitFromFilter) {
            if (String.IsNullOrEmpty(txt))
                return "";

            char[] ommissions = (omitFromFilter != null && omitFromFilter.Length > 0) ? omitFromFilter : null;
            int count = txt.Length;
            System.Text.StringBuilder buffer = new StringBuilder(count);
            for (int i = 0; i < count; i++) {
                char c = txt[i];
                if (char.IsPunctuation(c)) {
                    if (ommissions != null && IsInList(c, ref ommissions))
                        buffer.Append(c);
                    else
                        buffer.Append(replacementChar);
                } else {
                    buffer.Append(c);
                }
            }
            return buffer.ToString();
        }
        #endregion

        private static bool IsInList(char c, ref char[] chars) {
            for (int i = 0; i < chars.Length; i++) {
                if (chars[i] == c)
                    return true;
            }
            return false;
        }

        #region -------- PUBLIC - FilterNonAlphaNumeric/FilterNonAlpha --------
        public static string FilterNonAlphaNumeric(string txt) {
            if (String.IsNullOrEmpty(txt))
                return txt;

            Regex rgx = new Regex(@"[^a-z 0-9]", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            txt = rgx.Replace(txt, "");
            return txt;
        }

        public static string FilterNonAlpha(string txt, string replacementText = "") {
            if (String.IsNullOrEmpty(txt))
                return txt;

            Regex rgx = new Regex(@"[^a-z]", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            txt = rgx.Replace(txt, replacementText);
            return txt;
        }
        #endregion

        #region -------- PUBLIC - RemoveStrings --------
        //public static string ReplaceStrings(string text, params string[] strings)
        //{
        //    return ReplaceStrings(text, null, strings);
        //}
        public static string RemoveStrings(string text, params string[] strings) {
            if (String.IsNullOrEmpty(text))
                return "";

            if (strings == null || strings.Length == 0)
                return text;


            foreach (string str in strings) {
                text = text.Replace(str, "");
            }

            return text;
        }
        #endregion

        #region -------- PUBLIC - Duplicate --------
        public static string Duplicate(char c, int count) {
            return Duplicate(c.ToString(), count);
        }

        public static string Duplicate(string txt, int count) {
            if (count == 0) return "";
            if (count == 1) return txt;

            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < count; i++) {
                buffer.Append(txt);
            }
            return buffer.ToString();
        }
        #endregion

        #region -------- PUBLIC - Explode --------
        public static List<string> Explode(string txt) {
            //if (txt == null) throw new NullEmptyException("txt");
            if (String.IsNullOrEmpty(txt)) return new List<string>(0);
            List<string> lines = new List<string>();

            System.IO.StringReader reader = new System.IO.StringReader(txt);
            while (true) {
                string line = reader.ReadLine();
                if (line == null)
                    break;

                lines.Add(line);
            }
            reader.Dispose();

            return lines;
        }
        #endregion

        #region -------- PUBLIC - Merge --------
        public static string Merge(List<string> lines) {
            if (lines == null) return "";

            return Merge(lines.ToArray());
        }

        public static string Merge(string[] lines) {
            if (lines == null || lines.Length == 0) return "";
            if (lines.Length == 1) return lines[0];
            StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i];
                if (i > 0)
                    buffer.AppendLine();
                buffer.Append(line);
            }
            return buffer.ToString();
        }
        #endregion


        #region -------- PUBLIC - FilterFileName/HasInvalidFileChars --------
        public static string FilterFileName(string fileName) {
            return FilterFileName(fileName, "");
        }
        public static string FilterFileName(string fileName, string replaceText) {
            if (String.IsNullOrEmpty(fileName))
                return "";
            Regex filter = new Regex("[^A-Z a-z 0-9 -_!%@().]", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);
            fileName = filter.Replace(fileName, replaceText);
            return fileName;

            //_matchInvalidFileCharacters = new Regex(@"[\/:?^*<>|&%#]", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }


        public static bool HasInvalidFileChars(string fileName) {
            if (String.IsNullOrEmpty(fileName))
                return false;

            string filtered = FilterFileName(fileName, "");
            return !filtered.Equals(fileName);
        }

        //public static string GetInvalidFileChars(string fileName) {
        //    if (String.IsNullOrEmpty(fileName))
        //        return "";

        //    string filtered = FilterFileName(fileName, " ");

        //}
        #endregion

        /// <summary>
        /// Returns a string that represents each of the characters present in BOTH string.
        /// 
        /// For example: 
        ///     string stringA = ABC123
        ///     string b = ABBB23
        ///     delta = AB  23
        /// </summary>
        /// <returns>A string that represents each of the characters present in both strings.</returns>
        public static string GetDelta(string stringA, string stringB) {
            if (String.IsNullOrEmpty(stringA))
                throw new ArgumentNullException("The stringAa parameter is null/empty!");

            int count = (stringA.Length > stringB.Length) ? stringB.Length : stringA.Length;

            char mismatch = '\0';

            System.Text.StringBuilder buffer = new StringBuilder();
            for (int i = 0; i < count; i++) {
                char charA = stringA[i];
                char charB = stringB[i];
                if (charA == charB)
                    buffer.Append(charA);
                else
                    buffer.Append(mismatch);
            }

            return buffer.ToString();
        }

    }
}
