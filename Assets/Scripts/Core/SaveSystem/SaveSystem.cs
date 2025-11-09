using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveSystem 
{
    private static SaveData _saveData = new SaveData();
    private static List<ISaveable> _saveables = new List<ISaveable>();

    [System.Serializable]
    public class SaveData
    {
        public PlayerSaveData PlayerSaveData = new PlayerSaveData();
        public LeveleSaveData LevelSaveData = new LeveleSaveData();
    }

    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/save" + ".json";
        return saveFile;
    }

    public static void Register(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable))
        {
            _saveables.Add(saveable);
        }
    }

    public static void Unregister(ISaveable saveable)
    {
        if (_saveables.Contains(saveable))
        {
            _saveables.Remove(saveable);
        }
    }

    public static void Save()
    {
        HandleSaveData();

        string saveFilePath = SaveFileName();
        string directory = Path.GetDirectoryName(saveFilePath);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(_saveData, true));
    }
    
    public static void HandleSaveData()
    {
        foreach(ISaveable savable in _saveables)
        {
            savable.SaveData(ref _saveData);
        }
        //GameManager.MIDIManager.Save(ref _saveData.MIDIData);
        //GameManager.ScoreManager.Save(ref _saveData.ScoreSaveData);
    }

    public static void Load()
    {
        string saveFilePath = SaveFileName();
        
        if(!File.Exists(saveFilePath))
        {
            Debug.Log("Save file not found, creating default save.");
            Save();
        }

        string savedContent = File.ReadAllText(saveFilePath);

        _saveData = JsonUtility.FromJson<SaveData>(savedContent);
        HandleLoadData();
    }

    public static void HandleLoadData()
    {
        foreach(ISaveable saveable in _saveables)
        {
            saveable.LoadData(_saveData);
        }
        //GameManager.MIDIManager.Load(_saveData.MIDIData);
        //GameManager.ScoreManager.Load(_saveData.ScoreSaveData);
    }
}
