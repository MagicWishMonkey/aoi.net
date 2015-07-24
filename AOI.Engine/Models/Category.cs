using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOI.Models {
    public class Category : Model {
        #region -------- CONSTRUCTOR & VARIABLES --------
        public Category() : base() { }
        public Category(dynamic parameters){
            this.Bind(parameters);
        }
        #endregion


        public override IRepository Repository {
            get { return (IRepository)new Categories(); }
        }

        public override int ID { get; set; }
        public int ParentID { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }

        public Category Child(string name, bool createIfMissing = false) {
            var children = this.Children;
            foreach(var category in children){
                if(category.Label.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return category;
            }
            if (createIfMissing == true)
                return this.Create(name);
            return null;
        }

        public List<Category> Children {
            get {
                var children = ((Categories)this.Repository).Where(new { ParentID = this.ID });
                //var children = (from category in list select (Category)category).ToList<Category>();
                return children;
            }
        }

        public void Update(dynamic args){
            Type argType = args.GetType();
            Type type = this.GetType();
            foreach (System.Reflection.PropertyInfo srcProperty in argType.GetProperties()) {
                var key = srcProperty.Name;
                var val = srcProperty.GetValue(args);
                var dstProperty = type.GetProperty(key);
                dstProperty.SetValue(this, val);
            }
            //this.Repository.Update(this);
        }

        public Category Create(string label) {
            var child = new Category();
            child.Label = label;
            child.ParentID = this.ID;
            return ((Categories)this.Repository).Add(child);
            //throw new NotImplementedException();
        }
    }
}
