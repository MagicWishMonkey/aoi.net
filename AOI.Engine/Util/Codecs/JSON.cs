using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AOI.Util.Codecs {
    public class JSON {
        private static Dictionary<string, dynamic> Downcast(Dictionary<string, dynamic> tbl) {
            var keys = tbl.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++) {
                var key = keys[i];
                var value = tbl[key];
                if (value != null && value.GetType() == typeof(Newtonsoft.Json.Linq.JObject)) {
                    var jo = value as Newtonsoft.Json.Linq.JObject;
                    var converted = jo.ToObject<Dictionary<string, dynamic>>();
                    tbl[key] = Downcast(converted);
                }
            }
            return tbl;
        }


        public string Write(object o, bool pretty=false) {
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(o);
            return data;
        }

        public object Read(string data) {
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject(data, typeof(object));
            return o;
        }

        public T Read<T>(string data) {
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
            if (o.GetType() == typeof(Dictionary<string, dynamic>)) {
                var tbl = o as Dictionary<string, dynamic>;
                Downcast(tbl);
            }
            return o;
        }


        public static string Encode(object o) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(o);
        }

        public static object Decode(string data) {
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject(data, typeof(object));
            return o;
        }

        public static T Decode<T>(string data) {
            var o = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
            return o;
        }
    }
}
