using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public string SceneName;
    public void SceneChange()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
}
