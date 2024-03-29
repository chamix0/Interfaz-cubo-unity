﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothCubo
{
    internal class Program
    {
        #region Data

        private const int BUFFER_SIZE = 256;
        private static DeviceInformation device = null;
        public static string CUBE_TRACK_ID = "aadb";
        public static String selectedDevice = null; //"GiC69331";
        bool connectionSuccess = false;


        //Variables
        private static DevicesList _devicesList;
        private static CubeTracker _cubeTracker;

        #endregion

        // public static async Task Main(string[] args)
        // {
        //     //init variables
        //     _devicesList = new DevicesList();
        //     _cubeTracker = new CubeTracker();
        //
        //     //start Bluetooth 
        //     await BluetoothConection();
        // }


        private static async Task BluetoothConection()
        {
            bool connectionSuccess = false;

            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher deviceWatcher =
                DeviceInformation.CreateWatcher(
                    BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            deviceWatcher.Start();
            while (true)
            {
                if (device == null)
                {
                    Thread.Sleep(200);
                    ChooseDevice();
                }
                else
                {
                    BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(device.Id);
                    Console.WriteLine("Attempting to pair with the cube");
                    GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();

                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        connectionSuccess = true;
                        Console.WriteLine("pairing succeded");
                        var services = result.Services;
                        foreach (var service in services)
                        {
                            Console.WriteLine(service.Uuid);
                            if (service.Uuid.ToString("N").Substring(4, 4) == CUBE_TRACK_ID)
                            {
                                Console.WriteLine("found face track service");
                                GattCharacteristicsResult characteristicsResult =
                                    await service.GetCharacteristicsAsync();

                                if (result.Status == GattCommunicationStatus.Success)
                                {
                                    var characteristics = characteristicsResult.Characteristics;
                                    foreach (var characteristic in characteristics)
                                    {
                                        Console.WriteLine("-----------------");
                                        Console.WriteLine(characteristic);
                                        GattCharacteristicProperties properties =
                                            characteristic.CharacteristicProperties;

                                        if (properties.HasFlag(GattCharacteristicProperties.Notify))
                                        {
                                            Console.WriteLine("notify property found");
                                            GattCommunicationStatus status =
                                                await characteristic
                                                    .WriteClientCharacteristicConfigurationDescriptorAsync(
                                                        GattClientCharacteristicConfigurationDescriptorValue
                                                            .Notify);
                                            if (status == GattCommunicationStatus.Success)
                                                characteristic.ValueChanged += Characteristic_ValueChanged;
                                            
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (connectionSuccess)
                    {
                        Console.WriteLine("connection successful");
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                    }
                    else
                        Console.WriteLine("connection failed");

                    break;
                }
            }
        }

        private static void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] values = new Byte[20];
            reader.ReadBytes(values);
            string bin = Convert.ToString(values[16], 2);

            string aux = "";
            for (int j = 0; j < 8 - bin.Length; j++)
                aux += "0";
            
            aux += bin;
            bin = aux;

            Console.Out.WriteLine(_cubeTracker.ReadFaces(bin));
        }

        private static void ChooseDevice()
        {
            string[] devices = _devicesList.GetDevices();
            //Send the number of devices
            Console.WriteLine(devices.Length);
            for (int i = 0; i < devices.Length; i++)
                Console.WriteLine(devices[i]);

            device = _devicesList.GetDevice(Console.ReadLine());
            if (device==null)Console.WriteLine("Device not found");
        }

        private static void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            // throw new NotImplementedException();
        }

        private static void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // throw new NotImplementedException();
        }

        private static void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args) =>
            _devicesList.RemoveDevice(args.Id);

        private static void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // throw new NotImplementedException();
        }

        private static void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args) =>
            _devicesList.AddDevice(args);
    }
}