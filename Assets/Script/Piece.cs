using UnityEngine;
using UnityEngine.EventSystems;

public class Piece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

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
    Corner,
    Edge,
    Center
}
