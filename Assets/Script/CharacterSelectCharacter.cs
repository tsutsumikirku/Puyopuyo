using DG.Tweening;
using UnityEngine;

public class CharacterSelectCharacter : MonoBehaviour
{
    public GameObject[] characters;
    public AudioSourceSet[] selectSE;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource voiceAudioSource;

    // 元のスケールを保持しておく配列
    private Vector3[] defaultScales;

    void Awake()
    {
        defaultScales = new Vector3[characters.Length];
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] != null)
                defaultScales[i] = characters[i].transform.localScale;
            else
                defaultScales[i] = Vector3.one;
        }
    }

    public void SilentCharacterSelect(int index)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i] == null) continue;

            // 実行中のTweenを止めてデフォルトスケールに戻す
            characters[i].transform.DOKill();
            characters[i].transform.localScale = defaultScales[i];

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
            if (characters[i] == null) continue;

            if (i == index)
            {
                characters[i].SetActive(true);
                characters[i].transform.DOKill();
                var targetScale = defaultScales[i];
                characters[i].transform.localScale = Vector3.zero;
                characters[i].transform.DOScale(targetScale, 0.3f).SetEase(Ease.OutBack);

                audioSource.clip = selectSE[i].systemSound;
                voiceAudioSource.clip = selectSE[i].voiceSound;
                voiceAudioSource.Play();
                audioSource.Play();
            }
            else
            {
                characters[i].transform.DOKill();
                characters[i].transform.localScale = defaultScales[i];
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