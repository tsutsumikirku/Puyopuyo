using UnityEngine;

[CreateAssetMenu(fileName = "PuyoSpriteSet", menuName = "Puyopuyo/Puyo Sprite Set")]
public class PuyoSpriteSet : ScriptableObject
{
    [SerializeField] private Sprite red;
    [SerializeField] private Sprite blue;
    [SerializeField] private Sprite green;
    [SerializeField] private Sprite yellow;
    [SerializeField] private Sprite purple;
    [SerializeField] private Sprite ojama;
    [SerializeField] private Sprite[] redConnected = new Sprite[16];
    [SerializeField] private Sprite[] blueConnected = new Sprite[16];
    [SerializeField] private Sprite[] greenConnected = new Sprite[16];
    [SerializeField] private Sprite[] yellowConnected = new Sprite[16];
    [SerializeField] private Sprite[] purpleConnected = new Sprite[16];

    private static readonly PieceType[] Types =
    {
        PieceType.Red,
        PieceType.Blue,
        PieceType.Green,
        PieceType.Yellow,
        PieceType.Purple
    };

    public Sprite GetSprite(PieceType type)
    {
        return type switch
        {
            PieceType.Red => red,
            PieceType.Blue => blue,
            PieceType.Green => green,
            PieceType.Yellow => yellow,
            PieceType.Purple => purple,
            PieceType.Ojama => ojama,
            _ => null
        };
    }

    public Sprite GetSprite(PieceType type, PuyoConnectionMask connections)
    {
        int mask = (int)connections;
        Sprite[] variants = GetConnectedSprites(type);
        if (variants != null && mask >= 0 && mask < variants.Length)
        {
            Sprite connectedSprite = variants[mask];
            if (connectedSprite != null)
            {
                return connectedSprite;
            }
        }

        return GetSprite(type);
    }

    public PieceType GetRandomType()
    {
        if (Types.Length == 0)
        {
            return PieceType.Red;
        }

        return Types[Random.Range(0, Types.Length)];
    }

    private Sprite[] GetConnectedSprites(PieceType type)
    {
        return type switch
        {
            PieceType.Red => redConnected,
            PieceType.Blue => blueConnected,
            PieceType.Green => greenConnected,
            PieceType.Yellow => yellowConnected,
            PieceType.Purple => purpleConnected,
            _ => null
        };
    }

    private void OnValidate()
    {
        EnsureArraySize(ref redConnected);
        EnsureArraySize(ref blueConnected);
        EnsureArraySize(ref greenConnected);
        EnsureArraySize(ref yellowConnected);
        EnsureArraySize(ref purpleConnected);
    }

    private static void EnsureArraySize(ref Sprite[] sprites)
    {
        if (sprites == null || sprites.Length != 16)
        {
            Sprite[] resized = new Sprite[16];
            if (sprites != null)
            {
                int length = Mathf.Min(sprites.Length, resized.Length);
                for (int i = 0; i < length; i++)
                {
                    resized[i] = sprites[i];
                }
            }

            sprites = resized;
        }
    }
}

[System.Flags]
public enum PuyoConnectionMask
{
    None = 0,
    Up = 1 << 0,
    Down = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}
