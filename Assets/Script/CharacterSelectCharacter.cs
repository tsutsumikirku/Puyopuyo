using DG.Tweening;
using UnityEngine;

public class CharacterSelectCharacter : MonoBehaviour
{
    public GameObject[] characters;
    public AudioSourceSet[] selectSE;
    [SerializeField]AudioSource audioSource;
    [SerializeField] AudioSource voiceAudioSource;
    public void SilentCharacterSelect(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (i == index)
            {
                characters[i].SetActive(true);
            }
            else
            {
                characters[i].SetActive(false);
            }
        }
    }
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
                audioSource.clip = selectSE[i].systemSound;
                voiceAudioSource.clip = selectSE[i].voiceSound;
                voiceAudioSource.Play();
                audioSource.Play();
            }
            else
            {
                characters[i].SetActive(false);
            }
        }
    }
}
[System.Serializable]
public class AudioSourceSet
{
    public AudioClip systemSound;
    public AudioClip voiceSound;
}