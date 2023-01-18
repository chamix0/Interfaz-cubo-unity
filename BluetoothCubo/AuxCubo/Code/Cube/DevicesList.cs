using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Enumeration;

namespace BluetoothCubo
{
    public class DevicesList
    {
        private List<DeviceInformation> devices;

        public DevicesList()
        {
            devices = new List<DeviceInformation>();
        }

        private int ContainsDevice(string name)
        {
            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].Name == name || devices[i].Id == name)
                    return i;
            }
            return -1;
        }

        public void AddDevice(DeviceInformation device)
        {
            if (device.Name != "" && ContainsDevice(device.Name) == -1)
                devices.Add(device);
        }

        public void RemoveDevice(string id)
        {
            int index = ContainsDevice(id);
            if (index != -1)
                devices.RemoveAt(index);
        }

        public DeviceInformation GetDevice(string name)
        {
            int index = ContainsDevice(name);
            if (index != -1)
                return devices[index];
            return null;
        }

        public DeviceInformation GetDevice(int index)
        {
            if (index >= 0 && index < devices.Count)
                return devices[index];
            return null;
        }

        public string[] GetDevices()
        {
            List<string> names = new List<string>();
            foreach (var device in devices)
                names.Add(device.Name);
            return names.ToArray();
        }
    }
}