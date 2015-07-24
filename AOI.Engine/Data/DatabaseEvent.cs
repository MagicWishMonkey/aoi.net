//using System;
//using System.Linq;
//using System.Data;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//namespace AOI.Data {
//    public class DatabaseEvent {
//        #region -------- CONSTRUCTOR/VARIABLES --------
//        public DatabaseEvent(Query query, TimeSpan executionTime, Exception error) {
//            this.Query = query;
//            this.ExecutionTime = executionTime;
//            this.Error = error;
//            this.Timestamp = DateTime.Now;
//        }
//        public DatabaseEvent(Query query, TimeSpan executionTime) {
//            this.Query = query;
//            this.ExecutionTime = executionTime;
//            this.Timestamp = DateTime.Now;
//        }
//        #endregion


//        #region -------- PROPERTIES --------
//        public Query Query {
//            get;
//            private set;
//        }

//        public DateTime Timestamp {
//            get;
//            private set;
//        }

//        public TimeSpan ExecutionTime {
//            get;
//            private set;
//        }

//        public Exception Error {
//            get;
//            private set;
//        }
//        #endregion
//    }
//}
