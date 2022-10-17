using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Button connectButton;
    private bool valueSelected = false;

    //pipes
    private NamedPipeServerStream namedPipeServerStream;

    private void Awake()
    {
        devices = new List<string>();
        connectButton.onClick.AddListener(ConnectButtonAction);
        connectButton.interactable = false;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Thread serverReadThread = new Thread(ServerThreadRead);
        serverReadThread.Start();
    }


    private void ServerThreadRead()
    {
        namedPipeServerStream = new NamedPipeServerStream("connect");
        namedPipeServerStream.WaitForConnection();
        Debug.Log("Client has conected");


        //num devices
        int numDevices = Int32.Parse(ReciveDataFromClient(namedPipeServerStream));
        string buffer = "";
        for (int i = 0; i < numDevices; i++)
        {
            buffer = ReciveDataFromClient(namedPipeServerStream);
            SendDataToClient(namedPipeServerStream, 1);
            devices.Add(buffer);
            Dropdown.options.Add(new TMP_Dropdown.OptionData(buffer));
            print(buffer);
        }

        connectButton.interactable = true;
    }

    void ConnectButtonAction()
    {
        connectButton.interactable = false;
        SendDataToClient(namedPipeServerStream, Convert.ToByte(Dropdown.value));
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
        namedPipeServer.Read(buffer, 0, BUFFER_SIZE);
        string someString = Encoding.ASCII.GetString(buffer);
        return someString;
    }
}