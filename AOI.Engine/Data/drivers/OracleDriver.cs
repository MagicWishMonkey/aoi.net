//using System;
//using System.Linq;
//using System.Data;
//using System.Data.Common;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//using Oracle.DataAccess.Client;
//namespace AOI.Data {
//    public class OracleDriver : DatabaseDriver {
//        #region -------- CONSTRUCTOR/VARIABLES --------
//        public OracleDriver()
//            : base("Oracle", Oracle.DataAccess.Client.OracleClientFactory.Instance) {
//        }
//        #endregion

//        #region -------- PUBLIC OVERRIDE - CreateCommand/CreateParameter --------
//        public override DbCommand CreateCommand(DbConnection connection) {
//            var cmd = base.CreateCommand(connection);
//            ((OracleCommand)cmd).BindByName = true;
//            return cmd;
//        }

//        public override DbParameter CreateParameter(QueryParameter parameter) {
//            var param = base.CreateParameter(parameter) as OracleParameter;
//            if(parameter.TypeFlag > 27)
//                param.OracleDbType = (OracleDbType)parameter.TypeFlag;
//            return param;
//        }
//        #endregion
//    }
//}
