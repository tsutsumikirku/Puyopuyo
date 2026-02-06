using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    int selection = 0;
    void Start()
    {
        var saveData = SaveSystem.Load();
        selection = saveData.selection;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            selection++;
            if(selection  > 1)
            {
                selection = 0;
            }
            SaveSystem.Save(new SaveData { selection = selection });
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selection--;
            if(selection < 0)
            {
                selection = 1;
            }
            SaveSystem.Save(new SaveData { selection = selection });
        }
    }
}
[System.Serializable]
public class SaveData
{
    public int selection;
    public int player1Character;
    public int player2Character;
}
public static class SaveSystem
{
    private static string Path => System.IO.Path.Combine(
        Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        var json = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(Path, json);
    }

    public static SaveData Load()
    {
        if (!System.IO.File.Exists(Path)) return new SaveData();
        var json = System.IO.File.ReadAllText(Path);
        return JsonUtility.FromJson<SaveData>(json);
    }
}