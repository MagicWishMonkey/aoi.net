using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
    [Serializable]
    public class DatabaseException : System.Data.DataException{
        #region -------- CONSTRUCTOR/VARIABLES --------
        public DatabaseException(string msg):base(msg) { }
        public DatabaseException(string msg, Exception inner) : base(msg, inner) { }

        public DatabaseException(Query query, string msg)
            : base(Merge(msg, query)) {
                this.Query = query;
        }
        public DatabaseException(Query query, string msg, Exception inner)
            : base(Merge(msg, query), inner) {
                this.Query = query;
        }

        public DatabaseException(string databaseName, Query query, string msg)
            : base(Merge(msg, query, databaseName)) {
                this.Query = query;
        }
        public DatabaseException(string databaseName, Query query, string msg, Exception inner)
            : base(Merge(msg, query, databaseName), inner) {
                this.Query = query;
        }
        public DatabaseException(string databaseName, string msg)
            : base(Merge(msg, databaseName)) {

        }
        public DatabaseException(string databaseName, string msg, Exception inner)
            : base(Merge(msg, databaseName), inner) {

        }
        #endregion

        private static string Merge(string msg, Query query, string databaseName = null) {
            var buffer = new StringBuilder(msg);
            Append(buffer, query, databaseName);
            return buffer.ToString();
        }

        private static string Merge(string msg, string databaseName) {
            var buffer = new StringBuilder(msg);
            Append(buffer, null, databaseName);
            return buffer.ToString();
        }

        private static void Append(StringBuilder buffer, Query query = null, string databaseName = null) {
            buffer.AppendLine();
            buffer.AppendLine("-------------------------------------------------------------");
            if(!String.IsNullOrEmpty(databaseName))
                buffer.AppendLine("Database: " + databaseName);
            if (query != null) {
                buffer.AppendLine(query.Sql);
                var parameters = query.Parameters;
                foreach (var parameter in parameters) {
                    if (parameter.Direction == System.Data.ParameterDirection.Input) {
                        buffer.AppendLine(" @" + parameter.Name + " = " + Convert.ToString(parameter.Value));
                    }
                }


                foreach (var parameter in parameters) {
                    if (parameter.Direction != System.Data.ParameterDirection.Input) {
                        buffer.AppendLine(" [" + parameter.Direction.ToString() + "] @" + parameter.Name + " = " + Convert.ToString(parameter.Value));
                    }
                }
                buffer.AppendLine();
            }
        }


        #region -------- PROPERTIES --------
        public Query Query {
            get;
            private set;
        }
        #endregion
    }
}
