using Cysharp.Threading.Tasks;
using UnityEngine;
using YourMaidTools;
public class SceneChanger1 : MonoBehaviour
{
    private bool isChangingScene = false;
    [SerializeField] private YMAnimationPlayer fadeOutAnimationPlayer;
    [SerializeField] private AudioSource SeAudioSource;
    [SerializeField]private AudioClip sceneChangeSE;
    void Start()
    {
        SeAudioSource.clip = sceneChangeSE;
        SeAudioSource.Play();
    }
    public async UniTask ChangeSceneAsync(string sceneName)
    {
        if (isChangingScene) return;
        isChangingScene = true;
        SeAudioSource.Play();
        if (fadeOutAnimationPlayer != null)
        {
            await fadeOutAnimationPlayer.PlayAnimation();
        }
        await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        isChangingScene = false;
    }
}
