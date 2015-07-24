using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
    public class DatabaseDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public DatabaseDriver(string driverName, DbProviderFactory factory) {
            Contract.Requires(!String.IsNullOrEmpty(driverName));
            Contract.Requires(factory != null);

            this.Factory = factory;
            var name = factory.GetType().FullName;
            this.Type = name.Substring(0,name.LastIndexOf('.'));
            this.Name = driverName;
        }
        #endregion


        #region -------- PUBLIC VIRTUAL - CreateConnection/CreateAdapter/CreateParameter/CreateCommand --------
        public virtual DbConnection CreateConnection(string connectionString) {
            var conn = this.Factory.CreateConnection();
            conn.ConnectionString = connectionString;
            return conn;
        }

        public virtual DbDataAdapter CreateAdapter(DbCommand command) {
            var adapter = this.Factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }

        public virtual DbParameter CreateParameter(QueryParameter parameter) {
            var param = this.Factory.CreateParameter();
            param.ParameterName = parameter.Name;
            param.Direction = parameter.Direction;
            param.Value = (parameter.Value == null) ? DBNull.Value : parameter.Value;
            return param;
        }

        public virtual DbCommand CreateCommand(string connectionString) {
            var connection = this.CreateConnection(connectionString);
            return this.CreateCommand(connection);
        }

        public virtual DbCommand CreateCommand(DbConnection connection) {
            var command = this.Factory.CreateCommand();
            command.Connection = connection;
            return command;
        }
        #endregion


        #region -------- PUBLIC VIRTUAL - Bind --------
        public virtual void Bind(DbCommand cmd, Query query) {
            cmd.CommandType = query.Type;
            cmd.CommandText = query.Sql;
            cmd.CommandTimeout = 30;
            foreach (var param in query.Parameters) {
                var p = this.CreateParameter(param);
                //if(param.Direction == ParameterDirection.Output){
                //    p.Size = 16;
                //}
                cmd.Parameters.Add(p);
            }
        }
        #endregion

        #region -------- PROPERTIES --------
        public string Type {
            get;
            private set;
        }

        public string Name {
            get;
            private set;
        }

        public DbProviderFactory Factory {
            get;
            private set;
        }
        #endregion
    }
}
