using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Npgsql;

namespace AOI.Data {
    public class PostgresDriver : DatabaseDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public PostgresDriver()
            : base("Postgres", Npgsql.NpgsqlFactory.Instance) {
        }
        #endregion

        //#region -------- PUBLIC OVERRIDE - CreateParameter --------
        //public override DbParameter CreateParameter(QueryParameter parameter) {
        //    var param = base.CreateParameter(parameter) as SqlParameter;
        //    param.ParameterName = "@" + param.ParameterName;
        //    return param;
        //}
        //#endregion
    }
}
