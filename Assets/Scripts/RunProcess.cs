using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class RunProcess : MonoBehaviour
{
    private Process process = null;
    private ProcessMessages _messages;
    StreamWriter messageStream;


    private void Start()
    {
        _messages = GetComponent<ProcessMessages>();
        StartProcess();
    }

    void StartProcess()
    {
        try
        {
            process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = Application.dataPath + "./Executable/BluetoothCubo.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += new DataReceivedEventHandler(DataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
            process.Start();
            process.BeginOutputReadLine();
            messageStream = process.StandardInput;

            UnityEngine.Debug.Log("Successfully launched app");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
        }
    }


    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        _messages.Enqueue(eventArgs.Data);
        print(eventArgs.Data);
    }


    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }

    public void SendMessage(string msg)
    {
        messageStream.WriteLine(msg);
    }

    void OnApplicationQuit()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
        }
    }

    public Process GetProcess()
    {
        return process;
    }
}