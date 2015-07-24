using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using AOI.Util;


namespace AOI {
    public class Settings {
        #region -------- CONSTRUCTOR & VARIABLES --------
        private static Settings _instance;
        private IDictionary<string, dynamic> _settings;
        public Settings(IDictionary<string, dynamic> settings) {
            this._settings = settings;
        }
        #endregion

        public static Dictionary<string, dynamic> Group(string key) {
            var o = Settings.Get<Dictionary<string, dynamic>>(key);
            return o;
        }

        public static T Get<T>(string key) {
            var o = Settings.Get(key);
            return (T)o;
        }
        public static object Get(string key) {
            var value = Instance._settings[key];
            if (value != null && value.GetType() == typeof(Newtonsoft.Json.Linq.JObject)) {
                var jo = value as Newtonsoft.Json.Linq.JObject;
                var converted = jo.ToObject<Dictionary<string, dynamic>>();
                return converted;
            }
            return value;
        }


        #region -------- PROPERTIES --------
        public static Dictionary<string, dynamic> Databases {
            get {
                return Group("databases");
            }
        }


        public static string Passcode {
            get {
                return (string)Get("passcode");
            }
        }
        #endregion


        #region -------- PRIVATE STATIC PROPERTIES --------
        private static Settings Instance {
            get {
                if (_instance != null) {
                    return _instance;
                }
                var folder = Toolkit.CurrentDirectory;
                var file = folder.FindPrecedingFile("settings.json");
                if (file == null)
                    throw new Exception("The settings.json file could not be automatically located. Please specify the location.");

                var settings = Settings.Load(file.FullPath);
                _instance = settings;
                return settings;
            }
        }
        #endregion

        #region -------- PUBLIC STATIC - Load --------
        public static Settings Load(string settingsFilePath = null, string settingsOverrideFilePath = null) {
            var settings = Settings.ParseSettings(settingsFilePath, settingsOverrideFilePath);
            _instance = new Settings(settings);
            return _instance;
        }
        #endregion

        //private static Dictionary<string, dynamic> Downcast(Dictionary<string, dynamic> tbl){
        //    var keys = tbl.Keys.ToArray();
        //    for (var i = 0; i < keys.Length; i++) {
        //        var key = keys[i];
        //            var value = tbl[key];
        //            if (value != null && value.GetType() == typeof(Newtonsoft.Json.Linq.JObject)) {
        //                var jo = value as Newtonsoft.Json.Linq.JObject;
        //                var converted = jo.ToObject<Dictionary<string, dynamic>>();
        //                tbl[key] = Downcast(converted);
        //            }
        //        }
        //    return tbl;
        //}

        #region -------- PRIVATE STATIC - ParseSettings --------
        private static IDictionary<string, dynamic> ParseSettings(string settingsFilePath = null, string settingsOverrideFilePath = null) {

            if (settingsFilePath != null) {
                var settingsFile = Toolkit.CurrentDirectory.Parent.File(settingsFilePath);
                if (settingsFile.Exists) {
                    var settingsData = settingsFile.ReadText();
                    var settings = Toolkit.JSON.Read<Dictionary<string, dynamic>>(settingsData.Trim());
                    if (settingsOverrideFilePath == null) {
                        settingsOverrideFilePath = settingsFile.Parent.File("settings.override.json").FullPath;
                    }
                    if (settingsOverrideFilePath != null) {
                        var settingsOverrideFile = Toolkit.CurrentDirectory.Parent.File(settingsOverrideFilePath);
                        if (settingsOverrideFile.Exists) {
                            var settingsOverrideData = settingsOverrideFile.ReadText();
                            var settingsOverride = Toolkit.JSON.Read<Dictionary<string, dynamic>>(settingsOverrideData.Trim());
                            Toolkit.Override(settings, settingsOverride);
                        }
                    }

                    return settings;
                }
            }
            return null;
        }
        #endregion
    }
}
