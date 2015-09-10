using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fuze.Util;
using Fuze.Crypto;


namespace AOI
{
    public class Helper
    {
        #region -------- PUBLIC - BuildAccountHash --------
        public static string BuildAccountHash(string ssn, string firstName, string lastName, string state)
        {
            ssn = new System.Text.RegularExpressions.Regex(
                "[^0-9]",
                System.Text.RegularExpressions.RegexOptions.Compiled
            ).Replace(ssn, "");

            ssn = ssn.Substring(3, 6);

            var buffer = new StringBuilder();
            buffer.Append(ssn);
            buffer.Append(lastName.Trim().ToLower());
            buffer.Append(firstName.Trim().ToLower()[0]);
            buffer.Append(state.Trim().ToLower());
            var input = buffer.ToString();
            var output = Fuze.Crypto.Hash.SHA1(input);
            return output;
        }
        #endregion

    }
}
