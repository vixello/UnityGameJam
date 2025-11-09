using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public struct LeveleSaveData
{
    public LevelInfo[] Levels;
}

public struct LevelInfo
{
    public bool didPlayCutscene;
    public int levelIndex;
    public int maxScore;
}

[System.Serializable]
public struct PlayerSaveData
{
    public int totalCoins;
}


