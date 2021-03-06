﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOI.Data;
using AOI.Models;

namespace AOI {
    public class Categories : AbstractRepository<Category> {
        public override string SelectQuery { get { return "SELECT * FROM Categories;"; } }

        //#region -------- PUBLIC - Fetch --------
        ////public List<IModel> Fetch() {
        ////    return this.Select() as List<IModel>;
        ////}
        

        ////public List<Category> Fetch() {
        ////    var sql = "SELECT * FROM Categories;";
        ////    return this.Select(sql);
        ////}

        //public List<Category> Fetch(dynamic vals) {
        //    //var sql = "SELECT * FROM Categories WHERE ParentID=@ParentID;";
        //    var sql = "SELECT * FROM Categories;";
        //    var query = new Query(sql);

        //        Type type = vals.GetType();
        //        foreach (System.Reflection.PropertyInfo pi in type.GetProperties()) {
        //            var key = pi.Name;
        //            var val = pi.GetValue(vals);
        //            query.Set(key, val);
        //        }
        //        return null;
        //    //return this.Select(query);
        //}
        //#endregion

        #region -------- PUBLIC - SaveAdd/Update --------
        public override Category Add(dynamic properties) {
            var category = new Category();
            Type modelType = category.GetType();
            Type propertyType = properties.GetType();
            foreach (System.Reflection.PropertyInfo pi in propertyType.GetProperties()) {
                var key = pi.Name;
                var val = pi.GetValue(properties);
                var mpi = modelType.GetProperty(key);
                var canWrite = mpi.CanWrite;
                mpi.SetValue(category, val, null);
            }

            return this.Add(category);
        }

        public override Category Add(Category category) {
            if(category.ParentID > 0){
                var query = new Query(
                    "INSERT INTO categories (Label, Description, ParentID) VALUES (@Label, @Description, @ParentID); SELECT CAST(SCOPE_IDENTITY() AS int);"
                )
                .Set("Label", category.Label)
                .Set("Description", category.Description)
                .Set("ParentID", category.ParentID);

                var id = this.db.Insert(query);
                category.ID = id;
                
            } else {
                var query = new Query(
                    "INSERT INTO categories (Label, Description) VALUES (@Label, @Description); SELECT CAST(SCOPE_IDENTITY() AS int);"
                )
                .Set("Label", category.Label)
                .Set("Description", category.Description);
                var id = this.db.Insert(query);
                category.ID = id;
            }
            return category;
        }


        public override Category Update(Category category) {
            var query = new Query(
                    "UPDATE Categories SET Label=@label, Description=@Description WHERE ID=@ID;"
                )
                .Set("ID", category.ID)
                .Set("Label", category.Label)
                .Set("Description", category.Description);
            this.db.Update(query);
            return category;
        }
        #endregion

        //#region -------- PUBLIC - Delete --------
        //public override T Delete(T entity) {
        //    throw new NotImplementedException("This method is not implemented.");
        //}
        //#endregion
    }
}
