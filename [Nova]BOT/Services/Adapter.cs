using Microsoft.Win32;
using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace NovaBOT
{
    internal class Adapter
    {
        public ManagementObject adapter;
        public string adaptername;
        public string customname;
        public int devnum;

        public Adapter(ManagementObject a, string aname, string cname, int n)
        {
            adapter = a;
            adaptername = aname;
            customname = cname;
            devnum = n;
        }

        public Adapter(NetworkInterface i) : this(i.Description) { }

        public Adapter(string aname)
        {
            adaptername = aname;

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from win32_networkadapter where Name='" + adaptername + "'");
            ManagementObjectCollection found = searcher.Get();
            adapter = found.Cast<ManagementObject>().FirstOrDefault();

            try
            {
                Match match = Regex.Match(adapter.Path.RelativePath, "\\\"(\\d+)\\\"$");
                devnum = int.Parse(match.Groups[1].Value);
            }
            catch
            {
                return;
            }

            customname = NetworkInterface.GetAllNetworkInterfaces().Where(
                i => i.Description == adaptername
            ).Select(
                i => " (" + i.Name + ")"
            ).FirstOrDefault();
        }

        public NetworkInterface ManagedAdapter => NetworkInterface.GetAllNetworkInterfaces().Where(
                    nic => nic.Description == adaptername
                ).FirstOrDefault();

        public string Mac
        {
            get
            {
                try
                {
                    return BitConverter.ToString(ManagedAdapter.GetPhysicalAddress().GetAddressBytes()).Replace("-", "").ToUpper();
                }
                catch { return null; }
            }
        }

        public string RegistryKey => string.Format(@"SYSTEM\ControlSet001\Control\Class\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\{0:D4}", devnum);

        public string RegistryMac
        {
            get
            {
                try
                {
                    using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(RegistryKey, false))
                    {
                        return regkey.GetValue("NetworkAddress").ToString();
                    }
                }
                catch
                {
                    return null;
                }
            }
        }

        public bool SetRegistryMac(string value)
        {
            bool shouldReenable = false;
            try
            {
                if (value.Length > 0 && !Adapter.IsValidMac(value, false))
                {
                    throw new Exception(value + " is not a valid mac address");
                }

                using (RegistryKey regkey = Registry.LocalMachine.OpenSubKey(RegistryKey, true))
                {
                    if ((regkey.GetValue("AdapterModel") as string) != adaptername
                        && (regkey.GetValue("DriverDesc") as string) != adaptername)
                    {
                        throw new Exception("Adapter not found in registry");
                    }

                    uint result = (uint)adapter.InvokeMethod("Disable", null);
                    if (result != 0)
                    {
                        throw new Exception("Failed to disable network adapter.");
                    }

                    shouldReenable = true;

                    if (regkey != null)
                    {
                        if (value.Length > 0)
                        {
                            regkey.SetValue("NetworkAddress", value, RegistryValueKind.String);
                        }
                        else
                        {
                            regkey.DeleteValue("NetworkAddress");
                        }
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }

            finally
            {
                if (shouldReenable)
                {
                    uint result = (uint)adapter.InvokeMethod("Enable", null);
                    if (result != 0)
                    {
                        Console.WriteLine("Failed to re-enable network adapter.");
                    }
                }
            }
        }

        public override string ToString()
        {
            return adaptername + customname;
        }

        public static string GetNewMac()
        {
            System.Random r = new System.Random();
            byte[] bytes = new byte[6];
            r.NextBytes(bytes);

            bytes[0] = (byte)(bytes[0] | 0x02);
            bytes[0] = (byte)(bytes[0] & 0xfe);

            return MacToString(bytes);
        }

        public static bool IsValidMac(string mac, bool actual)
        {
            if (mac.Length != 12)
            {
                return false;
            }

            if (mac != mac.ToUpper())
            {
                return false;
            }

            if (!Regex.IsMatch(mac, "^[0-9A-F]*$"))
            {
                return false;
            }

            if (!actual)
            {
                char c = mac[1];
                return c == '2' || c == '6' || c == 'A' || c == 'E';
            }

            return true;
        }

        public static bool IsValidMac(byte[] bytes, bool actual)
        {
            return IsValidMac(Adapter.MacToString(bytes), actual);
        }

        public static string MacToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
        }
    }
}
