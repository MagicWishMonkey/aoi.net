using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
namespace AOI.Data {
    public class Record {
        #region -------- CONSTRUCTOR/VARIABLES --------
        public Record() {
            this.Data = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        }
        public Record(int capacity) {
            Contract.Assert(capacity > 0, "The capacity parameter is less than 0!");

            this.Data = new Dictionary<string, object>(capacity, StringComparer.CurrentCultureIgnoreCase);
        }

        public Record(Dictionary<string, object> data, bool copyData = false) {
            Contract.Assert(data != null, "The data parameter is null!");

            if (copyData) {
                this.Data = data;
            } else {
                this.Data = new Dictionary<string, object>(data.Count, StringComparer.CurrentCultureIgnoreCase);
                foreach (string key in data.Keys) {
                    this.Data.Add(key, data[key]);
                }
            }
        }
        public Record(Record model) {
            Contract.Assert(model != null, "The model parameter is null!");
            this.Data = model.Data;
        }
        #endregion


        #region -------- PUBLIC - Get --------
        public object Get(string key) {
            Contract.Assert(!String.IsNullOrEmpty(key), "The key is null/empty!");
            //if (!this.Data.ContainsKey(key))
            //    return null;
            try {
                return this.Data[key];
            } catch (Exception ex) {
                throw new DatabaseException("The record does not contain an entry for the specified key-> " + key, ex);
            } 
        }
        public object TryGet(string key) {
            Contract.Assert(!String.IsNullOrEmpty(key), "The key is null/empty!");
            try {
                return this.Data[key];
            } catch{
                Log.Debug("Record.TryGet Error-> invalid key '" + key + "'");
                return null;
            }
        }

        public string GetString(string key) {
            var obj = this.Get(key);
            if (obj == null)
                return "";
            return Convert.ToString(obj);
        }
        public int GetInt(string key) {
            var val = this.Get(key);
            if (val is Int32)
                return (int)val;
            //var num = Convert.ToString(val);
            return Int32.Parse(Convert.ToString(val));
            //return (int)val;
        }
        public double GetDouble(string key) {
            var val = this.Get(key);
            return (double)val;
        }
        public bool GetBool(string key) {
            string val = this.GetString(key);
            if (val.Length == 0)
                return false;
            val = val.Trim();
            if (val == "1" || val.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                return true;
            return false;
        }
        public DateTime GetDateTime(string key) {
            string val = this.GetString(key);
            DateTime date;
            DateTime.TryParse(val, out date);
            return date;
        }
        #endregion


        #region -------- PUBLIC - Set --------
        public void Set(string key, object val) {
            Contract.Assert(!String.IsNullOrEmpty(key), "The key is null/empty!");
            lock (this.Data) {
                if (!this.Data.ContainsKey(key))
                    this.Data.Add(key, val);
                else
                    this.Data[key] = val;
            }
        }
        #endregion


        #region -------- PUBLIC - CloneSchema/Clone --------
        /// <summary>
        /// Return a new record instance with the same structure as this record, but without any of the data.
        /// </summary>
        /// <returns></returns>
        public Record CloneSchema() {
            var keys = this.Data.Keys;
            var data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in keys) {
                data.Add(key, null);
            }
            return new Record(data);
        }

        /// <summary>
        /// Return a new record instance containing the same structure and data as this record.
        /// </summary>
        public Record Clone() {
            return new Record(this.Data, true);
        }
        #endregion

        #region -------- PUBLIC - Contains/Clear/Delete --------
        public bool Contains(string key) {
            Contract.Assert(!String.IsNullOrEmpty(key), "The key is null/empty!");
            return this.Data.ContainsKey(key);
        }

        public void Clear(string key) {
            Contract.Assert(!String.IsNullOrEmpty(key), "The key parameter is null/empty!");

            if (this.Data.ContainsKey(key))
                this.Data[key] = null;
        }

        public void Delete(string key) {
            Contract.Assert(!String.IsNullOrEmpty(key), "The key parameter is null/empty!");

            if (this.Data.ContainsKey(key))
                this.Data.Remove(key);
        }
        #endregion

        #region -------- PUBLIC - Rename --------
        public void Rename(string key, string newKey) {
            Contract.Assert(!String.IsNullOrEmpty(key), "The key parameter is null/empty!");
            Contract.Assert(!String.IsNullOrEmpty(newKey), "The newKey parameter is null/empty!");

            var val = this.Get(key);
            this.Set(newKey, val);
            if (this.Data.ContainsKey(key))
                this.Data.Remove(key);
        }
        #endregion

        #region -------- PROPERTIES --------
        public object this[string key] {
            get { return this.Get(key); }
            set { this.Set(key, value); }                
        }
        public Dictionary<string, object> Data {
            get;
            private set;
        }
        #endregion

        #region -------- PUBLIC STATIC - FromDictionary/FromDictionaries --------
        public static Record FromDictionary(Dictionary<string, object> data) {
            Contract.Assert(data != null, "The data parameter is null!");
            Record model = new Record(data);
            return model;
        }

        public static List<Record> FromDictionaries(List<Dictionary<string, object>> dataList) {
            Contract.Assert(dataList != null, "The data parameter is null!");
            List<Record> list = new List<Record>(dataList.Count);
            for (int i = 0; i < dataList.Count; i++) {
                list.Add(new Record(dataList[i]));
            }
            return list;
        }

        public static List<Dictionary<string, object>> ToDictionaries(List<Record> models) {
            List<Dictionary<string, object>> dataList = new List<Dictionary<string, object>>(models.Count);
            for (int i = 0; i < models.Count; i++) {
                dataList.Add(models[i].Data);
            }
            return dataList;
        }
        #endregion
    }
}
