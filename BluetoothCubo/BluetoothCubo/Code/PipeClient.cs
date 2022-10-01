using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace BluetoothCubo
{
    public class PipeClient
    {
        public void createComunication()
        {
            using (NamedPipeClientStream namedPipeClient = new NamedPipeClientStream("test-pipe"))
            {
                namedPipeClient.Connect();
                Console.WriteLine(namedPipeClient.ReadByte());
                namedPipeClient.WriteByte(2);
            }
        }
    }
}