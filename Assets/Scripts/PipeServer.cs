using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.IO.Pipes;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class PipeServer : MonoBehaviour
{
    private const int BUFFER_SIZE = 256;
    private List<string> devices;
    public TMP_Dropdown Dropdown;

    private void Awake()
    {
        devices = new List<string>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Thread serverReadThread = new Thread(ServerThreadRead);
        serverReadThread.Start();
    }


    private void ServerThreadRead()
    {
        NamedPipeServerStream namedPipeServerStream =
            new NamedPipeServerStream("pipe");

        namedPipeServerStream.WaitForConnection();
        Debug.Log("Client has conected");

        string buffer = "";
        while (buffer != "-1")
        {
            buffer = ReciveDataFromClient(namedPipeServerStream);
            SendDataToClient(namedPipeServerStream, 1);
            devices.Add(buffer);
            print(buffer);
        }


        while (true)
        {
            ReciveDataFromClient(namedPipeServerStream);
        }
    }

    private void SendDataToClient(NamedPipeServerStream namedPipeServer, Byte value)
    {
        namedPipeServer.WaitForPipeDrain();
        namedPipeServer.WriteByte(value);
    }


    private string ReciveDataFromClient(NamedPipeServerStream namedPipeServer)
    {
        namedPipeServer.WaitForPipeDrain();
        Byte[] buffer = new byte[BUFFER_SIZE];
        namedPipeServer.Read(buffer);
        string someString = Encoding.ASCII.GetString(buffer);
        return someString;
    }
}