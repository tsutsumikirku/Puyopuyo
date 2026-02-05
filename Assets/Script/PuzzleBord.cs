using System.Collections.Generic;
using UnityEngine;

public class PuzzleBord : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 12;
    [SerializeField] private float spacing = 1.1f;
    [SerializeField] private Vector2 origin = Vector2.zero;

    [Header("Piece Settings")]
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private PuyoSpriteSet spriteSet;

    private readonly List<Piece> spawnedPieces = new List<Piece>();

    private void Start()
    {
        BuildBoard();
    }

    [ContextMenu("Build Board")]
    public void BuildBoard()
    {
        ClearBoard();

        if (piecePrefab == null || spriteSet == null)
        {
            Debug.LogWarning("PuzzleBord requires a piece prefab and sprite set to build the board.");
            return;
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = new Vector3(origin.x + x * spacing, origin.y + y * spacing, 0f);
                Piece piece = Instantiate(piecePrefab, position, Quaternion.identity, transform);
                PieceType type = spriteSet.GetRandomType();
                piece.Initialize(type, spriteSet.GetSprite(type));
                spawnedPieces.Add(piece);
            }
        }
    }

    public void RefreshSprites()
    {
        if (spriteSet == null)
        {
            return;
        }

        foreach (Piece piece in spawnedPieces)
        {
            piece.ApplySprite(spriteSet.GetSprite(piece.Type));
        }
    }

    private void ClearBoard()
    {
        foreach (Piece piece in spawnedPieces)
        {
            if (piece != null)
            {
                DestroyImmediate(piece.gameObject);
            }
        }

        spawnedPieces.Clear();
    }
}
