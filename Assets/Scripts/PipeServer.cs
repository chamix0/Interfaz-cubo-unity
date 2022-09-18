using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PipeServer : MonoBehaviour
{
    private NamedPipeServerStream namedPipeServer;

    // Start is called before the first frame update
    void Start()
    {
        //start the process that manages bluetooth conection
        
        // ProcessStartInfo psi = new ProcessStartInfo(Application.dataPath + "/Executables/v2/BluetoothCubo.exe");
        // psi.UseShellExecute = true;
        // psi.WindowStyle = ProcessWindowStyle.Hidden;
        // psi.WindowStyle = ProcessWindowStyle.Minimized;
        // Process process = Process.Start(psi);


        using (namedPipeServer)
        {
          //conect Server to 
            
            
            
            namedPipeServer = new NamedPipeServerStream("test-pipe");
            namedPipeServer.WaitForConnection();
            // namedPipeServer.WriteByte(1);
            // int byteFromClient = namedPipeServer.ReadByte();
            // Debug.Log(byteFromClient);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnApplicationQuit()
    {
        print("cerrando el pipe");
        namedPipeServer.Flush();
        namedPipeServer.Disconnect();
        namedPipeServer.Dispose();
    }

    public void clicked()
    {
        using (namedPipeServer)
        {
            namedPipeServer.WriteByte(1);

            byte[] buffer = new byte[256];
            int byteFromClient = namedPipeServer.Read(buffer, 0, buffer.Length);
            string someString = Encoding.ASCII.GetString(buffer);
            Debug.Log(someString);
        }
    }
}