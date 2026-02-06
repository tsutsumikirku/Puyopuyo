using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode currentGameMode;
    public Character playerCharacter = Character.Milche;
    public Character player2Character = Character.Milche;
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
}
public enum GameMode
{
    Single,
    Versus
}
public enum Character
{
    Milche = 0,
    Ruche = 1,
    Beriena = 2,
    Mincho = 3

}
