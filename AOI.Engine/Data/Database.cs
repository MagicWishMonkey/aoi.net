using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Configuration;

namespace AOI.Data {
    public class Database {
        #region -------- CONSTRUCTOR/VARIABLES --------
        //public event DatabaseAction OnQuery;
        //public event DatabaseAction OnQueryError;
        private Database(string databaseName, string connectionString, DatabaseDriver driver) {
            Contract.Requires(!String.IsNullOrEmpty(databaseName));
            Contract.Requires(!String.IsNullOrEmpty(connectionString));
            Contract.Requires(driver != null);
            this.Name = databaseName;
            this.ConnectionString = connectionString;
            this.Driver = driver;
            this.UseTransactions = false;            
        }
        #endregion


        #region -------- PUBLIC - Select/SelectTable --------
        public DataSet Select(string sql) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.Select(new Query(sql));
        }

        public DataSet Select(Query query) {
            Contract.Requires(query != null);
            using (var txn = new Transactor(this)) {
                return txn.Select(query);
            }
        }

        public DataTable SelectTable(string sql) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.SelectTable(new Query(sql));
        }

        public DataTable SelectTable(Query query) {
            Contract.Requires(query != null);
            using (var txn = new Transactor(this)) {
                return txn.SelectTable(query);
            }
        }
        #endregion

        #region -------- PUBLIC - Execute/ExecuteScalar/Update/Insert --------
        public object Execute(string sql) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.Execute(new Query(sql));
        }
        public object Execute(Query query) {
            Contract.Requires(query != null);

            using (var txn = new Transactor(this)) {
                //if (this.UseTransactions) {
                //    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.Required)) {
                //        var o = txn.Execute(query);
                //        scope.Complete();
                //        return o;
                //    }
                //}

                return txn.Execute(query);
            }
        }

        public void Update(string sql) {
            this.Execute(sql);
        }
        public void Update(Query query) {
            this.Execute(query);
        }
        public int Insert(string sql) {
            var o = this.Execute(sql);
            return (int)o;
        }
        public int Insert(Query query) {
            var o = this.Execute(query);
            return (int)o;
        }

        public object ExecuteScalar(string sql) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.ExecuteScalar(new Query(sql));
        }
        public object ExecuteScalar(Query query) {
            Contract.Requires(query != null);

            using (var txn = new Transactor(this)) {
                return txn.ExecuteScalar(query);
            }
        }
        #endregion

        #region -------- PUBLIC - SelectRecords --------
        public List<Record> SelectRecords(string sql) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.SelectRecords(new Query(sql));
        }
        public List<Record> SelectRecords(Query query) {
            Contract.Requires(query != null);
            List<Record> list = new List<Record>();
            List<string> fields = null;
            Record record = null;
            this.ExecuteReader(query, x => {
                if (fields == null) {
                    fields = x.Columns;
                }

                record = (record == null) ? new Record() : record.CloneSchema();
                for (int i = 0; i < fields.Count; i++) {
                    var field = fields[i];
                    var val = x.GetValue(field);
                    record[field] = val;
                }
                list.Add(record);
            });

            return list;
        }

        public List<T> SelectRecords<T>(string sql) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.SelectRecords<T>(new Query(sql));
        }
        public List<T> SelectRecords<T>(Query query) {
            Contract.Requires(query != null);
            var list = new List<T>();

            List<string> fields = null;
            var type = typeof(T);

            var fieldMapper = new Dictionary<string, string>();
            foreach (var field in type.GetProperties()) {
                var name = field.Name;
                fieldMapper[name.ToLower()] = name;
            }

            foreach (var field in type.GetFields()) {
                var name = field.Name;
                fieldMapper[name.ToLower()] = name;
            }

            this.ExecuteReader(query, x => {
                if (fields == null) {
                    fields = x.Columns;
                }

                var obj = Activator.CreateInstance(type);
                for (int i = 0; i < fields.Count; i++) {
                    var fieldName = fields[i];
                    var value = x.GetValue(fieldName);
                    if (value == DBNull.Value)
                        continue;
                    //fieldName = fieldMapper[fieldName];

                    var property = type.GetProperty(fieldName);
                    if (property != null) {
                        property.SetValue(obj, value);
                        continue;
                    }

                    var field = type.GetField(fieldName);
                    if (field != null) {
                        field.SetValue(obj, value);
                    }
                }
                list.Add((T)obj);
            });

            return list;
        }
        #endregion

        #region -------- PUBLIC - ExecuteReader --------
        public void ExecuteReader(string sql, Action<RecordReader> recordReader) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            this.ExecuteReader(new Query(sql), recordReader);
        }
        public void ExecuteReader(Query query, Action<RecordReader> recordReader) {
            Contract.Requires(query != null);
            Contract.Requires(recordReader != null);
            using (var txn = new Transactor(this)) {
                txn.ExecuteReader(query, recordReader);
            }
        }
        #endregion

        #region -------- PUBLIC - ExecuteBatch --------
        public void ExecuteBatch(params Query[] queries) {
            Contract.Requires(queries != null);
            using (var txn = new Transactor(this)) {
                txn.ExecuteBatch(queries);
            }
        }

        public void ExecuteBatch(List<Query> queries) {
            Contract.Requires(queries != null);
            using (var txn = new Transactor(this)) {
                txn.ExecuteBatch(queries.ToArray());
            }
        }


        public void ExecuteBatch(List<string> sqlStatements) {
            Contract.Requires(sqlStatements != null);
            this.ExecuteBatch(sqlStatements.ToArray());
        }
        public void ExecuteBatch(params string[] sqlStatements) {
            Contract.Requires(sqlStatements != null);
            var queries = new Query[sqlStatements.Length];
            for (int i = 0; i < sqlStatements.Length; i++) {
                queries[i] = new Query(sqlStatements[i]);
            }
            this.ExecuteBatch(queries);
        }
        #endregion

        #region -------- PUBLIC - SelectField<T>/SelectList<T> --------
        public T SelectField<T>(string sql, string fieldName = null) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.SelectField<T>(new Query(sql), fieldName);
        }
        public T SelectField<T>(Query query, string fieldName = null) {
            Contract.Requires(query != null);
            Object val = null;
            using (var txn = new Transactor(this)) {
                txn.ExecuteReader(query, (x) => {
                    if (val != null) {
                        throw new DataException("The query has returned more than one record!");
                    }

                    if(String.IsNullOrEmpty(fieldName))
                        val = x.GetValue(0);
                    else
                        val = x.GetValue(fieldName);
                });
            }
            return (T)val;
        }

        public List<T> SelectList<T>(string sql, string fieldName = null) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            return this.SelectList<T>(new Query(sql), fieldName);
        }
        public List<T> SelectList<T>(Query query, string fieldName = null) {
            Contract.Requires(query != null);
            List<T> list = new List<T>();
            int ix = -1;
            using (var txn = new Transactor(this)) {
                txn.ExecuteReader(query, (x) => {
                    if (ix == -1) {
                        if (!String.IsNullOrEmpty(fieldName))
                            ix = x.GetOrdinal(fieldName);
                        else
                            ix = 0;
                    }
                    var val = x.GetValue(ix);
                    list.Add((T)val);
                });
            }
            return list;
        }
         #endregion


        //#region -------- PUBLIC - CheckAvailability --------
        ///// <summary>
        ///// Verify that the database is online and available by opening and closing a connection to it.
        ///// Return true if the connection is successful, false if it times out or an exception is thrown.
        ///// </summary>
        ///// <returns>Return true if the connection is successful, false if it times out or an exception is thrown.</returns>
        //public bool CheckAvailability() {
        //    var conn = this.Driver.CreateConnection(this.ConnectionString);
        //    return conn.CheckAvailability();
        //}
        //#endregion


        //#region -------- PUBLIC - CreateQuery --------
        //public BoundQuery CreateQuery(string sql) {
        //    var qry = new BoundQuery(this, sql);
        //    return qry;
        //}
        //#endregion


        //#region -------- INTERNAL - Bubble --------
        //internal void Bubble(DatabaseEvent evt) {
        //    if (evt.Error == null) {
        //        Console.WriteLine("EXEC " + evt.Query.Sql + " -- " + evt.ExecutionTime.Milliseconds + "ms");
        //        if (this.OnQuery != null)
        //            this.OnQuery(evt);
        //    } else {
        //        if (this.OnQueryError != null)
        //            this.OnQueryError(evt);
        //    }
        //}
        //#endregion

        public static Database Open(string name) {
            var cfg = ConfigurationManager.ConnectionStrings[name];
            var connStr = cfg.ConnectionString;
            var provider = cfg.ProviderName;

            var type = provider.ToLower();
            if (type.EndsWith("sqlclient"))
                provider = "SqlServer";
            else if (provider.ToLower() == "npgsql" || provider.ToLower().StartsWith("postgres"))
                provider = "Postgres";

            //var cfg = ConfigurationSettings.AppSettings[name];
            return CreateCustom(name, provider, connStr);

        }

        #region -------- PUBLIC STATIC - Resolve/Create --------
        public static Database Resolve(string name, string databaseType, string connectionString) {
            var driver = Drivers.Get(databaseType);
            return new Database(name, connectionString, driver);
        }
        public static Database CreateCustom(string name, string databaseType, string connectionString) {
            return Resolve(name, databaseType, connectionString);
        }
        public static Database CreateSqlServer(string name, string connectionString) {
            return Resolve(name, "SqlServer",  connectionString);
        }
        public static Database CreateOdbc(string name, string connectionString) {
            return Resolve(name, "Odbc", connectionString);
        }
        public static Database CreateOleDb(string name, string connectionString) {
            return Resolve(name, "OleDb", connectionString);
        }
        public static Database CreatePostgres(string name, string connectionString) {
            return Resolve(name, "Postgres", connectionString);
        }

        //public static Database Resolve(string databaseType, string connectionString) {
        //    var driver = Drivers.Get(databaseType);
        //    return new Database(("Database#"+NumberSequence.NextString), connectionString, driver);
        //}
        //public static Database CreateCustom(string databaseType, string connectionString) {
        //    return Resolve(("Database#" + NumberSequence.NextString), databaseType, connectionString);
        //}
        //public static Database CreateSqlServer(string connectionString) {
        //    return Resolve(("Database#" + NumberSequence.NextString), "SqlServer", connectionString);
        //}
        //public static Database CreateOdbc(string connectionString) {
        //    return Resolve(("Database#" + NumberSequence.NextString), "Odbc", connectionString);
        //}
        //public static Database CreateOleDb(string connectionString) {
        //    return Resolve(("Database#" + NumberSequence.NextString), "OleDb", connectionString);
        //}
        #endregion

        #region -------- PUBLIC OVERRIDE - ToString --------
        public override string ToString() {
            return this.Driver.Type + "@" + this.ConnectionString;
        }
        #endregion

        #region -------- PUBLIC - UpdateConnection --------
        public void UpdateConnection(string connectionString) {
            Contract.Requires(!String.IsNullOrEmpty(connectionString));
            this.ConnectionString = connectionString;
        }
        #endregion

        #region -------- PROPERTIES --------
        public string Name {
            get;
            private set;
        }

        public string ConnectionString {
            get;
            private set;
        }

        public DatabaseDriver Driver {
            get;
            private set;
        }

        public bool UseTransactions {
            get;
            set;
        }
        #endregion
    }
}
