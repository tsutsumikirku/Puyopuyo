using UnityEngine.UI;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Image playerImage;
    [SerializeField] private Sprite[] characterSprites;
    [SerializeField] private Image bordImage;
    [SerializeField] private Sprite[] bordSprites;
    void Start()
    {
        switch (player)
        {
            case Player.PlayerOne:
                playerImage.sprite = characterSprites[(int)GameManager.instance.playerCharacter];
                bordImage.sprite = bordSprites[(int)GameManager.instance.playerCharacter];
                break;
            case Player.PlayerTwo:
                playerImage.sprite = characterSprites[(int)GameManager.instance.player2Character];
                bordImage.sprite = bordSprites[(int)GameManager.instance.player2Character];
                break;
        }
    }
}
public enum Player
{
    PlayerOne,
    PlayerTwo
}
