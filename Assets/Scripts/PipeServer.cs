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
    private bool valueSelected = false;

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
        while (!String.Equals(buffer,"null"))
        {
            buffer = ReciveDataFromClient(namedPipeServerStream);
            print(buffer);
            print(!String.Equals(buffer,"null"));
            if (!String.Equals(buffer,"null"))
            {
                SendDataToClient(namedPipeServerStream, 1);
                devices.Add(buffer);
                Dropdown.options.Add(new TMP_Dropdown.OptionData(buffer));
                print(buffer);
            }
        }


        while (!valueSelected) ;
        SendDataToClient(namedPipeServerStream, Convert.ToByte(Dropdown.value));
    }

    public void ValueChanged()
    {
        valueSelected = true;
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