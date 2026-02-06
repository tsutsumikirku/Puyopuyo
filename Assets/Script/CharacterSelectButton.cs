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
    public void OnePlayerSelect()
    {
        isOnePlayerSelected = true;
        characterImage.sprite = selectCharacterTexture;
    }
    public void OnePlayerUnselect()
    {
        isOnePlayerSelected = false;
        if (isTwoPlayerSelected) return;
        characterImage.sprite = unselectCharacterTexture;
    }
    public void TwoPlayerSelect()
    {
        isTwoPlayerSelected = true;
        characterImage.sprite = selectCharacterTexture;
    }
    public void TwoPlayerUnselect()
    {
        isTwoPlayerSelected = false;
        if (isOnePlayerSelected) return;
        characterImage.sprite = unselectCharacterTexture;
    }
}
