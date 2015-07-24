using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AOI.Data;
using AOI.Models;

namespace AOI {
    public class Products : AbstractRepository<Product> {
        public override string SelectQuery { get { return "SELECT * FROM Products;"; } }

        //#region -------- PUBLIC - SaveAdd/Update --------
        //public override Category Add(dynamic properties) {
        //    var category = new Category();
        //    Type modelType = category.GetType();
        //    Type propertyType = properties.GetType();
        //    foreach (System.Reflection.PropertyInfo pi in propertyType.GetProperties()) {
        //        var key = pi.Name;
        //        var val = pi.GetValue(properties);
        //        var mpi = modelType.GetProperty(key);
        //        var canWrite = mpi.CanWrite;
        //        mpi.SetValue(category, val, null);
        //    }

        //    return this.Add(category);
        //}

        public override Product Add(Product product) {
            return product;
        }


        //public override Category Update(Category category) {
        //    var query = new Query(
        //            "UPDATE Categories SET Label=@label, Description=@Description WHERE ID=@ID;"
        //        )
        //        .Set("ID", category.ID)
        //        .Set("Label", category.Label)
        //        .Set("Description", category.Description);
        //    this.db.Update(query);
        //    return category;
        //}
        //#endregion

        //#region -------- PUBLIC - Delete --------
        //public override T Delete(T entity) {
        //    throw new NotImplementedException("This method is not implemented.");
        //}
        //#endregion
    }
}
