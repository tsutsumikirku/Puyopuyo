using System.Collections.Generic;
using UnityEngine;

public class PuzzleBord : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 12;
    [SerializeField] private float spacing = 1.1f;
    [SerializeField] private Vector2 origin = Vector2.zero;
    [SerializeField] private Vector2 uiCellSize = new Vector2(64f, 64f);

    [Header("Piece Settings")]
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private PuyoSpriteSet spriteSet;

    [Header("Fall Settings")]
    [SerializeField] private float fallInterval = 0.8f;
    [SerializeField] private float softDropInterval = 0.05f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode softDropKey = KeyCode.DownArrow;
    [SerializeField] private KeyCode hardDropKey = KeyCode.Space;
    [SerializeField] private KeyCode rotateClockwiseKey = KeyCode.X;
    [SerializeField] private KeyCode rotateCounterClockwiseKey = KeyCode.Z;

    private Piece[,] board;
    private ActivePair activePair;
    private float fallTimer;
    private bool gameOver;

    private void Start()
    {
        InitializeBoard();
    }

    private void Update()
    {
        if (gameOver || activePair.PivotPiece == null)
        {
            return;
        }

        HandleInput();
        HandleFall();
    }

    [ContextMenu("Restart Game")]
    public void RestartGame()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        ClearBoard();

        if (height < 2 || width < 1)
        {
            Debug.LogWarning("PuzzleBord requires a width of at least 1 and height of at least 2.");
            return;
        }

        if (piecePrefab == null || spriteSet == null)
        {
            Debug.LogWarning("PuzzleBord requires a piece prefab and sprite set to start the game.");
            return;
        }

        board = new Piece[width, height];
        fallTimer = 0f;
        gameOver = false;
        SpawnPair();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(moveLeftKey))
        {
            TryMoveActive(Vector2Int.left);
        }

        if (Input.GetKeyDown(moveRightKey))
        {
            TryMoveActive(Vector2Int.right);
        }

        if (Input.GetKeyDown(hardDropKey))
        {
            while (TryMoveActive(Vector2Int.down))
            {
            }

            LockActivePair();
            return;
        }

        if (Input.GetKeyDown(rotateClockwiseKey))
        {
            TryRotateActive(true);
        }

        if (Input.GetKeyDown(rotateCounterClockwiseKey))
        {
            TryRotateActive(false);
        }
    }

    private void HandleFall()
    {
        fallTimer += Time.deltaTime;
        float interval = Input.GetKey(softDropKey) ? softDropInterval : fallInterval;

        if (fallTimer < interval)
        {
            return;
        }

        fallTimer = 0f;
        if (!TryMoveActive(Vector2Int.down))
        {
            LockActivePair();
        }
    }

    private void ClearBoard()
    {
        if (board != null)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    if (board[x, y] != null)
                    {
                        DestroyImmediate(board[x, y].gameObject);
                    }
                }
            }
        }

        if (activePair.PivotPiece != null)
        {
            DestroyImmediate(activePair.PivotPiece.gameObject);
        }

        if (activePair.ChildPiece != null)
        {
            DestroyImmediate(activePair.ChildPiece.gameObject);
        }

        activePair = default;
    }

    private void SpawnPair()
    {
        Vector2Int pivotPosition = new Vector2Int(width / 2, height - 2);
        Vector2Int childPosition = pivotPosition + Vector2Int.up;

        if (!IsCellEmpty(pivotPosition) || !IsCellEmpty(childPosition))
        {
            gameOver = true;
            Debug.LogWarning("Game Over: spawn position blocked.");
            return;
        }

        PieceType pivotType = spriteSet.GetRandomType();
        PieceType childType = spriteSet.GetRandomType();
        Piece pivotPiece = CreatePiece(pivotPosition, pivotType);
        Piece childPiece = CreatePiece(childPosition, childType);

        activePair = new ActivePair
        {
            Pivot = pivotPosition,
            Offset = Vector2Int.up,
            PivotPiece = pivotPiece,
            ChildPiece = childPiece
        };
    }

    private Piece CreatePiece(Vector2Int gridPosition, PieceType type)
    {
        Piece piece = Instantiate(piecePrefab, transform, false);
        piece.Initialize(type, spriteSet.GetSprite(type));
        ApplyGridPosition(piece, gridPosition);
        return piece;
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(origin.x + gridPosition.x * spacing, origin.y + gridPosition.y * spacing, 0f);
    }

    private Vector2 GridToUI(Vector2Int gridPosition)
    {
        return new Vector2(origin.x + gridPosition.x * uiCellSize.x, origin.y + gridPosition.y * uiCellSize.y);
    }

    private void ApplyGridPosition(Piece piece, Vector2Int gridPosition)
    {
        if (piece == null)
        {
            return;
        }

        if (piece.IsUI)
        {
            piece.ApplyUISize(uiCellSize);
            piece.ApplyUIPosition(GridToUI(gridPosition));
        }
        else
        {
            piece.transform.position = GridToWorld(gridPosition);
        }
    }

    private bool TryMoveActive(Vector2Int delta)
    {
        Vector2Int newPivot = activePair.Pivot + delta;
        Vector2Int newChild = activePair.Pivot + activePair.Offset + delta;

        if (!IsPositionValid(newPivot, newChild))
        {
            return false;
        }

        activePair.Pivot = newPivot;
        UpdateActiveWorldPositions();
        return true;
    }

    private void TryRotateActive(bool clockwise)
    {
        Vector2Int rotatedOffset = clockwise
            ? new Vector2Int(-activePair.Offset.y, activePair.Offset.x)
            : new Vector2Int(activePair.Offset.y, -activePair.Offset.x);

        Vector2Int[] kickTests =
        {
            Vector2Int.zero,
            Vector2Int.right,
            Vector2Int.left,
            Vector2Int.up
        };

        foreach (Vector2Int kick in kickTests)
        {
            Vector2Int newPivot = activePair.Pivot + kick;
            Vector2Int newChild = newPivot + rotatedOffset;
            if (IsPositionValid(newPivot, newChild))
            {
                activePair.Pivot = newPivot;
                activePair.Offset = rotatedOffset;
                UpdateActiveWorldPositions();
                return;
            }
        }
    }

    private bool IsPositionValid(Vector2Int pivot, Vector2Int child)
    {
        return IsCellEmpty(pivot) && IsCellEmpty(child);
    }

    private bool IsCellEmpty(Vector2Int position)
    {
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
        {
            return false;
        }

        return board[position.x, position.y] == null;
    }

    private void UpdateActiveWorldPositions()
    {
        if (activePair.PivotPiece == null || activePair.ChildPiece == null)
        {
            return;
        }

        ApplyGridPosition(activePair.PivotPiece, activePair.Pivot);
        ApplyGridPosition(activePair.ChildPiece, activePair.Pivot + activePair.Offset);
    }

    private void LockActivePair()
    {
        Vector2Int pivotPosition = activePair.Pivot;
        Vector2Int childPosition = activePair.Pivot + activePair.Offset;
        board[pivotPosition.x, pivotPosition.y] = activePair.PivotPiece;
        board[childPosition.x, childPosition.y] = activePair.ChildPiece;
        activePair = default;

        ResolveBoard();
        SpawnPair();
    }

    private void ResolveBoard()
    {
        bool cleared;
        do
        {
            cleared = ClearMatches();
            if (cleared)
            {
                CollapseBoard();
            }
        } while (cleared);
    }

    private bool ClearMatches()
    {
        bool[,] visited = new bool[width, height];
        bool[,] toClear = new bool[width, height];
        bool anyCleared = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == null || visited[x, y])
                {
                    continue;
                }

                List<Vector2Int> group = FloodFillGroup(new Vector2Int(x, y), visited);
                if (group.Count >= 4)
                {
                    anyCleared = true;
                    foreach (Vector2Int cell in group)
                    {
                        toClear[cell.x, cell.y] = true;
                    }
                }
            }
        }

        if (!anyCleared)
        {
            return false;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (toClear[x, y])
                {
                    Destroy(board[x, y].gameObject);
                    board[x, y] = null;
                }
            }
        }

        return true;
    }

    private List<Vector2Int> FloodFillGroup(Vector2Int start, bool[,] visited)
    {
        List<Vector2Int> group = new List<Vector2Int>();
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        Piece startPiece = board[start.x, start.y];
        if (startPiece == null)
        {
            return group;
        }

        PieceType type = startPiece.Type;
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            if (current.x < 0 || current.x >= width || current.y < 0 || current.y >= height)
            {
                continue;
            }

            if (visited[current.x, current.y])
            {
                continue;
            }

            Piece piece = board[current.x, current.y];
            if (piece == null || piece.Type != type)
            {
                continue;
            }

            visited[current.x, current.y] = true;
            group.Add(current);

            foreach (Vector2Int direction in directions)
            {
                stack.Push(current + direction);
            }
        }

        return group;
    }

    private void CollapseBoard()
    {
        for (int x = 0; x < width; x++)
        {
            int writeRow = 0;
            for (int y = 0; y < height; y++)
            {
                Piece piece = board[x, y];
                if (piece == null)
                {
                    continue;
                }

                if (writeRow != y)
                {
                    board[x, y] = null;
                    board[x, writeRow] = piece;
                    ApplyGridPosition(piece, new Vector2Int(x, writeRow));
                }

                writeRow++;
            }
        }
    }

    private struct ActivePair
    {
        public Vector2Int Pivot;
        public Vector2Int Offset;
        public Piece PivotPiece;
        public Piece ChildPiece;
    }
}
