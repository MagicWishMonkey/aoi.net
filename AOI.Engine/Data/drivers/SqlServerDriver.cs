using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
    public class SqlServerDriver : DatabaseDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public SqlServerDriver()
            : base("SqlServer", System.Data.SqlClient.SqlClientFactory.Instance) {
        }
        #endregion


        #region -------- PUBLIC OVERRIDE - CreateParameter --------
        public override DbParameter CreateParameter(QueryParameter parameter) {
            var param = base.CreateParameter(parameter) as SqlParameter;
            param.ParameterName = "@" + param.ParameterName;
            return param;
        }
        #endregion
    }
}
