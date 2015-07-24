using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
    public class OdbcDriver : DatabaseDriver {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public OdbcDriver()
            : base("Odbc", System.Data.Odbc.OdbcFactory.Instance) {
        }
        #endregion

        
    }
}
