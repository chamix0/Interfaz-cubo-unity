using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothCubo
{
    public class ArduinoCentral
    {
        #region Data

        private const int BUFFER_SIZE = 256;
        private static DeviceInformation device = null;
        public static string CUBE_TRACK_ID = "19b1";
        public static String selectedDevice = null; //"GiC69331";
        bool connectionSuccess = false;
        private static GattCharacteristic ledCharacteristicRead;
        private static GattCharacteristic ledCharacteristicWrite;
        private static bool valueLed = false;

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
                    // Console.WriteLine("press any key to pair with the cube\n");
                    // Console.ReadLine();
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
                            Console.WriteLine(service.Uuid.ToString("N").Substring(0, 4));
                            if (service.Uuid.ToString("N").Substring(0, 4) == CUBE_TRACK_ID)
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

                                        if (properties.HasFlag(GattCharacteristicProperties.Read))
                                        {
                                            ledCharacteristicRead = characteristic;
                                        }

                                        if (properties.HasFlag(GattCharacteristicProperties.Write))
                                        {
                                            ledCharacteristicWrite = characteristic;
                                        }
                                    }
                                }
                            }
                        }
                    }


                    if (connectionSuccess)
                        Console.WriteLine("connection successful");
                    else
                        Console.WriteLine("connection failed");


                    string inputMsg = Console.ReadLine();
                    while (inputMsg != "f")
                    {
                        inputMsg = Console.ReadLine();
                        if (inputMsg == "1")
                        {
                            Console.WriteLine("Reading value in characteristic " + ledCharacteristicRead.Uuid);
                            readDevice(ledCharacteristicRead);
                        }
                        else if (inputMsg == "2")
                        {
                            Console.WriteLine("Writting value in characteristic " + ledCharacteristicWrite.Uuid);
                            writeDevice(ledCharacteristicWrite);
                        }
                    }

                    break;
                }
            }

            deviceWatcher.Stop();
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

            Console.Out.WriteLine(bin);
        }

        private static void ChooseDevice()
        {
            string[] devices = _devicesList.GetDevices();
            //send the number of devices
            Console.WriteLine(devices.Length);
            for (int i = 0; i < devices.Length; i++)
            {
                Console.WriteLine(devices[i]);
            }

            // Console.WriteLine(" write the index of the device you want to connect to:");
            device = _devicesList.GetDevice(Console.ReadLine());
        }


        public static async Task readDevice(GattCharacteristic characteristic)
        {
            GattReadResult result = await characteristic.ReadValueAsync();
            if (result.Status == GattCommunicationStatus.Success)
            {
                var reader = DataReader.FromBuffer(result.Value);
                byte[] input = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(input);
                Console.WriteLine(input);
                // Utilize the data as needed
            }
        }

        public static async Task writeDevice(GattCharacteristic characteristic)
        {
            var writer = new DataWriter();
            if (valueLed)
            {
                valueLed = false;
                writer.WriteByte(0x00);
            }
            else
            {
                valueLed = true;
                writer.WriteByte(0x01);
            }

            GattCommunicationStatus result =
                await characteristic.WriteValueAsync(writer.DetachBuffer());
            if (result == GattCommunicationStatus.Success)
            {
                // Successfully wrote to device
            }
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