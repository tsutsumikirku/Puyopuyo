using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public GameMode currentGameMode;
    public Character playerCharacter = Character.Milche;
    public Character player2Character = Character.Milche;
    public CharacterSelectButton[] characterImage;

    // Update is called once per frame
    void Start()
    {
        playerCharacter = GameManager.instance.playerCharacter;
        player2Character = GameManager.instance.player2Character;
    }
    void Update()
    {
        
    }
}
