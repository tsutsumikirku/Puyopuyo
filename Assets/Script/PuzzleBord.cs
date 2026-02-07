using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PuzzleBord : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 12;
    [SerializeField] private float spacing = 1.1f;
    [SerializeField] private Vector2 origin = Vector2.zero;
    [SerializeField] private Vector2 uiCellSize = new Vector2(64f, 64f);
    [SerializeField] private bool centerBoardOnTransform = true;

    [Header("Piece Settings")]
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private PuyoSpriteSet spriteSet;
    [SerializeField] private PuyoSpriteSet clearEffectSpriteSet;

    [Header("Next Preview Settings")]
    [SerializeField] private Transform nextPreviewRoot;
    [SerializeField] private Vector2 nextPreviewCellSize = new Vector2(48f, 48f);
    [SerializeField] private float nextPreviewSpacing = 0.9f;
    [SerializeField] private bool showNextPreview = true;
    [SerializeField] private bool autoFitNextPreviewCellSize = true;

    [Header("Fall Settings")]
    [SerializeField] private float fallInterval = 0.8f;
    [SerializeField] private float softDropInterval = 0.05f;
    [SerializeField] private float fallAnimationDuration = 0.15f;
    [SerializeField] private float lateralMoveAnimationDuration = 0.08f;
    [SerializeField] private float rotateAnimationDuration = 0.08f;
    [SerializeField] private float landingBounceDuration = 0.12f;
    [SerializeField] private Vector3 landingBounceScale = new Vector3(1.1f, 0.9f, 1f);
    [SerializeField] private float clearBlinkDuration = 0.3f;
    [SerializeField] private float clearBlinkInterval = 0.08f;
    [SerializeField] private float garbageDropDelay = 0.2f;
    [SerializeField] private PuyoClearEffect clearEffectPrefab;
    [SerializeField] private PuzzleBord opponentBoard;

    [Header("Frame Settings")]
    [SerializeField] private Sprite frameSprite;
    [SerializeField] private Color frameColor = Color.white;
    [SerializeField] private bool showFrameTiles = false;

    [Header("Input Settings")]
    [SerializeField] private KeyCode moveLeftKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode moveRightKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode softDropKey = KeyCode.DownArrow;
    [SerializeField] private KeyCode hardDropKey = KeyCode.Space;
    [SerializeField] private KeyCode rotateClockwiseKey = KeyCode.X;
    [SerializeField] private KeyCode rotateCounterClockwiseKey = KeyCode.Z;

    [Header("Combo Settings")]
    [SerializeField] TextMeshProUGUI     comboText;
    [SerializeField] AudioClip[] comboAudioClip;
    [SerializeField] AudioClip chainSE;
    [SerializeField] float durationToShowComboText = 0.5f;
    private Tween tween;
    private string baseComboText;
    private Vector3 beforeScale;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip fallSE;

    private Piece[,] board;
    private ActivePair activePair;
    private PieceType nextPivotType;
    private PieceType nextChildType;
    private bool hasNextPair;
    private Piece nextPreviewPivot;
    private Piece nextPreviewChild;
    private float fallTimer;
    private bool gameOver;
    private bool isResolving;
    private bool isRunning;
    private int pendingGarbage;
    private int currentChainCount;
    private readonly Dictionary<Piece, Coroutine> moveCoroutines = new Dictionary<Piece, Coroutine>();
    private readonly Dictionary<Piece, Coroutine> bounceCoroutines = new Dictionary<Piece, Coroutine>();
    private readonly List<GameObject> frameTiles = new List<GameObject>();

    public System.Action OnGameOver;
    public System.Action<int> OnChainTriggered;

    private void Start()
    {
        UpdateUISizing();
        baseComboText = comboText.text;
        beforeScale = comboText.transform.localScale;
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        if (gameOver || isResolving || activePair.PivotPiece == null)
        {
            return;
        }

        HandleInput();
        HandleFall();
    }

    [ContextMenu("Restart Game")]
    public void RestartGame()
    {
        StartGame();
    }

    public void StartGame()
    {
        isRunning = true;
        InitializeBoard();
    }

    public void StopGame()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;
        isResolving = true;
        StopAllCoroutines();
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

        UpdateUISizing();

        board = new Piece[width, height];
        fallTimer = 0f;
        gameOver = false;
        isResolving = false;
        pendingGarbage = 0;
        currentChainCount = 0;
        BuildFrameTiles();
        PrepareNextPair();
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
        if (!TryMoveActive(Vector2Int.down, fallAnimationDuration))
        {
            LockActivePair();
        }
    }

    private void ClearBoard()
    {
        ClearFrameTiles();
        ClearNextPreview();

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
        hasNextPair = false;
    }

    private void SpawnPair()
    {
        if (!hasNextPair)
        {
            PrepareNextPair();
        }

        Vector2Int pivotPosition = new Vector2Int(width / 2, height - 1);
        Vector2Int childPosition = pivotPosition + Vector2Int.up;

        if (!IsCellEmpty(pivotPosition) || (childPosition.y < height && !IsCellEmpty(childPosition)))
        {
            SetGameOver("Game Over: spawn position blocked.");
            return;
        }

        PieceType pivotType = nextPivotType;
        PieceType childType = nextChildType;
        Piece pivotPiece = CreatePiece(pivotPosition, pivotType);
        Piece childPiece = CreatePiece(childPosition, childType);

        activePair = new ActivePair
        {
            Pivot = pivotPosition,
            Offset = Vector2Int.up,
            PivotPiece = pivotPiece,
            ChildPiece = childPiece
        };

        PrepareNextPair();
        RefreshAllSprites();
    }

    private void PrepareNextPair()
    {
        if (spriteSet == null)
        {
            return;
        }

        nextPivotType = spriteSet.GetRandomType();
        nextChildType = spriteSet.GetRandomType();
        hasNextPair = true;
        UpdateNextPreview();
    }

    private void UpdateNextPreview()
    {
        if (!showNextPreview || nextPreviewRoot == null || piecePrefab == null || spriteSet == null)
        {
            ClearNextPreview();
            return;
        }

        if (autoFitNextPreviewCellSize)
        {
            UpdateNextPreviewCellSize();
        }

        if (nextPreviewPivot == null)
        {
            nextPreviewPivot = CreatePreviewPiece();
        }

        if (nextPreviewChild == null)
        {
            nextPreviewChild = CreatePreviewPiece();
        }

        nextPreviewPivot.Initialize(nextPivotType, spriteSet.GetSprite(nextPivotType));
        nextPreviewChild.Initialize(nextChildType, spriteSet.GetSprite(nextChildType));

        ApplyPreviewPosition(nextPreviewPivot, Vector2Int.zero);
        ApplyPreviewPosition(nextPreviewChild, Vector2Int.up);
    }

    private void UpdateNextPreviewCellSize()
    {
        if (nextPreviewRoot == null)
        {
            return;
        }

        if (nextPreviewRoot.TryGetComponent(out RectTransform rectTransform))
        {
            Vector2 size = rectTransform.rect.size;
            if (size.x > 0f && size.y > 0f)
            {
                float cellSize = Mathf.Min(size.x, size.y) * 0.5f;
                if (cellSize > 0f)
                {
                    nextPreviewCellSize = new Vector2(cellSize, cellSize);
                }
            }
        }
    }

    private Piece CreatePreviewPiece()
    {
        Piece piece = Instantiate(piecePrefab, nextPreviewRoot, false);
        piece.ApplySprite(null);
        return piece;
    }

    private void ApplyPreviewPosition(Piece piece, Vector2Int gridPosition)
    {
        if (piece == null)
        {
            return;
        }

        if (piece.IsUI)
        {
            piece.ApplyUISize(nextPreviewCellSize);
            Vector2 center = GetNextPreviewCenter();
            Vector2 anchored = new Vector2(gridPosition.x * nextPreviewCellSize.x, (gridPosition.y - 0.5f) * nextPreviewCellSize.y);
            piece.ApplyUIPosition(center + anchored);
        }
        else
        {
            Vector3 center = GetNextPreviewCenterWorld();
            Vector3 offset = new Vector3(gridPosition.x * nextPreviewSpacing, (gridPosition.y - 0.5f) * nextPreviewSpacing, 0f);
            piece.transform.localPosition = center + offset;
        }
    }

    private Vector2 GetNextPreviewCenter()
    {
        if (nextPreviewRoot != null && nextPreviewRoot.TryGetComponent(out RectTransform rectTransform))
        {
            return rectTransform.rect.center;
        }

        return Vector2.zero;
    }

    private Vector3 GetNextPreviewCenterWorld()
    {
        return nextPreviewRoot != null ? nextPreviewRoot.localPosition : Vector3.zero;
    }

    private void ClearNextPreview()
    {
        if (nextPreviewPivot != null)
        {
            DestroyImmediate(nextPreviewPivot.gameObject);
        }

        if (nextPreviewChild != null)
        {
            DestroyImmediate(nextPreviewChild.gameObject);
        }

        nextPreviewPivot = null;
        nextPreviewChild = null;
    }

    private Piece CreatePiece(Vector2Int gridPosition, PieceType type)
    {
        Piece piece = Instantiate(piecePrefab, transform, false);
        piece.Initialize(type, spriteSet.GetSprite(type));
        ApplyGridPosition(piece, gridPosition, false);
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

    private void ApplyGridPosition(Piece piece, Vector2Int gridPosition, bool animate)
    {
        if (piece == null)
        {
            return;
        }

        if (piece.IsUI)
        {
            piece.ApplyUISize(uiCellSize);
            ApplyUIPosition(piece, GridToUI(gridPosition), animate);
        }
        else
        {
            ApplyWorldPosition(piece, GridToWorld(gridPosition), animate);
        }
    }

    private void BuildFrameTiles()
    {
        ClearFrameTiles();

        if (!showFrameTiles || frameSprite == null)
        {
            return;
        }

        bool useUI = UseUIGrid();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int gridPosition = new Vector2Int(x, y);
                GameObject tile = useUI ? CreateFrameTileUI(gridPosition) : CreateFrameTileWorld(gridPosition);
                if (tile != null)
                {
                    frameTiles.Add(tile);
                }
            }
        }
    }

    private GameObject CreateFrameTileUI(Vector2Int gridPosition)
    {
        GameObject tile = new GameObject($"FrameTile_{gridPosition.x}_{gridPosition.y}", typeof(RectTransform), typeof(Image));
        tile.transform.SetParent(transform, false);

        Image image = tile.GetComponent<Image>();
        image.sprite = frameSprite;
        image.color = frameColor;
        image.preserveAspect = true;

        RectTransform rectTransform = tile.GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = uiCellSize;
        rectTransform.anchoredPosition = GridToUI(gridPosition);
        tile.transform.SetAsFirstSibling();
        return tile;
    }

    private GameObject CreateFrameTileWorld(Vector2Int gridPosition)
    {
        GameObject tile = new GameObject($"FrameTile_{gridPosition.x}_{gridPosition.y}", typeof(SpriteRenderer));
        tile.transform.SetParent(transform, false);

        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        renderer.sprite = frameSprite;
        renderer.color = frameColor;
        renderer.sortingOrder = -1;
        tile.transform.position = GridToWorld(gridPosition);
        return tile;
    }

    private void ClearFrameTiles()
    {
        for (int i = 0; i < frameTiles.Count; i++)
        {
            if (frameTiles[i] != null)
            {
                DestroyImmediate(frameTiles[i]);
            }
        }

        frameTiles.Clear();
    }

    private void ApplyGridPosition(Piece piece, Vector2Int gridPosition, float duration, bool bounce)
    {
        if (piece == null)
        {
            return;
        }

        if (piece.IsUI)
        {
            piece.ApplyUISize(uiCellSize);
            ApplyUIPosition(piece, GridToUI(gridPosition), duration, bounce);
        }
        else
        {
            ApplyWorldPosition(piece, GridToWorld(gridPosition), duration, bounce);
        }
    }

    private bool TryMoveActive(Vector2Int delta)
    {
        return TryMoveActive(delta, 0f);
    }

    private bool TryMoveActive(Vector2Int delta, float animationDuration)
    {
        Vector2Int newPivot = activePair.Pivot + delta;
        Vector2Int newChild = activePair.Pivot + activePair.Offset + delta;

        if (!IsPositionValid(newPivot, newChild))
        {
            return false;
        }

        activePair.Pivot = newPivot;
        bool shouldAnimate = animationDuration > 0f;
        if (!shouldAnimate && delta.y == 0 && lateralMoveAnimationDuration > 0f)
        {
            shouldAnimate = true;
            animationDuration = lateralMoveAnimationDuration;
        }

        if (shouldAnimate)
        {
            ApplyGridPosition(activePair.PivotPiece, activePair.Pivot, animationDuration, false);
            ApplyGridPosition(activePair.ChildPiece, activePair.Pivot + activePair.Offset, animationDuration, false);
        }
        else
        {
            UpdateActiveWorldPositions();
        }
        RefreshAllSprites();
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
                AnimateActivePositions(rotateAnimationDuration);
                RefreshAllSprites();
                return;
            }
        }

        if (activePair.Offset == Vector2Int.up || activePair.Offset == Vector2Int.down)
        {
            Vector2Int swappedPivot = activePair.Pivot + activePair.Offset;
            Vector2Int swappedChild = swappedPivot - activePair.Offset;
            if (IsPositionValid(swappedPivot, swappedChild))
            {
                activePair.Pivot = swappedPivot;
                activePair.Offset = -activePair.Offset;
                AnimateActivePositions(rotateAnimationDuration);
                RefreshAllSprites();
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

        ApplyGridPosition(activePair.PivotPiece, activePair.Pivot, false);
        ApplyGridPosition(activePair.ChildPiece, activePair.Pivot + activePair.Offset, false);
    }

    private void AnimateActivePositions(float duration)
    {
        if (activePair.PivotPiece == null || activePair.ChildPiece == null)
        {
            return;
        }

        ApplyGridPosition(activePair.PivotPiece, activePair.Pivot, duration, false);
        ApplyGridPosition(activePair.ChildPiece, activePair.Pivot + activePair.Offset, duration, false);
    }

    private void LockActivePair()
    {
        Vector2Int pivotPosition = activePair.Pivot;
        Vector2Int childPosition = activePair.Pivot + activePair.Offset;
        Piece pivotPiece = activePair.PivotPiece;
        Piece childPiece = activePair.ChildPiece;
        board[pivotPosition.x, pivotPosition.y] = pivotPiece;
        if (childPosition.y >= height)
        {
            SetGameOver("Game Over: piece locked above the board.");
            return;
        }

        board[childPosition.x, childPosition.y] = childPiece;
        activePair = default;

        RefreshAllSprites();
        PlayFallSE();
        StartBounceCoroutine(pivotPiece, pivotPiece != null && pivotPiece.IsUI);
        StartBounceCoroutine(childPiece, childPiece != null && childPiece.IsUI);
        StartCoroutine(ResolveAfterLockRoutine());
    }

    private System.Collections.IEnumerator ResolveAfterLockRoutine()
    {
        isResolving = true;

        bool moved = CollapseBoard(true);
        if (moved)
        {
            yield return new WaitForSeconds(fallAnimationDuration);
        }

        while (true)
        {
            List<List<Vector2Int>> groups = FindMatchGroups();
            if (groups.Count == 0)
            {
                break;
            }
            currentChainCount += CountDistinctClearTypes(groups);
            comboText.text = baseComboText.Replace("num", currentChainCount.ToString());
            tween?.Kill();
            comboText.gameObject.SetActive(true);
            comboText.transform.localScale = Vector3.zero;
            tween = comboText.transform.DOScale(beforeScale, durationToShowComboText).SetEase(Ease.OutBack).OnComplete(() =>
            {
                comboText.gameObject.SetActive(false);
            });
            GameManager.instance.PlaySE(chainSE);
            // if (currentChainCount - 1 < comboAudioClip.Length)
            // {
            //     AudioClip clip = comboAudioClip[currentChainCount - 1];
            //     if (clip != null)
            //     {
            //         GameManager.instance.PlaySE(clip);
            //     }
            // }
            // else
            // {
            //     AudioClip clip = comboAudioClip[comboAudioClip.Length - 1];
            //     if (clip != null)
            //     {
            //         GameManager.instance.PlaySE(clip);
            //     }
            // }
            int clearedThisChain = 0;
            foreach (List<Vector2Int> group in groups)
            {
                yield return StartCoroutine(BlinkMatches(group));
                clearedThisChain += ClearMatches(group);
            }

            int canceledGarbage = Mathf.Min(pendingGarbage, clearedThisChain);
            if (canceledGarbage > 0)
            {
                pendingGarbage -= canceledGarbage;
            }

            int sendGarbage = clearedThisChain - canceledGarbage;
            if (opponentBoard != null && sendGarbage > 0)
            {
                opponentBoard.ReceiveGarbage(sendGarbage);
            }

            bool collapsed = CollapseBoard(true);
            if (collapsed)
            {
                yield return new WaitForSeconds(fallAnimationDuration);
            }
        }

        if (pendingGarbage > 0)
        {
            yield return StartCoroutine(DropGarbageRoutine());
        }

        currentChainCount = 0;
        isResolving = false;
        SpawnPair();
    }

    private List<List<Vector2Int>> FindMatchGroups()
    {
        bool[,] visited = new bool[width, height];
        List<List<Vector2Int>> groups = new List<List<Vector2Int>>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (board[x, y] == null || visited[x, y])
                {
                    continue;
                }

                if (board[x, y].Type == PieceType.Ojama)
                {
                    continue;
                }

                List<Vector2Int> group = FloodFillGroup(new Vector2Int(x, y), visited);
                if (group.Count >= 4)
                {
                    groups.Add(group);
                }
            }
        }

        return groups;
    }

    private int CountDistinctClearTypes(List<List<Vector2Int>> groups)
    {
        HashSet<PieceType> types = new HashSet<PieceType>();
        foreach (List<Vector2Int> group in groups)
        {
            if (group.Count == 0)
            {
                continue;
            }

            Vector2Int cell = group[0];
            Piece piece = board[cell.x, cell.y];
            if (piece != null && piece.Type != PieceType.Ojama)
            {
                types.Add(piece.Type);
            }
        }

        return Mathf.Max(1, types.Count);
    }

    private int ClearMatches(List<Vector2Int> matches)
    {
        HashSet<Vector2Int> cellsToClear = new HashSet<Vector2Int>();
        foreach (Vector2Int cell in matches)
        {
            cellsToClear.Add(cell);
        }

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int cell in matches)
        {
            foreach (Vector2Int direction in directions)
            {
                Vector2Int adjacent = cell + direction;
                Piece adjacentPiece = GetPieceAt(adjacent);
                if (adjacentPiece != null && adjacentPiece.Type == PieceType.Ojama)
                {
                    cellsToClear.Add(adjacent);
                }
            }
        }

        foreach (Vector2Int cell in cellsToClear)
        {
            Piece piece = board[cell.x, cell.y];
            if (piece == null)
            {
                continue;
            }

            SpawnClearEffect(piece);
            Destroy(piece.gameObject);
            board[cell.x, cell.y] = null;
        }

        RefreshAllSprites();
        return cellsToClear.Count;
    }

    private System.Collections.IEnumerator BlinkMatches(List<Vector2Int> matches)
    {
        HashSet<Piece> pieces = new HashSet<Piece>();
        foreach (Vector2Int cell in matches)
        {
            Piece piece = board[cell.x, cell.y];
            if (piece != null)
            {
                pieces.Add(piece);
            }
        }

        float elapsed = 0f;
        bool visible = true;
        float interval = Mathf.Max(0.01f, clearBlinkInterval);
        while (elapsed < clearBlinkDuration)
        {
            visible = !visible;
            float alpha = visible ? 1f : 0.2f;
            foreach (Piece piece in pieces)
            {
                if (piece != null)
                {
                    piece.SetAlpha(alpha);
                }
            }

            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }

        foreach (Piece piece in pieces)
        {
            if (piece != null)
            {
                piece.SetAlpha(1f);
            }
        }
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
            if (piece == null || piece.Type != type || piece.Type == PieceType.Ojama)
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

    private bool CollapseBoard(bool animate)
    {
        bool moved = false;
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
                    ApplyGridPosition(piece, new Vector2Int(x, writeRow), animate);
                    moved = true;
                }

                writeRow++;
            }
        }

        if (moved)
        {
            RefreshAllSprites();
            PlayFallSE();
        }

        return moved;
    }

    private void PlayFallSE()
    {
        if (fallSE == null || GameManager.instance == null)
        {
            return;
        }

        GameManager.instance.PlaySE(fallSE);
    }

    private void ApplyUIPosition(Piece piece, Vector2 target, bool animate)
    {
        if (!animate || fallAnimationDuration <= 0f)
        {
            StopMoveCoroutine(piece);
            piece.ApplyUIPosition(target);
            return;
        }

        Vector2 start = piece.RectTransform != null ? piece.RectTransform.anchoredPosition : target;
        StartMoveCoroutine(piece, start, target, true, fallAnimationDuration, true);
    }

    private void ApplyWorldPosition(Piece piece, Vector3 target, bool animate)
    {
        if (!animate || fallAnimationDuration <= 0f)
        {
            StopMoveCoroutine(piece);
            piece.transform.position = target;
            return;
        }

        Vector3 start = piece.transform.position;
        StartMoveCoroutine(piece, start, target, false, fallAnimationDuration, true);
    }

    private void ApplyUIPosition(Piece piece, Vector2 target, float duration, bool bounce)
    {
        if (duration <= 0f)
        {
            StopMoveCoroutine(piece);
            piece.ApplyUIPosition(target);
            return;
        }

        Vector2 start = piece.RectTransform != null ? piece.RectTransform.anchoredPosition : target;
        StartMoveCoroutine(piece, start, target, true, duration, bounce);
    }

    private void ApplyWorldPosition(Piece piece, Vector3 target, float duration, bool bounce)
    {
        if (duration <= 0f)
        {
            StopMoveCoroutine(piece);
            piece.transform.position = target;
            return;
        }

        Vector3 start = piece.transform.position;
        StartMoveCoroutine(piece, start, target, false, duration, bounce);
    }

    private void StartMoveCoroutine(Piece piece, Vector3 start, Vector3 target, bool isUI, float duration, bool bounce)
    {
        if (moveCoroutines.TryGetValue(piece, out Coroutine existing) && existing != null)
        {
            StopCoroutine(existing);
        }

        Coroutine routine = StartCoroutine(MovePieceRoutine(piece, start, target, isUI, duration, bounce));
        moveCoroutines[piece] = routine;
    }

    private void StopMoveCoroutine(Piece piece)
    {
        if (piece == null)
        {
            return;
        }

        if (moveCoroutines.TryGetValue(piece, out Coroutine existing) && existing != null)
        {
            StopCoroutine(existing);
        }

        moveCoroutines.Remove(piece);
    }

    private System.Collections.IEnumerator MovePieceRoutine(Piece piece, Vector3 start, Vector3 target, bool isUI, float duration, bool bounce)
    {
        float elapsed = 0f;
        float moveDuration = Mathf.Max(0.01f, duration);

        while (elapsed < moveDuration)
        {
            if (piece == null)
            {
                yield break;
            }

            float t = elapsed / moveDuration;
            Vector3 position = Vector3.Lerp(start, target, t);
            if (isUI)
            {
                piece.ApplyUIPosition(new Vector2(position.x, position.y));
            }
            else
            {
                piece.transform.position = position;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (piece != null)
        {
            if (isUI)
            {
                piece.ApplyUIPosition(new Vector2(target.x, target.y));
            }
            else
            {
                piece.transform.position = target;
            }
        }

        if (bounce && landingBounceDuration > 0f)
        {
            StartBounceCoroutine(piece, isUI);
        }
    }

    private void StartBounceCoroutine(Piece piece, bool isUI)
    {
        if (piece == null)
        {
            return;
        }

        if (bounceCoroutines.TryGetValue(piece, out Coroutine existing) && existing != null)
        {
            StopCoroutine(existing);
        }

        Coroutine routine = StartCoroutine(BouncePieceRoutine(piece, isUI));
        bounceCoroutines[piece] = routine;
    }

    private System.Collections.IEnumerator BouncePieceRoutine(Piece piece, bool isUI)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, landingBounceDuration);
        Vector3 baseScale = Vector3.one;

        if (isUI && piece.RectTransform != null)
        {
            baseScale = piece.RectTransform.localScale;
        }
        else if (!isUI)
        {
            baseScale = piece.transform.localScale;
        }

        while (elapsed < duration)
        {
            if (piece == null)
            {
                yield break;
            }

            float t = elapsed / duration;
            float phase = t < 0.5f ? t * 2f : (t - 0.5f) * 2f;
            Vector3 targetScale = t < 0.5f
                ? Vector3.Lerp(baseScale, Vector3.Scale(baseScale, landingBounceScale), phase)
                : Vector3.Lerp(Vector3.Scale(baseScale, landingBounceScale), baseScale, phase);

            if (isUI && piece.RectTransform != null)
            {
                piece.RectTransform.localScale = targetScale;
            }
            else if (!isUI)
            {
                piece.transform.localScale = targetScale;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (piece != null)
        {
            if (isUI && piece.RectTransform != null)
            {
                piece.RectTransform.localScale = baseScale;
            }
            else if (!isUI)
            {
                piece.transform.localScale = baseScale;
            }
        }
    }

    private void SpawnClearEffect(Piece piece)
    {
        if (clearEffectPrefab == null || piece == null)
        {
            return;
        }

        Transform parent = piece.transform.parent;
        PuyoClearEffect effect = Instantiate(clearEffectPrefab, parent, false);
        PuyoSpriteSet spriteSource = clearEffectSpriteSet != null ? clearEffectSpriteSet : spriteSet;
        if (spriteSource != null)
        {
            effect.SetSprite(spriteSource.GetSprite(piece.Type));
        }

        if (piece.IsUI && piece.RectTransform != null)
        {
            effect.PlayUI(piece.RectTransform, uiCellSize);
        }
        else
        {
            effect.PlayWorld(piece.transform.position);
        }
    }

    private void RefreshAllSprites()
    {
        if (spriteSet == null || board == null)
        {
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Piece piece = board[x, y];
                if (piece == null)
                {
                    continue;
                }

                UpdatePieceSprite(piece, new Vector2Int(x, y));
            }
        }

        if (activePair.PivotPiece != null)
        {
            UpdatePieceSprite(activePair.PivotPiece, activePair.Pivot);
        }

        if (activePair.ChildPiece != null)
        {
            UpdatePieceSprite(activePair.ChildPiece, activePair.Pivot + activePair.Offset);
        }
    }

    private void UpdatePieceSprite(Piece piece, Vector2Int gridPosition)
    {
        if (piece == null || spriteSet == null)
        {
            return;
        }

        piece.ApplySprite(spriteSet.GetSprite(piece.Type));
    }

    public void ReceiveGarbage(int amount)
    {
        if (amount <= 0 || gameOver)
        {
            return;
        }

        pendingGarbage += amount;
    }

    private System.Collections.IEnumerator DropGarbageRoutine()
    {
        if (pendingGarbage <= 0 || board == null)
        {
            yield break;
        }

        if (garbageDropDelay > 0f)
        {
            yield return new WaitForSeconds(garbageDropDelay);
        }

        int remaining = pendingGarbage;
        pendingGarbage = 0;

        List<int> availableColumns = new List<int>();
        for (int x = 0; x < width; x++)
        {
            if (FindDropRow(x) >= 0)
            {
                availableColumns.Add(x);
            }
        }

        while (remaining > 0 && availableColumns.Count > 0)
        {
            int columnIndex = Random.Range(0, availableColumns.Count);
            int column = availableColumns[columnIndex];
            int row = FindDropRow(column);
            if (row < 0)
            {
                availableColumns.RemoveAt(columnIndex);
                continue;
            }

            Vector2Int position = new Vector2Int(column, row);
            Piece piece = CreatePiece(position, PieceType.Ojama);
            board[column, row] = piece;
            SetPieceStartAboveBoard(piece, column);
            ApplyGridPosition(piece, position, fallAnimationDuration, false);
            remaining--;

            if (FindDropRow(column) < 0)
            {
                availableColumns.RemoveAt(columnIndex);
            }
        }

        if (fallAnimationDuration > 0f)
        {
            yield return new WaitForSeconds(fallAnimationDuration);
        }

        if (remaining > 0)
        {
            SetGameOver("Game Over: no space for garbage puyos.");
        }

        RefreshAllSprites();
    }

    private int FindDropRow(int column)
    {
        if (column < 0 || column >= width)
        {
            return -1;
        }

        for (int y = 0; y < height; y++)
        {
            if (board[column, y] == null)
            {
                return y;
            }
        }

        return -1;
    }

    private void SetPieceStartAboveBoard(Piece piece, int column)
    {
        if (piece == null)
        {
            return;
        }

        Vector2Int spawnPosition = new Vector2Int(column, height);
        if (piece.IsUI)
        {
            piece.ApplyUISize(uiCellSize);
            piece.ApplyUIPosition(GridToUI(spawnPosition));
        }
        else
        {
            piece.transform.position = GridToWorld(spawnPosition);
        }
    }

    private void SetGameOver(string message)
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;
        Debug.LogWarning(message);
        isResolving = true;
        OnGameOver?.Invoke();
    }

    private Piece GetPieceAt(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.x >= width || gridPosition.y < 0 || gridPosition.y >= height)
        {
            return null;
        }

        if (activePair.PivotPiece != null && gridPosition == activePair.Pivot)
        {
            return activePair.PivotPiece;
        }

        if (activePair.ChildPiece != null && gridPosition == activePair.Pivot + activePair.Offset)
        {
            return activePair.ChildPiece;
        }

        return board != null ? board[gridPosition.x, gridPosition.y] : null;
    }

    private void AlignOriginToCenter()
    {
        Vector2 cellSize = UseUIGrid() ? uiCellSize : new Vector2(spacing, spacing);
        Vector2 center = GetBoardCenterPosition();
        origin = center - new Vector2((width - 1) * cellSize.x * 0.5f, (height - 1) * cellSize.y * 0.5f);
    }

    private Vector2 GetBoardCenterPosition()
    {
        if (UseUIGrid() && TryGetComponent(out RectTransform rectTransform))
        {
            return rectTransform.rect.center;
        }

        return transform.position;
    }

    private bool UseUIGrid()
    {
        if (piecePrefab == null)
        {
            return false;
        }

        return piecePrefab.IsUI || piecePrefab.GetComponent<RectTransform>() != null;
    }

    private void UpdateUISizing()
    {
        if (!UseUIGrid())
        {
            if (centerBoardOnTransform)
            {
                AlignOriginToCenter();
            }

            return;
        }

        if (!TryGetComponent(out RectTransform rectTransform))
        {
            if (centerBoardOnTransform)
            {
                AlignOriginToCenter();
            }

            return;
        }

        if (width > 0 && height > 0)
        {
            Rect rect = rectTransform.rect;
            float cellWidth = rect.width / width;
            float cellHeight = rect.height / height;
            if (cellWidth > 0f && cellHeight > 0f)
            {
                uiCellSize = new Vector2(cellWidth, cellHeight);
            }
        }

        AlignOriginToCenter();
    }

    private void OnValidate()
    {
        UpdateUISizing();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateUISizing();
    }

    private struct ActivePair
    {
        public Vector2Int Pivot;
        public Vector2Int Offset;
        public Piece PivotPiece;
        public Piece ChildPiece;
    }
}
