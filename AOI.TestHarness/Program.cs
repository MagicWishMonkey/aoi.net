using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fuze;
using Fuze.Util;
using Fuze.Data;
using AOI.Data;


namespace AOI {
    class Program {
        static void Main(string[] args) {
            var osInfo = System.Environment.OSVersion;
            Settings.Load(Toolkit.CurrentDirectory.FindPrecedingFile("settings.json").FullPath);
            var pwd = Settings.Passcode;

            var dbcfg = Settings.Databases["default"];

            var cat1 = new Categories().Add(new {
                Label = "Wangs"
            });
            var cats = new Categories().Select();

            var parents = new Categories().Where(new { ParentID = DBNull.Value });
            var productData = Fuze.Util.IO.File.ReadText("C:\\Work\\art_of_old_india_products_complete.json");
            var products = Fuze.Util.Codecs.JSON.Decode<List<Dictionary<string, dynamic>>>(productData);
            foreach (var product in products) {
                if (product.ContainsKey("category") == true) {
                    var productCategory = ((string)product["category"]).ToUpper();
                    var parts = productCategory.Split('/');
                    var parentCategory = parts[0];
                    var pc = parents.Where(c => c.Label.ToUpper() == parentCategory).FirstOrDefault();
                    if (pc == null) {
                        pc = new Categories().Single(new { Label = productCategory });
                        if (pc == null) {
                            pc = new Categories().Add(new {
                                Label = parentCategory
                            });
                            parents.Add(pc);
                        }
                    }

                    if (parts.Length > 1) {
                        var childCategory = parts[1];
                        var child = pc.Child(childCategory, true);
                        product["category_id"] = child.ID;
                    } else {
                        product["category_id"] = pc.ID;
                    }
                }
                var description = product["description"];
                description = Fuze.Util.Toolkit.UrlDecode(description);
                //var url = product["picture"];
                //var images = product["images"];

                //var data = Util.Http.GET(url);
                //Util.IO.File.WriteBinary("C:\\Work\\product.jpg", data);
                //product["picture_snapshot"] = data;
            }

            //var cats = new Categories().Select();
            //var parents = new Categories().Where(new { ParentID = DBNull.Value });
            //var productData = Fuze.Util.IO.File.ReadText("C:\\Work\\products.json");
            //var products = Fuze.Util.Codecs.JSON.Decode<List<Dictionary<string, dynamic>>>(productData);
            //foreach (var product in products) {
            //    var productCategory = ((string)product["category"]).ToUpper();
            //    var parts = productCategory.Split('/');
            //    var parentCategory = parts[0];
            //    var pc = parents.Where(c => c.Label.ToUpper() == parentCategory).FirstOrDefault();
            //    if (pc == null) {
            //        pc = new Categories().Single(new { Label = productCategory });
            //        if (pc == null) {
            //            pc = new Categories().Add(new {
            //                Label = parentCategory
            //            });
            //            parents.Add(pc);
            //        }
            //    }

            //    if (parts.Length > 1) {
            //        var childCategory = parts[1];
            //        var child = pc.Child(childCategory, true);
            //        product["category_id"] = child.ID;
            //    } else {
            //        product["category_id"] = pc.ID;
            //    }

            //    var description = product["description"];
            //    description = Fuze.Util.Toolkit.UrlDecode(description);
            //    var url = product["picture"];

            //    //var data = Util.Http.GET(url);
            //    //Util.IO.File.WriteBinary("C:\\Work\\product.jpg", data);
            //    //product["picture_snapshot"] = data;
            //}


            Log.Trace("...");

            //new Categories().Fetch(
            //    new {
            //        ParentID = 1
            //    }
            //);

            //var filtered = new Categories().Where(new {
            //    ParentID = 2
            //});

            //var categories = new Categories().Select();
            //var category = categories[0];
            //var childrens = category.Children;
            ////category.Update(new {
            ////    Description = "This is a category."
            //// });

            //var child = category.Create("MIRRORS");
            //var children = category.Children;


            var db = Fuze.Data.Database.Open("DefaultConnection");
            //db.Insert(new Query("INSERT INTO categories (label, ParentID) VALUES (@label, @ParentID);").Set("Label", "ACCESSORIES").Set("ParentID", 2));

            //var people = db.SelectRecords<Person>("SELECT applicant_transaction_number as ID, first_name as FirstName, last_name as LastName FROM \"Applicants\" LIMIT 10;");
            //var records = db.SelectRecords("SELECT first_name as FirstName, last_name as LastName FROM \"Applicants\" LIMIT 10;");
            //var records = db.SelectRecords("SELECT * FROM \"Applicants\";");

            //db.Insert(new Query("INSERT INTO categories (label) VALUES (@label);").Set("Label", "ACCESSORIES"));
            //db.Insert(new Query("INSERT INTO categories (label) VALUES (@label);").Set("Label", "ARCHITECTURAL"));
            //db.Insert(new Query("INSERT INTO categories (label) VALUES (@label);").Set("Label", "FURNISHINGS"));
            //db.Insert(new Query("INSERT INTO categories (label) VALUES (@label);").Set("Label", "TEXTILES"));

            var records = db.SelectRecords<Category>("SELECT * FROM Categories;");
            Console.WriteLine("...");
        }
    }
}
