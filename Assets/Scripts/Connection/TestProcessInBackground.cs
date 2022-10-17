using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class TestProcessInBackground : MonoBehaviour
{
    private string path;
    private Process _process;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/Executables/BluetoothCubo.exe";
        ProcessStartInfo psi = new ProcessStartInfo(path,
            Application.persistentDataPath + Path.AltDirectorySeparatorChar +
            "Savedata.json");
        psi.UseShellExecute = true;
        // psi.WindowStyle = ProcessWindowStyle.Hidden;
        // psi.WindowStyle = ProcessWindowStyle.Minimized;
        _process = Process.Start(psi);
    }

    private void OnDestroy()
    {
        _process.CloseMainWindow();
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}