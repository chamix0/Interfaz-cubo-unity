using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class JsonSaving : MonoBehaviour
{
    [NonSerialized] public CubeTracker _cubeTracker;
    private string path = "";
    private string persistentPath = "";

    void Awake()
    {
        SetPaths();
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

    private void SetPaths()
    {
        path = Application.dataPath + "/savedata/Savedata.json";
        persistentPath =
            Application.persistentDataPath + Path.AltDirectorySeparatorChar +
            "Savedata.json"; //here is where you actually store the info
    }

    public void SaveTheData()
    {
        string savePath = persistentPath;
        Debug.Log("Saving Data at " + savePath);
        string json = JsonUtility.ToJson(_cubeTracker);
        Debug.Log(json);
        using StreamWriter writer = new StreamWriter(savePath);
        writer.Write(json);
    }

    public void dumpData()
    {
        string savePath = persistentPath;
        File.Delete(savePath);
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();
        CubeTracker data = JsonUtility.FromJson<CubeTracker>(json);
        _cubeTracker = data;
        Debug.Log(data.ToString());
    }
}