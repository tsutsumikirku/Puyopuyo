using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class InGame : MonoBehaviour
{
    [SerializeField] private Image ThreeImage;
    [SerializeField] private Image TwoImage;
    [SerializeField] private Image OneImage;
    [SerializeField] private Image StartImage;
    [SerializeField] private AudioClip ThreeVoice;
    [SerializeField] private AudioClip ThreeSE;
    [SerializeField] private AudioClip TwoVoice;
    [SerializeField] private AudioClip TwoSE;
    [SerializeField] private AudioClip OneVoice;
    [SerializeField] private AudioClip OneSE;
    [SerializeField] private AudioClip StartVoice;
    [SerializeField] private AudioClip StartSE;
    [SerializeField] float sizeDuration = 0.5f;
    [SerializeField] private PuzzleBord gameBord;
    [SerializeField] private PuzzleBord twoPlayerGameBord;
    async UniTask Start()
    {
        GameManager.instance.CurrentSceneType = SceneType.Stop;
        await UniTask.WaitForSeconds(1f);
        ThreeImage.gameObject.SetActive(true);
        var beforeScale = ThreeImage.transform.localScale;
        ThreeImage.transform.localScale = Vector3.zero;
        if(ThreeVoice)GameManager.instance.PlaySE(ThreeVoice);
        if(ThreeSE)GameManager.instance.PlaySE(ThreeSE);
        ThreeImage.transform.DOScale(beforeScale, sizeDuration).SetEase(Ease.OutBack);
        await UniTask.WaitForSeconds(1f);
        ThreeImage.gameObject.SetActive(false);
        if(TwoVoice)GameManager.instance.PlaySE(TwoVoice);
        if(TwoSE)GameManager.instance.PlaySE(TwoSE);
        var beforeScale2 = TwoImage.transform.localScale;
        TwoImage.transform.localScale = Vector3.zero;
        TwoImage.gameObject.SetActive(true);
        TwoImage.transform.DOScale(beforeScale2, sizeDuration).SetEase(Ease.OutBack);
        await UniTask.WaitForSeconds(1f);
        TwoImage.gameObject.SetActive(false);
        if(OneVoice)GameManager.instance.PlaySE(OneVoice);
        if(OneSE)GameManager.instance.PlaySE(OneSE);
        var beforeScale3 = OneImage.transform.localScale;
        OneImage.transform.localScale = Vector3.zero;
        OneImage.gameObject.SetActive(true);
        OneImage.transform.DOScale(beforeScale3, sizeDuration).SetEase(Ease.OutBack);
        await UniTask.WaitForSeconds(1f);
        OneImage.gameObject.SetActive(false);
        if(StartVoice)GameManager.instance.PlaySE(StartVoice);
        if(StartSE)GameManager.instance.PlaySE(StartSE);
        var beforeScale4 = StartImage.transform.localScale;
        StartImage.transform.localScale = Vector3.zero;
        StartImage.gameObject.SetActive(true);
        StartImage.transform.DOScale(beforeScale4, sizeDuration).SetEase(Ease.OutBack).OnComplete(async () =>
        {
            await UniTask.WaitForSeconds(0.5f);
            StartImage.gameObject.SetActive(false);
        });
        GameManager.instance.CurrentSceneType = SceneType.InGame;
        if(GameManager.instance.currentGameMode == GameMode.Single)
        {
            gameBord.StartGame();
        }
        else if(GameManager.instance.currentGameMode == GameMode.Versus)
        {
            gameBord.StartGame();
            twoPlayerGameBord.StartGame();
        }
    }
    private void PlayerOneGameOver()
    {
        
    }
    private void PlayerTwoGameOver()
    {
    }
}
