using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using BluetoothCubo;

public class JsonSaving
{
    public CubeTracker _cubeTracker;
    private string path = "";
    private string persistentPath = "";

    public JsonSaving(string path)
    {
        SetPaths(path);
        if (File.Exists(persistentPath))
        {
            LoadData();
        }
        else
        {
            CreateData();
            SaveTheData();
        }
    }

    private void CreateData()
    {
        _cubeTracker = new CubeTracker();
    }

    private void SetPaths(string path)
    {
        persistentPath = path;
    }

    public void SaveTheData()
    {
        string savePath = persistentPath;

        string jsonString = JsonSerializer.Serialize(_cubeTracker);
        File.WriteAllText(savePath, jsonString);
    }

    public void dumpData()
    {
        string savePath = persistentPath;
        File.Delete(savePath);
    }

    public void LoadData()
    {
        string jsonString = File.ReadAllText(path);
        _cubeTracker = JsonSerializer.Deserialize<CubeTracker>(jsonString);
    }
}