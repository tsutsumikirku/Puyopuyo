using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    int selection = 0;
    [SerializeField] TitleButton[] titleButtons;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSE;
    [SerializeField] AudioClip decisionSE;
    [SerializeField] SceneChanger1 sceneChanger;
    void Start()
    {
        var saveData = SaveSystem.Load();
        selection = saveData.selection;
        for (int i = 0; i < titleButtons.Length; i++)
        {
            if(i == selection)
            {
                titleButtons[i].Select();
            }
            else
            {
                titleButtons[i].Unselect();
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            audioSource.PlayOneShot(decisionSE);
            selection++;
            if(selection  > 2)
            {
                selection = 0;
            }
            SaveSystem.Save(new SaveData { selection = selection });
            for (int i = 0; i < titleButtons.Length; i++)
            {
                if(i == selection)
                {
                    titleButtons[i].Select();
                }
                else
                {
                    titleButtons[i].Unselect();
                }
        }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            audioSource.PlayOneShot(decisionSE);
            selection--;
            if(selection < 0)
            {
                selection = 2;
            }
            SaveSystem.Save(new SaveData { selection = selection });
            for (int i = 0; i < titleButtons.Length; i++)
            {
                if(i == selection)
                {
                    titleButtons[i].Select();
                }
                else
                {
                    titleButtons[i].Unselect();
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch (selection)
            {
                case 0:
                    GameManager.instance.currentGameMode = GameMode.Single;
                    sceneChanger.ChangeSceneAsync("SingleCharacterSelect").Forget();
                    break;
                case 1:
                    GameManager.instance.currentGameMode = GameMode.Versus;
                    sceneChanger.ChangeSceneAsync("MultiCharacterSelect").Forget();
                    break;
                case 2:
                    
                    break;
            }
        }
    }
    public void ChangeScene(int index)
    {
        switch (index)
        {
            case 0:
                GameManager.instance.currentGameMode = GameMode.Single;
                sceneChanger.ChangeSceneAsync("SingleCharacterSelect").Forget();
                break;
            case 1:
                GameManager.instance.currentGameMode = GameMode.Versus;
                sceneChanger.ChangeSceneAsync("MultiCharacterSelect").Forget();
                break;
            case 2:
                
                break;
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