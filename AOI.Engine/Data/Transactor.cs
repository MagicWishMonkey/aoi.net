using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
    internal sealed class Transactor : IDisposable{
        #region -------- CONSTRUCTOR/VARIABLES --------
        public Transactor(Database database) {
            this.Source = database;
            this.Connection = database.Driver.CreateConnection(database.ConnectionString);
            this.StartTS = System.Environment.TickCount;
        }
        #endregion
      

        #region -------- PUBLIC - Select/SelectTable --------
        public DataSet Select(Query query) {
            var cmd = this.NewCommand(query);
            var adapter = this.Source.Driver.CreateAdapter(cmd);
            DataSet ds = new DataSet();
            try {
                this.Connection.Open();
                
                adapter.Fill(ds);
                
                //this.Bubble(query);
            } catch (Exception ex) {
                ex = new DatabaseException(this.DatabaseName, query, "Select Error-> " + ex.Message, ex);
                //Self.Fork(delegate(object evt) {
                //    this.Source.Bubble((DatabaseEvent)evt);
                //}, new DatabaseEvent(query, TimeSpan.FromMilliseconds(this.ElapsedTS), ex));

                throw ex;
            } finally {
                cmd.Dispose();
                this.Connection.TryClose();
            }
            return ds;
        }

        public DataTable SelectTable(Query query) {
            DataSet ds = this.Select(query);
            DataTable table = (ds.Tables.Count > 0) ? ds.Tables[0] : null;

            return table;
        }
        #endregion


        #region -------- PUBLIC - Execute --------
        public object Execute(Query query) {
            var cmd = this.NewCommand(query);
            object id = null;

            try {
                this.Connection.Open();
                //cmd.ExecuteNonQuery();
                id = cmd.ExecuteScalar();
                //this.Bubble(query);
            } catch (Exception ex) {
                ex = new DatabaseException(this.DatabaseName, query, "Execute Error-> " + ex.Message, ex);

                //Self.Fork(delegate(object evt) {
                //    this.Source.Bubble((DatabaseEvent)evt);
                //}, new DatabaseEvent(query, TimeSpan.FromMilliseconds(this.ElapsedTS), ex));

                throw ex;
            } finally {
                cmd.Dispose();
                this.Connection.TryClose();
            }


            if (!query.HasOutputs)
                return id;

            var outputs = query.Outputs;
            if (outputs.Count == 1)
                return outputs[0].Value;

            return outputs.Select(x => x.Value).ToList();
        }
        #endregion

        #region -------- PUBLIC - ExecuteScalar --------
        public object ExecuteScalar(Query query) {
            var cmd = this.NewCommand(query);
            Object o = null;
            try {
                this.Connection.Open();
                o = cmd.ExecuteScalar();
                //this.Bubble(query);
            } catch (Exception ex) {
                //ex = new DataException("Database.ExecuteScalar Error-> " + ex.Message, ex.InnerException);
                ex = new DatabaseException(this.DatabaseName, query, "ExecuteScalar Error-> " + ex.Message, ex);

                //Self.Fork(delegate(object evt) {
                //    this.Source.Bubble((DatabaseEvent)evt);
                //}, new DatabaseEvent(query, TimeSpan.FromMilliseconds(this.ElapsedTS), ex));

                throw ex;
            } finally {
                cmd.Dispose();
                this.Connection.TryClose();
            }

            return o;
        }
        #endregion

        #region -------- PUBLIC - ExecuteReader --------
        public void ExecuteReader(Query query, Action<RecordReader> recordReader) {
            var cmd = this.NewCommand(query);
            try {
                this.Connection.Open();

                using (RecordReader reader = new RecordReader(cmd.ExecuteReader() as DbDataReader)) {
                    if (reader.HasRows) {
                        while (reader.Read()) {
                            recordReader(reader);
                        }
                    }

                    reader.Destroy();
                }

                //this.Bubble(query);
            } catch (Exception ex) {
                //ex = new DataException("Database.ExecuteReader Error-> " + ex.Message, ex.InnerException);
                ex = new DatabaseException(this.DatabaseName, query, "ExecuteReader Error-> " + ex.Message, ex);
                //Self.Fork(delegate(object evt) {
                //    this.Source.Bubble((DatabaseEvent)evt);
                //}, new DatabaseEvent(query, TimeSpan.FromMilliseconds(this.ElapsedTS), ex));

                throw ex;
            } finally {
                cmd.Dispose();
                this.Connection.TryClose();
            }
        }
        #endregion


        #region -------- PUBLIC - ExecuteBatch --------
        public void ExecuteBatch(params Query[] queries) {
            //throw new NotImplementedException();
            if (queries.Length == 0)
                return;

            //var first = queries[0];
            //var cmd = this.NewCommand(first);
            //var connection = command.Connection;
            try {
                this.Connection.Open();
                foreach (var query in queries) {
                    var cmd = this.NewCommand(query);
                    try {
                        cmd.ExecuteNonQuery();
                        //this.Bubble(query);
                    } catch (Exception ex) {
                        ex = new DatabaseException(this.DatabaseName, query, "Execute Error-> " + ex.Message, ex);
                        //this.Bubble(query, ex);
                    } finally {
                        cmd.Dispose();
                    }
                }
            } catch (Exception ex) {
                ex = new DatabaseException(this.DatabaseName, "Execute Error-> " + ex.Message, ex);
                throw ex;
            } finally {
                this.Connection.TryClose();
            }
        }
        #endregion



        #region -------- PRIVATE - NewCommand --------
        private DbCommand NewCommand(Query query) {
            var cmd = this.Source.Driver.CreateCommand(this.Connection);
            this.Source.Driver.Bind(cmd, query);
            return cmd;
        }
        #endregion


        //#region -------- PRIVATE - Bubble --------
        //private void Bubble(Query query) {
        //    this.Source.Bubble(new DatabaseEvent(query, TimeSpan.FromMilliseconds(this.ElapsedTS)));
        //}

        //private void Bubble(Query query, Exception error) {
        //    this.Source.Bubble(new DatabaseEvent(query, TimeSpan.FromMilliseconds(this.ElapsedTS), error));
        //}
        //#endregion


        #region -------- PROPERTIES --------
        private string DatabaseName {
            get { return this.Source.Name; }
        }
        public Database Source {
            get;
            private set;
        }

        private DbConnection Connection {
            get;
            set;
        }

        public int StartTS {
            get;
            private set;
        }

        private int ElapsedTS {
            get {
                return System.Environment.TickCount - this.StartTS;
            }
        }
        #endregion


        #region -------- Dispose --------
        private bool _disposed = false;
        public void Dispose() {
            if (this._disposed)
                return;
            this._disposed = true;
            this.Connection.Destroy();
            GC.SuppressFinalize(this);
        }
        ~Transactor() { this.Dispose(); }
        #endregion
    }
}
