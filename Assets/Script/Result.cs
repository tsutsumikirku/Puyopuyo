using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YourMaidTools;
using UnityEngine.SceneManagement;
using TMPro;

public class Result : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI Player1Text;
    [SerializeField] private TextMeshProUGUI Player2Text;
    [SerializeField] private TitleButton[] titleButton;
    int selectionButton = 0;
    void Start()
    {
        Player1Text.text = "Player 1 Score: " + GameManager.instance.playerOneScore  +
                           "Combo: " + GameManager.instance.playerOneCombo;
        if (GameManager.instance.currentGameMode == GameMode.Single) return;
        Player2Text.gameObject.SetActive(true);
        Player2Text.text = "Player 2 Score: " + GameManager.instance.playerTwoScore +  
                           "Combo: " + GameManager.instance.playerTwoCombo;
        titleButton[selectionButton].Select();
        for (int i = 0; i < titleButton.Length; i++)
        {
            if(i == selectionButton)
            {
                titleButton[i].Select();
            }
            else
            {
                titleButton[i].Unselect();
            }
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectionButton++;
            if(selectionButton > 2)
            {
                selectionButton = 0;
            }
            for (int i = 0; i < titleButton.Length; i++)
            {
                if(i == selectionButton)
                {
                    titleButton[i].Select();
                }
                else
                {
                    titleButton[i].Unselect();
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectionButton--;
            if(selectionButton < 0)
            {
                selectionButton = 2;
            }
            for (int i = 0; i < titleButton.Length; i++)
            {
                if(i == selectionButton)
                {
                    titleButton[i].Select();
                }
                else
                {
                    titleButton[i].Unselect();
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            switch (selectionButton)
            {
                case 0:
                if(GameManager.instance.currentGameMode == GameMode.Single)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("SingleBattle");
                else if(GameManager.instance.currentGameMode == GameMode.Versus)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("MultiBattle");
                break;
            case 1:
                if(GameManager.instance.currentGameMode == GameMode.Single)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("SingleCharacterSelect");
                else if(GameManager.instance.currentGameMode == GameMode.Versus)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("MultiCharacterSelect");
                break;
            case 2:
                FindAnyObjectByType<SceneChanger1>().ChangeScene("Title");
                break;
            }
        }
    }
    public void ClickButton(int buttonIndex)
    {
        switch (buttonIndex)
        {
            case 0:
                if(GameManager.instance.currentGameMode == GameMode.Single)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("SingleBattle");
                else if(GameManager.instance.currentGameMode == GameMode.Versus)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("MultiBattle");
                break;
            case 1:
                if(GameManager.instance.currentGameMode == GameMode.Single)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("SingleCharacterSelect");
                else if(GameManager.instance.currentGameMode == GameMode.Versus)
                FindAnyObjectByType<SceneChanger1>().ChangeScene("MultiCharacterSelect");
                break;
            case 2:
                FindAnyObjectByType<SceneChanger1>().ChangeScene("Title");
                break;
        }
    }   
}
