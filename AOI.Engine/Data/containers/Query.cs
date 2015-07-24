using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
     [Serializable]
    public class Query {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public Query(string sql) {
            Contract.Requires(!String.IsNullOrEmpty(sql));
            this.Sql = sql;
            this.Parameters = new List<QueryParameter>(5);

            if (sql.Trim().IndexOf(" ") < 0) {///NO SPACES IN QUERY = STORED PROCEDURE CALL
                this.Type = CommandType.StoredProcedure;
            } else {
                this.Type = CommandType.Text;
                this.Sql = FilterSqlComments(sql);
            }
        }
        #endregion

        #region -------- PUBLIC - Contains --------
        public bool Contains(string parameterName) {
            Contract.Requires(!String.IsNullOrEmpty(parameterName));
            return this.Parameters.Exists(x => x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region -------- PUBLIC - Get --------
        public QueryParameter Get(string parameterName) {
            Contract.Requires(!String.IsNullOrEmpty(parameterName));
            return this.Parameters.Where(x => x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        #endregion

        #region -------- PUBLIC - Clone --------
        public virtual Query Clone() {
            var query = new Query(this.Sql);
            //query.Parameters.AddRange(this.Parameters);
            return query;
        }
        #endregion

        #region -------- PUBLIC OVERRIDE - ToString --------
        public override string ToString() {
            return this.Sql;
        }
        #endregion

        #region -------- PUBLIC - Set/SetOut --------
        public Query Set(string parameterName) {
            return this.Set(new QueryParameter(parameterName));
        } 
        public Query Set(string parameterName, ParameterDirection direction) {
            return this.Set(new QueryParameter(parameterName, direction));
        }
        public Query Set(string parameterName, object value) {
            return this.Set(new QueryParameter(parameterName, value));
        }
        public Query Set(string parameterName, object value, ParameterDirection direction) {
            return this.Set(new QueryParameter(parameterName, value, direction));
        }

        // /// <summary>
        // /// Set the parameter name/value, this override is necessary to keep the runtime from mistakenly
        // /// assuming that an integer value of 1/2/3/6 is a ParameterDirection type cast value. Grrrrrr.
        // /// </summary>
        //public Query Set(string parameterName, int value) {
        //    return this.Set(new QueryParameter(parameterName, value));
        //}
     
        public Query Set(QueryParameter parameter) {
            Contract.Requires(parameter != null);
            Contract.Requires(!this.Contains(parameter.Name));

            var existing = this.Get(parameter.Name);
            if(existing != null){//duplicate, overwrite
                this.Parameters.Remove(existing);
            }

            this.Parameters.Add(parameter);
            return this;
        }


        public Query Out(string parameterName) {
            return this.Set(parameterName, ParameterDirection.Output);
        }
        public Query Out(string parameterName, object value) {
            return this.Set(new QueryParameter(parameterName, value, ParameterDirection.Output));
        }
        public Query Out(QueryParameter parameter) {
            parameter.Direction = ParameterDirection.Output;
            return this.Set(parameter);
        }
        #endregion


        ////public Query SetOutput(string parameterName) {
        ////    return this.Set(new QueryParameter(parameterName));
        ////}
        ////public Query SetOutput(string parameterName, ParameterDirection direction) {
        ////    return this.Set(new QueryParameter(parameterName, direction));
        ////}
        ////public Query SetOutput(string parameterName, object value) {
        ////    return this.Set(new QueryParameter(parameterName, value));
        ////}
        ////public Query SetOutput(string parameterName, object value, ParameterDirection direction) {
        ////    return this.Set(new QueryParameter(parameterName, value, direction));
        ////}
        ////public Query SetOutput(QueryParameter parameter) {
        ////    Contract.Requires(parameter != null);
        ////    Contract.Requires(!this.Contains(parameter.Name));
        ////    this.Parameters.Add(parameter);
        ////    return this;
        ////}

        //public BoundQuery Resolve() {
        //    return (BoundQuery)this;
        //}


        #region -------- PUBLIC STATIC - Create --------
        public static Query Create() {
            return new Query("");
        }
        public static Query Create(string sql) {
            return new Query(sql);
        }
        #endregion
        #region -------- PRIVATE STATIC - FilterSqlComments --------
        private static string FilterSqlComments(string sql) {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"--.+$", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);
            sql = regex.Replace(sql, " ");
            return sql;
        }
        #endregion

        #region -------- PROPERTIES --------
        public string Sql {
            get;
            set;
        }

        public List<QueryParameter> Parameters {
            get;
            private set;
        }

        public List<QueryParameter> Outputs {
            get {
                return this.Parameters.Where(x => x.Direction != ParameterDirection.Input).ToList();
            }
        }

        public List<QueryParameter> Inputs {
            get {
                return this.Parameters.Where(x => x.Direction == ParameterDirection.Input).ToList();
            }
        }

        public bool HasOutputs {
            get { return (this.Outputs.Count > 0) ? true : false; }
        }

        public CommandType Type {
            get;
            set;
        }
        #endregion
    }
}
