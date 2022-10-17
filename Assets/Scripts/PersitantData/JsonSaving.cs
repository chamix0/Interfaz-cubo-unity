using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class JsonSaving : MonoBehaviour
{
    public SaveData _saveData;
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
        _saveData = new SaveData();
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
        string json = JsonUtility.ToJson(_saveData);
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
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        _saveData = data;
        Debug.Log(data.ToString());
    }
}