using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Piece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private PieceType pieceType;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Image uiImage;
    [SerializeField] private RectTransform rectTransform;

    public PieceType Type => pieceType;
    public bool IsUI => uiImage != null;
    public RectTransform RectTransform => rectTransform;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (uiImage == null)
        {
            uiImage = GetComponent<Image>();
        }

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public void Initialize(PieceType type, Sprite sprite)
    {
        pieceType = type;
        ApplySprite(sprite);
    }

    public void ApplySprite(Sprite sprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }

        if (uiImage != null)
        {
            uiImage.sprite = sprite;
        }
    }

    public void ApplyUISize(Vector2 size)
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = size;
        }
    }

    public void ApplyUIPosition(Vector2 anchoredPosition)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = anchoredPosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
public enum PieceType
{
    Red,
    Blue,
    Green,
    Yellow,
    Purple
}
