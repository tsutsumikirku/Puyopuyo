using UnityEditor.SearchService;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode currentGameMode;
    public Character playerCharacter = Character.Milche;
    public Character player2Character = Character.Milche;
    public AudioSource audioSource;
    public AudioSource seAudioSource;
    public static GameManager instance;
    public int playerOneScore = 0;
    public int playerTwoScore = 0;
    public int playerOneCombo = 0;
    public int playerTwoCombo = 0;
    [SerializeField] AudioClip InGameBGM;
    [SerializeField] AudioClip OutGameBGM;
    public SceneType CurrentSceneType
    {
        get { return currentSceneType; }
        set
        {
            currentSceneType = value;
            switch (currentSceneType)
            {
                case SceneType.InGame:
                    audioSource.clip = InGameBGM;
                    audioSource.loop = true;
                    audioSource.Play();
                    break;
                case SceneType.OutGame:
                    audioSource.clip = OutGameBGM;
                    audioSource.loop = true;
                    audioSource.Play();
                    break;
                case SceneType.Stop:
                    audioSource.Pause();
                    break;
            }
        }
    }
    private SceneType currentSceneType = SceneType.OutGame;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            CurrentSceneType = SceneType.OutGame;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySE(AudioClip clip)
    {
        seAudioSource.PlayOneShot(clip);
    }
}
public enum SceneType
{
    OutGame,
    InGame,
    Stop
}
public enum GameMode
{
    Single,
    Versus
}
public enum Character
{
    Ruche = 0,
    Milche = 1,
    Beriena = 2,
    Mincho = 3

}
