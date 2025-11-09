public interface ISaveable
{
    void SaveData(ref SaveSystem.SaveData data);
    void LoadData(SaveSystem.SaveData data);
}

