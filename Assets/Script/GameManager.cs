using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode currentGameMode;
    public Character playerCharacter = Character.Milche;
    public Character player2Character = Character.Milche;
    public AudioSource audioSource;
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySE(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
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
