public class Game
{
    readonly int[] board = new int[12 * 12];

    public FieldValue this[int i, int j]
    {
        get => ConvertToField(board[i + 12 * j]);
        set => ConvertFromField(value);
    }

    FieldValue ConvertToField(int hash)
    {
        return new FieldValue(
            false,
            false,
            false,
            hash % 10
        );
    }

    int ConvertFromField(FieldValue value)
    {
        return value.Number;
    }
}

public record FieldValue(
    bool HasBomb,
    bool HasFlag,
    bool IsOpen,
    int Number
);