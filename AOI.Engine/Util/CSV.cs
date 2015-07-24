using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Util {
    public class CSV {
        #region -------- PARSE --------
        public static Dictionary<string, string> Parse(string csvFile) {
            return Parse(csvFile, ',');
        }
        public static Dictionary<string, string> Parse(string csvFile, char delimiter) {
            if (IO.FileUtil.Exists(csvFile))
                csvFile = IO.File.ReadText(csvFile);

            if (delimiter == null)
                delimiter = ',';

            Dictionary<string, string> table = null;

            string[] columns = null;
            int columnCount = 0;
            var reader = new System.IO.StringReader(csvFile);
            string line;
            while ((line = reader.ReadLine()) != null) {
                var row = line.Split(delimiter);
                if (table == null) {
                    table = new Dictionary<string, string>();
                    columns = row;
                    columnCount = columns.Length;
                    break;
                }

                for (var x = 0; x < columnCount; x++) {
                    var column = columns[x];
                    try {
                        var val = row[x];
                        if (val == "")
                            val = null;

                        table[column] = val;
                    } catch {
                        table[column] = null;
                    }
                }
            }
            return table;
        }
        #endregion

    }
}
