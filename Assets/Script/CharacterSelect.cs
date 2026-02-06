using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public GameMode currentGameMode;
    public Character playerCharacter = Character.Milche;
    public Character player2Character = Character.Milche;
    public CharacterSelectCharacter characterSelectCharacter;
    public CharacterSelectCharacter secondCharacterSelectCharacter; 
    public CharacterSelectButton[] characterImage;

    // Update is called once per frame
    void Start()
    {
        playerCharacter = GameManager.instance.playerCharacter;
        player2Character = GameManager.instance.player2Character;
        currentGameMode = GameManager.instance.currentGameMode;
        switch (currentGameMode)
        {
            case GameMode.Single:
                characterImage[(int)playerCharacter].OnePlayerSelect();
                characterSelectCharacter.SelectCharacter((int)playerCharacter);
                break;
            case GameMode.Versus:
                characterImage[(int)playerCharacter].OnePlayerSelect();
                characterImage[(int)player2Character].TwoPlayerSelect();
                characterSelectCharacter.SelectCharacter((int)playerCharacter);
                secondCharacterSelectCharacter.SelectCharacter((int)player2Character);
                break;
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            playerCharacter--;
            if((int)playerCharacter < 0)
            {
                playerCharacter = (Character)(characterImage.Length - 1);
            }
            GameManager.instance.playerCharacter = playerCharacter;
            for (int i = 0; i < characterImage.Length; i++)
            {
                if (i == (int)playerCharacter)
                {
                    characterImage[i].OnePlayerSelect();
                    characterSelectCharacter.SelectCharacter(i);
                }
                else
                {
                    characterImage[i].OnePlayerUnselect();
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            playerCharacter++;
            if((int)playerCharacter > 3)
            {
                playerCharacter = 0;
            }
            GameManager.instance.playerCharacter = playerCharacter;
            for (int i = 0; i < characterImage.Length; i++)
            {
                if (i == (int)playerCharacter)
                {
                    characterImage[i].OnePlayerSelect();
                    characterSelectCharacter.SelectCharacter(i);
                }
                else
                {
                    characterImage[i].OnePlayerUnselect();
                }
            }
        }
        if(currentGameMode == GameMode.Versus)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                player2Character--;
                if((int)player2Character < 0)
                {
                    player2Character = (Character)(characterImage.Length - 1);
                }
                GameManager.instance.player2Character = player2Character;
                for (int i = 0; i < characterImage.Length; i++)
                {
                    if (i == (int)player2Character)
                    {
                        characterImage[i].TwoPlayerSelect();
                        secondCharacterSelectCharacter.SelectCharacter(i);
                    }
                    else
                    {
                        characterImage[i].TwoPlayerUnselect();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                player2Character++;
                if((int)player2Character > 3)
                {
                    player2Character = 0;
                }   
                GameManager.instance.player2Character = player2Character;
                for (int i = 0; i < characterImage.Length; i++)
                {
                    if (i == (int)player2Character)
                    {
                        characterImage[i].TwoPlayerSelect();
                        secondCharacterSelectCharacter.SelectCharacter(i);
                    }
                    else
                    {
                        characterImage[i].TwoPlayerUnselect();
                    }
                }
            }
        }
    }
}
