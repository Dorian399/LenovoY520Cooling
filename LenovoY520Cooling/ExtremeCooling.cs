using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace LenovoY520Cooling
{
    internal class ExtremeCooling
    {
        public static bool Exists()
        {
            try
            {
                const string wmiNamespace = @"root\wmi";
                ManagementScope scope = new ManagementScope($@"\\.\{wmiNamespace}");
                scope.Connect();

                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM LENOVO_GAMEZONE_DATA"));
                ManagementObjectCollection instances = searcher.Get();

                foreach (ManagementObject instance in instances)
                {
                    try
                    {
                        ManagementBaseObject inParams = instance.GetMethodParameters("SetFanCooling");
                        if (inParams != null)
                        {
                            return true;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void SetEnabled(bool enabled)
        {
            const string wmiNamespace = @"root\wmi";
            ManagementScope scope = new ManagementScope($@"\\.\{wmiNamespace}");
            scope.Connect();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM LENOVO_GAMEZONE_DATA"));
            ManagementObjectCollection instances = searcher.Get();

            foreach (ManagementObject instance in instances)
            {
                ManagementBaseObject inParams = instance.GetMethodParameters("SetFanCooling");
                inParams["Data"] = (uint)(enabled ? 1 : 0);

                ManagementBaseObject outParams = instance.InvokeMethod("SetFanCooling", inParams, null);

            }
        }
    }
}
