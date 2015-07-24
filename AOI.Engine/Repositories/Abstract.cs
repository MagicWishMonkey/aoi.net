using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOI.Models;
using AOI.Data;

namespace AOI {
    public interface IRepository {
        List<IModel> Fetch();
        IModel Save(IModel model);
    }

    public abstract class AbstractRepository<T> : IRepository, IDisposable where T : IModel {//Entity<T> {
        #region -------- CONSTRUCTOR & VARIABLES --------
        internal Database db;
        public AbstractRepository() {
            this.db = Database.Open("DefaultConnection");
        }
        #endregion


        #region -------- PUBLIC - Fetch --------
        public List<IModel> Fetch() {
            throw new NotImplementedException("This method is not implemented.");
        }
        #endregion

        public abstract string SelectQuery { get; }

        public Query BuildSelectQuery(dynamic parameters = null) {
            var sql = this.SelectQuery;
            if (parameters == null)
                return new Query(sql);

            sql = sql.TrimEnd();
            if (sql[sql.Length - 1] == ';')
                sql = sql.Substring(0, sql.Length - 1);

            var buffer = new StringBuilder();
            buffer.Append(sql);


            var query = new Query(sql);
            var clauses = new List<string>();

            Type type = parameters.GetType();
            var properties = type.GetProperties();
            for (var i = 0; i < properties.Length; i++) {
                var pi = properties[i];
                var key = pi.Name;
                var val = pi.GetValue(parameters);
                if (val.GetType() == typeof(DBNull)) {
                    if (i == 0)
                        buffer.Append(" WHERE " + key + " IS NULL");
                    else
                        buffer.Append("AND " + key + " IS NULL");
                } else {
                    if (i == 0)
                        buffer.Append(" WHERE " + key + "=@" + key);
                    else
                        buffer.Append("AND " + key + "=@" + key);
                }
                query.Set(key, val);
            }
            buffer.Append(";");
            query.Sql = buffer.ToString();
            return query;
        }


        #region -------- PUBLIC - Select --------
        public List<T> Select() {
            var query = new Query(this.SelectQuery);
            return this.Select(query);
        }
        public List<T> Select(string sql) {
            var query = new Query(sql);
            return this.Select(query);
        }
        public List<T> Select(dynamic parameters) {
            var query = this.BuildSelectQuery(parameters);
            return this.Select(query);
        }
        public List<T> Select(Query query) {
            var lst = this.db.SelectRecords<T>(query);
            return lst;
        }

        public T Single(dynamic parameters) {
            var lst = this.Where(parameters);
            if (lst.Count == 0)
                return default(T);
            return lst[0];
        }
        public List<Category> Where(dynamic parameters) {
            var query = this.BuildSelectQuery(parameters);
            var lst = this.db.SelectRecords<Category>(query);
            return lst;
        }

        //public List<T> Select(string sql) {
        //    var lst = this.db.SelectRecords<T>(sql);
        //    //foreach (var e in lst) {
        //    //    e.Repository = this;
        //    //}
        //    return lst;
        //}
        //public List<T> Select(Query query) {
        //    var lst = this.db.SelectRecords<T>(query);
        //    //foreach (var e in lst) {
        //    //    e.Repository = this;
        //    //}
        //    return lst;
        //}
        #endregion

        #region -------- PUBLIC - Save/Add/Update --------
        public virtual IModel Save(IModel model) {
            if (model.ID == 0)
                return (IModel)this.Add((T)model);
            return (IModel)this.Update((T)model);
        }
        public virtual IModel Save(T entity) {
            if (entity.ID == 0)
                return this.Add(entity);
            return this.Update(entity);
        }

        public virtual T Add(dynamic properties) {
            throw new NotImplementedException("This method is not implemented.");
        }

        public virtual T Add(T entity) {
            throw new NotImplementedException("This method is not implemented.");
        }

        public virtual T Update(T entity) {
            throw new NotImplementedException("This method is not implemented.");
        }
        #endregion

        #region -------- PUBLIC - Delete --------
        public virtual T Delete(T entity) {
            throw new NotImplementedException("This method is not implemented.");
        }
        #endregion

        #region -------- PUBLIC - Dispose --------
        private bool _disposed = false;
        protected virtual void Dispose(bool disposing) {
            if (!this._disposed) {
                if (disposing) {
                    this.db = null;
                    //this.ctx.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
