using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
namespace AOI.Data {
    public static class Drivers {
        #region -------- CONSTRUCTOR/VARIABLES --------
        private static Dictionary<string, DatabaseDriver> _drivers = new Dictionary<string, DatabaseDriver>(StringComparer.OrdinalIgnoreCase);
        static Drivers() {
            var baseType = typeof(DatabaseDriver);
            var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            var driverTypes = types.Where(t => t.IsSubclassOf(baseType)).ToList();

            foreach (var driverType in driverTypes) {
                var driver = (DatabaseDriver)Activator.CreateInstance(driverType);
                Register(driver);
            }
        }
        #endregion

        #region -------- PUBLIC - Register --------
        public static void Register(DatabaseDriver driver) {
            Contract.Requires(driver != null);
            lock (_drivers) {
                if (_drivers.ContainsKey(driver.Name)) {
                    if (_drivers[driver.Name].GetType().Equals(driver.GetType()))
                        return;
                }

                if (_drivers.ContainsKey(driver.Name))
                    throw new Exception("A driver with that name has already been registered! Name-> " + driver.Name);
                if (_drivers.ContainsKey(driver.Type))
                    throw new Exception("A driver with that type has already been registered! Type-> " + driver.Type);
                _drivers.Add(driver.Name, driver);
                _drivers.Add(driver.Type, driver);
            }
        }
        #endregion


        #region -------- PUBLIC - Get --------
        public static DatabaseDriver Get(string driverType) {
            Contract.Requires(!String.IsNullOrEmpty(driverType));
            try {
                var type = _drivers[driverType].GetType();
                var driver = (DatabaseDriver)Activator.CreateInstance(type);
                return driver;
            } catch (Exception ex) {
                throw new Exception("The requested driver could not be found, it has not been registered. DriverType-> " + driverType, ex);
            }
        }
        #endregion


        #region -------- PROPERTIES --------
        public static int NumberOfDrivers{
            get { return _drivers.Count; }
        }
        #endregion
    }
}
