using DG.Tweening;
using UnityEngine;
using YourMaidTools;

public class TitleButton : MonoBehaviour
{
    [SerializeField] YMAnimationBase[] selectAnimations;

    RectTransform rect;
    Vector3 originalScale;
    Vector2 originalAnchoredPosition;
    Vector2 originalSizeDelta;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            Debug.LogError("RectTransform が見つかりません。");
            return;
        }
        originalScale = rect.localScale;
        originalAnchoredPosition = rect.anchoredPosition;
        originalSizeDelta = rect.sizeDelta;
    }

    public void Select()
    {
        Debug.Log("Select");
        if (selectAnimations != null)
        {
            foreach (var anim in selectAnimations)
            {
                if (anim != null) anim.PlayAnimation(false);
            }
        }

        rect.DOKill();
        rect.DOSizeDelta(new Vector2(originalSizeDelta.x * 1.2f, originalSizeDelta.y), 0.2f);
        rect.DOAnchorPosX(originalAnchoredPosition.x + 20f, 0.2f);
    }

    public void Unselect()
    {
        Debug.Log("Unselect");
        if (selectAnimations != null)
        {
            foreach (var anim in selectAnimations)
            {
                if (anim != null) anim.ReverseAnimation(false);
            }
        }

        rect.DOKill();
        rect.DOSizeDelta(originalSizeDelta, 0.2f);
        rect.DOAnchorPosX(originalAnchoredPosition.x, 0.2f);
    }
}
