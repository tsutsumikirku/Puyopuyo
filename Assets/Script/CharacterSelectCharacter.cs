using DG.Tweening;
using UnityEngine;

public class CharacterSelectCharacter : MonoBehaviour
{
    public GameObject[] characters;
    public AudioClip[] selectSE;
    public void SelectCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (i == index)
            {
                characters[i].SetActive(true);
                var beforescale = characters[i].transform.localScale;
                characters[i].transform.localScale = Vector3.zero;
                characters[i].transform.DOScale(beforescale, 0.3f).SetEase(Ease.OutBack);
                GameManager.instance.PlaySE(selectSE[i]);
            }
            else
            {
                characters[i].SetActive(false);
            }
        }
    }
}
