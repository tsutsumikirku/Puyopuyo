using UnityEngine;

[CreateAssetMenu(fileName = "PuyoSpriteSet", menuName = "Puyopuyo/Puyo Sprite Set")]
public class PuyoSpriteSet : ScriptableObject
{
    [SerializeField] private Sprite red;
    [SerializeField] private Sprite blue;
    [SerializeField] private Sprite green;
    [SerializeField] private Sprite yellow;
    [SerializeField] private Sprite ojama;

    private static readonly PieceType[] Types =
    {
        PieceType.Red,
        PieceType.Blue,
        PieceType.Green,
        PieceType.Yellow
    };

    public Sprite GetSprite(PieceType type)
    {
        return type switch
        {
            PieceType.Red => red,
            PieceType.Blue => blue,
            PieceType.Green => green,
            PieceType.Yellow => yellow,
            PieceType.Ojama => ojama,
            _ => null
        };
    }

    public PieceType GetRandomType()
    {
        if (Types.Length == 0)
        {
            return PieceType.Red;
        }

        return Types[Random.Range(0, Types.Length)];
    }

}
