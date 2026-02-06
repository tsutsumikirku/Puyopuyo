using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Sprite selectCharacterTexture;
    [SerializeField] private Sprite unselectCharacterTexture;
    [SerializeField] private Image characterImage;
    [SerializeField] private Image OnePlayerSelectImage;
    [SerializeField] private Image TwoPlayerSelectImage;
    bool isOnePlayerSelected = false;
    bool isTwoPlayerSelected = false;
    private void Awake()
    {
        characterImage.sprite = unselectCharacterTexture;
        OnePlayerSelectImage.enabled = false;
        TwoPlayerSelectImage.enabled = false;
    }
    public void OnePlayerSelect()
    {
        isOnePlayerSelected = true;
        characterImage.sprite = selectCharacterTexture;
        OnePlayerSelectImage.enabled = true;
    }
    public void OnePlayerUnselect()
    {
        isOnePlayerSelected = false;
        OnePlayerSelectImage.enabled = false;
        if (isTwoPlayerSelected) return;
        characterImage.sprite = unselectCharacterTexture;
    }
    public void TwoPlayerSelect()
    {
        isTwoPlayerSelected = true;
        characterImage.sprite = selectCharacterTexture;
        TwoPlayerSelectImage.enabled = true;
    }
    public void TwoPlayerUnselect()
    {
        isTwoPlayerSelected = false;
        TwoPlayerSelectImage.enabled = false;
        if (isOnePlayerSelected) return;
        characterImage.sprite = unselectCharacterTexture;
    }
}
