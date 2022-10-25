using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class RunProcess : MonoBehaviour
{
    private Process process = null;
    private ProcessMessages _messages;
    StreamWriter messageStream;


    private void Start()
    {
        StartProcess();
        _messages = GetComponent<ProcessMessages>();
    }

    public void StartProcess()
    {
        if (process != null && !process.HasExited)
            process.Kill();

        try
        {
            process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = Application.dataPath + "/Executable/BluetoothCubo.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
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
        string data = eventArgs.Data;
        print($"<color=#00FF00> Process : " + data + "</color>");
        _messages.EnqueueMsg(data);
    }


    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }

    public void SendMessageProcess(string msg)
    {
        messageStream.WriteLine(msg);
    }

    void OnApplicationQuit()
    {
        if (process != null && !process.HasExited)
        {
            process.Kill();
            print("process dead");
        }
    }
}