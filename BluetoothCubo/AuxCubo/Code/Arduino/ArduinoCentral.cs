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
        public static string VIBRATION_ID = "19B10001E8F2537E4F6CD104768A1214";
        public static string MOTION_SENSOR_ID = "19B10000E8F2537E4F6CD104768A1214";
        public static string LED_PANEL_ID = "19B10002E8F2537E4F6CD104768A1214";

        public static String selectedDevice = null; //"GiC69331";
        bool connectionSuccess = false;
        private static GattCharacteristic motionSensorCharacteristicRead;
        private static GattCharacteristic ledCharacteristicWrite;
        private static GattCharacteristic ledPanelCharacteristicWrite;

        //Variables
        private static DevicesList _devicesList;

        #endregion

        public static async Task Main(string[] args)
        {
            //init variables
            _devicesList = new DevicesList();

            //start Bluetooth 
            await BluetoothConection();
        }


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
                    Console.WriteLine("Attempting to pair with the Arduino");
                    GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync();

                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        connectionSuccess = true;
                        Console.WriteLine("pairing succeded");
                        var services = result.Services;
                        foreach (var service in services)
                        {
                            Console.WriteLine(service.Uuid);
                            if (CompareUuid(service.Uuid.ToString("N"), VIBRATION_ID))
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
                                        Console.WriteLine(characteristic.Uuid.ToString("N"));
                                        GattCharacteristicProperties properties =
                                            characteristic.CharacteristicProperties;

                                        if (CompareFullUuid(characteristic.Uuid.ToString("N"), VIBRATION_ID))
                                        {
                                            Console.WriteLine("vibration property found");

                                            if (properties.HasFlag(GattCharacteristicProperties.Write))
                                            {
                                                ledCharacteristicWrite = characteristic;
                                            }
                                        }
                                        else if (CompareFullUuid(characteristic.Uuid.ToString("N"), LED_PANEL_ID))
                                        {
                                            Console.WriteLine("LED PANEL property found");
                                            if (properties.HasFlag(GattCharacteristicProperties.Write))
                                            {
                                                ledPanelCharacteristicWrite = characteristic;
                                            }
                                        }
                                        else if (CompareFullUuid(characteristic.Uuid.ToString("N"), MOTION_SENSOR_ID))
                                        {
                                            if (properties.HasFlag(GattCharacteristicProperties.Notify))
                                            {
                                                Console.WriteLine("sensor property found");
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
                    }


                    if (connectionSuccess)
                        Console.WriteLine("connection successful");
                    else
                        Console.WriteLine("connection failed");


                    string inputMsg = Console.ReadLine();
                    while (inputMsg != "f")
                    {
                        inputMsg = Console.ReadLine();
                        int value = 0;
                        bool isNumeric = Int32.TryParse(inputMsg, out value);
                        if (isNumeric)
                        {
                            if (value <= 1)
                            {
                                Console.WriteLine("Writting value in characteristic " + ledCharacteristicWrite.Uuid);
                                writeDevice(ledCharacteristicWrite, ConvertInt32ToByteArray(value)[0]);
                            }
                            else
                            {
                                Console.WriteLine("Writting value in characteristic " + ledCharacteristicWrite.Uuid);
                                writeDevice(ledPanelCharacteristicWrite, ConvertInt32ToByteArray(value)[0]);
                            }
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
            byte[] input = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(input);

            Console.Out.WriteLine("Motion sensor: " + input[0]);
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

        public static async Task writeDevice(GattCharacteristic characteristic, byte value)
        {
            var writer = new DataWriter();
            writer.WriteByte(value);
            GattCommunicationStatus result =
                await characteristic.WriteValueAsync(writer.DetachBuffer());
            if (result == GattCommunicationStatus.Success)
            {
                // Successfully wrote to device
            }
        }

        public static byte[] ConvertInt32ToByteArray(Int32 i32)
        {
            return BitConverter.GetBytes(i32);
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

        #region Utils

        static bool CompareUuid(string Uuid, string cad)
        {
            return Uuid.Substring(0, 4) == cad.ToLower().Substring(0, 4);
        }

        static bool CompareFullUuid(string Uuid, string cad)
        {
            return Uuid.Equals(cad.ToLower());
        }

        #endregion
    }
}