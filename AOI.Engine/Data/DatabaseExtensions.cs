using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
    public static class DatabaseExtensions {
        #region -------- ReadKeys --------
        public static List<int> ReadKeys(this INullableReader r) {
            Contract.Requires(r != null);

            var list = new List<int>();
            return r.AppendKeys(list);
        }
        #endregion

        #region -------- AppendKeys --------
        public static List<int> AppendKeys(this INullableReader r, List<int> list) {
            Contract.Requires(r != null);
            Contract.Requires(list != null);
           
            while (true) {
                var id = r.GetNullableInt32(0);
                if (id != null)
                    list.Add((int)id);
                if (!r.Read())
                    break;
            }
            return list;
        }
        #endregion


        #region -------- ReadValues --------
        public static List<string> ReadValues(this INullableReader r) {
            Contract.Requires(r != null);

            var list = new List<string>();
            return r.AppendValues(list);
        }
        #endregion

        #region -------- AppendValues --------
        public static List<string> AppendValues(this INullableReader r, List<string> list) {
            Contract.Requires(r != null);
            Contract.Requires(list != null);

            while (true) {
                var val = r.GetString(0);
                list.Add(val);
                if (!r.Read())
                    break;
            }
            return list;
        }
        #endregion


        #region -------- Destroy --------
        public static void Destroy(this IDbConnection connection) {
            if (connection == null)
                return;
            try {
                connection.Close();
            } catch { }
            try {
                connection.Dispose();
            } catch { }
        }

        public static void Destroy(this IDbCommand command) {
            if (command == null)
                return;

            try {
                command.Dispose();
            } catch { }

            command.Connection.Destroy();
        }

        public static void Destroy(this DbDataAdapter adapter) {
            if (adapter == null)
                return;

            try {
                adapter.Dispose();
            } catch { }

        }


        public static void Destroy(this RecordReader reader) {
            if (reader == null)
                return;
            try {
                reader.Close();
            } catch { }
            try {
                reader.Dispose();
            } catch { }
        }
        #endregion

        #region -------- TryClose --------
        public static void TryClose(this IDbConnection connection) {
            if (connection == null)
                return;
            try {
                connection.Close();
            } catch { }
        }

        public static void TryDispose(this IDbConnection connection) {
            if (connection == null)
                return;
            try {
                connection.Dispose();
            } catch { }
        }
        #endregion

        #region -------- CheckAvailability --------
        /// <summary>
        /// Verify that the database is online and available by opening and closing a connection to it.
        /// Return true if the connection is successful, false if it times out or an exception is thrown.
        /// </summary>
        /// <returns>Return true if the connection is successful, false if it times out or an exception is thrown.</returns>
        public static bool CheckAvailability(this IDbConnection connection) {
            Contract.Requires(connection != null);
            try {
                connection.Close();
                return true;
            } catch (Exception ex) {
                Log.Warn("Database.Connection.CheckAvailability Error-> " + ex.Message);
                return false;
            } finally {
                connection.TryClose();
                connection.TryDispose();
            }
        }
        #endregion
    }
}
