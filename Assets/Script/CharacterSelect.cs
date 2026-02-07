using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class CharacterSelect : MonoBehaviour
{
    public GameMode currentGameMode;
    public Character playerCharacter = Character.Milche;
    public Character player2Character = Character.Milche;
    public CharacterSelectCharacter characterSelectCharacter;
    public CharacterSelectCharacter secondCharacterSelectCharacter; 
    public CharacterSelectButton[] characterImage;
    public Image SpaceKeyImage;
    public float spaceKeyPushTime = 0.5f;
    bool isSpaceKeyPushed = false;
    [SerializeField] private SceneChanger1 sceneChanger;

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
                characterSelectCharacter.SilentCharacterSelect((int)playerCharacter);
                break;
            case GameMode.Versus:
                characterImage[(int)playerCharacter].OnePlayerSelect();
                characterImage[(int)player2Character].TwoPlayerSelect();
                characterSelectCharacter.SilentCharacterSelect((int)playerCharacter);
                secondCharacterSelectCharacter.SilentCharacterSelect((int)player2Character);
                break;
        }
    }
    void Update()
    {
        if(isSpaceKeyPushed)return;
        if(Input.GetKey(KeyCode.Space))
        {
            spaceKeyPushTime -= Time.deltaTime;
            SpaceKeyImage.fillAmount = 1f - (spaceKeyPushTime / 0.5f);
            if(spaceKeyPushTime <= 0f)
            {
                isSpaceKeyPushed = true;
                switch (currentGameMode)
                {
                    case GameMode.Single:
                        _ = sceneChanger.ChangeSceneAsync("SingleBattle");
                        break;
                    case GameMode.Versus:
                        _ = sceneChanger.ChangeSceneAsync("MultiBattle");
                        break;
                }
            }
        }
        else
        {
            spaceKeyPushTime += Time.deltaTime;
            if(spaceKeyPushTime >= 0.5f)
            {
                spaceKeyPushTime = 0.5f;
            }
            SpaceKeyImage.fillAmount = 1f - (spaceKeyPushTime / 0.5f);
        }
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
        else if (Input.GetKeyDown(KeyCode.W))
        {
            playerCharacter += 2;
            if ((int)playerCharacter > 3)
            {
                playerCharacter = (Character)((int)playerCharacter - 4);
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
        else if (Input.GetKeyDown(KeyCode.S))
        {
            playerCharacter -= 2;
            if ((int)playerCharacter < 0)
            {
                playerCharacter = (Character)((int)playerCharacter + 4);
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
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                player2Character += 2;
                if ((int)player2Character > 3)
                {
                    player2Character = (Character)((int)player2Character - 4);
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
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                player2Character -= 2;
                if ((int)player2Character < 0)
                {
                    player2Character = (Character)((int)player2Character + 4);
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
