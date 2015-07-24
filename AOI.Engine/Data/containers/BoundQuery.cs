//using System;
//using System.Linq;
//using System.Data;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//namespace AOI.Data {
//    [Serializable]
//    public class BoundQuery : Query {
//        #region -------- CONSTRUCTOR/VARIABLES --------
//        public BoundQuery(string sql)
//            : base(sql) {
//            this.Username = Self.Username;
//        }
//        public BoundQuery(Database database, string sql)
//            : base(sql) {
//            this.Database = database;
//            this.Username = Self.Username;
//        }
//        #endregion


//        #region -------- OVERRIDES - Set/SetOutput/Clone --------
//        //public override Query Set(QueryParameter parameter) {
//        //    base.Set(parameter);
//        //    return this;
//        //}
//        //public override Query SetOutput(QueryParameter parameter) {
//        //    base.SetOutput(parameter);
//        //    return this;
//        //}
//        public override Query Clone() {
//            var query = new BoundQuery(this.Database, this.Sql);
//            return query;
//        }
//        #endregion


//        public T SelectField<T>(string fieldName = null) {
//            return this.Database.SelectField<T>(this, fieldName);
//        }
//        public object ExecuteScalar() {
//            return this.Database.ExecuteScalar(this);
//        }
//        public object Execute() {
//            if (BatchProcessor.Append(this))
//                return null;

//            return this.Database.Execute(this);
//        }
//        public void ExecuteReader(Action<RecordReader> reader) {
//            this.Database.ExecuteReader(this, reader);
//        }
//        public DataTable Select() {
//            return this.Database.SelectTable(this);
//        }

//        public void Bind(Database database) {
//            this.Database = database;
//        }

//        public BoundQuery Bind(string username) {
//            this.Username = username;
//            return this;
//        }

//        #region -------- PROPERTIES --------
//        public Database Database {
//            get;
//            private set;
//        }

//        public string Username {
//            get;
//            private set;
//        }
//        #endregion
//    }
//}
