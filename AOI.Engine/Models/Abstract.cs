using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Models {
    public interface IModel {
        IRepository Repository { get; }
        int ID { get; set; }
        Model Save();
    }

    public abstract class Model : IModel {
        public Model() { }
        public Model(dynamic parameters) {
            this.Bind(parameters);
        }


        public void Bind(dynamic parameters) {
            Type modelType = this.GetType();
            Type propertyType = parameters.GetType();
            foreach (System.Reflection.PropertyInfo pi in propertyType.GetProperties()) {
                var key = pi.Name;
                var val = pi.GetValue(parameters);
                var mpi = modelType.GetProperty(key);
                mpi.SetValue(this, val, null);
            }
        }

        public abstract IRepository Repository { get; }
        public abstract int ID { get; set; }

        public Model Save() {
            var model = (IModel)this.Repository.Save(this);
            return (Model)model;
        }

        public override string ToString() {
            var name = this.GetType().Name;
            return name + "#" + this.ID.ToString();
        }
    }

    //public interface IEntity<T> {
    //    int ID { get; set; }
    //    //AbstractRepository<T> Repository { get; set; }
    //}
    //public abstract class BaseEntity {
    //    public virtual int ID { get; set; }

    //    //public AbstractRepository<BaseEntity> Repository {
    //    //    get;
    //    //    set;
    //    //}
        
    //}
    //public abstract class Entity<T> : BaseEntity, IEntity<T> {
    //    //public Entity(){
    //    //    this.ID = -1;
    //    //}

    //    public virtual int ID { get; set; }

    //    //public AbstractRepository<Entity<T>> Repository {
    //    //    get;
    //    //    set;
    //    //}
    //}
}
