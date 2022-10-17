using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestProcessInBackground : MonoBehaviour
{
    private string path;
    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + "/Executables/v1/BluetoothCubo.exe";
        ProcessStartInfo psi = new ProcessStartInfo(Application.dataPath + "/Executables/v1/BluetoothCubo.exe");
        psi.UseShellExecute = true;
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        psi.WindowStyle = ProcessWindowStyle.Minimized;
        Process process = Process.Start(psi);
    }

    // Update is called once per frame
    void Update()
    {
    }

 
}