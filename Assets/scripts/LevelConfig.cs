using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
[InitializeOnLoad]
#endif

public class LevelConfig : ScriptableObject
{

    #region Edit + Editor + Singleton

#if UNITY_EDITOR
    [MenuItem("Level/Level Config")]
    public static void Edit()
    {
        Selection.activeObject = LevelConfig.instance;
    }
#endif

    private const string ISNSettingsAssetName = "LevelConfigSettings";
    private const string ISNSettingsPath = "Resources";
    private const string ISNSettingsAssetExtension = ".asset";

    private static LevelConfig _instance = null;

    public static LevelConfig instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load(ISNSettingsAssetName) as LevelConfig;

                if (_instance == null)
                {
                    _instance = CreateInstance<LevelConfig>();
#if UNITY_EDITOR
                    string fullPath = Path.Combine(Path.Combine("Assets", ISNSettingsPath),
                                                   ISNSettingsAssetName + ISNSettingsAssetExtension
                                                   );
                    AssetDatabase.CreateAsset(_instance, fullPath);
#endif
                }
            }
            return _instance;
        }
    }

    #endregion

    public List<Level> levels = new List<Level>();




}


[System.Serializable]
public class Level
{
    public int Time;
    public int CountEnemy;
    public int MaxSpeedEnemy;
    public int MinSpeedEnemy;
}
