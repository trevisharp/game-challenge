public class Game
{
    const int length = 12;
    const int bombCode = 0b1_0000;
    const int flagCode = 0b10_0000;
    const int openCode = 0b100_0000;

    readonly int[] board = new int[length * length];

    public FieldValue this[int index]
    {
        get => ConvertToField(board[index]);
        set => board[index] = ConvertFromField(value);
    }

    public FieldValue this[int i, int j]
    {
        get => this[i + length * j];
        set => this[i + length * j] = value;
    }

    public void InitGame(int bombsCount = length)
    {
        var bombs = FillList(bombsCount, board.Length);
        InitBoardValues(bombs);
        RandomSafeClick();
    }

    public void Open(int i, int j)
    {
        var field = this[i, j];
        if (field.IsOpen)
            return;
        
        this[i, j] = field with { IsOpen = true };
        
        if (field.HasBomb)
            return;
        
        if (field.Number > 0)
            return;

        int x0 = int.Max(i - 1, 0), xf = int.Min(i + 1, length - 1);
        int y0 = int.Max(j - 1, 0), yf = int.Min(j + 1, length - 1);
        for (int x = x0; x <= xf; x++)
            for (int y = y0; y <= yf; y++)
                Open(x, y);
    }

    void RandomSafeClick()
    {
        var safeClick = Random.Shared.Next(board.Length);
        while (this[safeClick].HasBomb || this[safeClick].IsOpen)
            safeClick = Random.Shared.Next(board.Length);
        
        int x = safeClick % length;
        int y = safeClick / length;
        Open(x, y);
    }

    void InitBoardValues(List<int> bombs)
    {
        for (int j = 0; j < length; j++)
        {
            for (int i = 0; i < length; i++)
            {
                int index = i + length * j;
                this[i, j] = new FieldValue(
                    bombs.Contains(index),
                    false, false,
                    DiscoverValue(index, bombs)
                );
            }
        }
    }

    static int DiscoverValue(int value, List<int> bombs)
    {
        int count = 0;

        int[] deltas = [
            -length - 1, -length, -length + 1,
            -1, +1,
            length - 1, length, length + 1
        ];

        foreach (var delta in deltas)
        {
            if (bombs.Contains(value + delta))
                count++;
        }

        return count;
    }

    static List<int> FillList(int size, int max)
    {
        List<int> bombs = [];
        var rand = Random.Shared;

        while (bombs.Count < size)
        {
            int next = rand.Next(max);
            if (bombs.Contains(next))
                continue;

            bombs.Add(next);
        }

        return bombs;
    }

    static FieldValue ConvertToField(int hash)
    {
        return new FieldValue(
            (hash & bombCode) > 0,
            (hash & flagCode) > 0,
            (hash & openCode) > 0,
            hash % bombCode
        );
    }

    static int ConvertFromField(FieldValue value)
    {
        return value.Number +
            (value.HasBomb ? bombCode : 0) + 
            (value.HasFlag ? flagCode : 0) + 
            (value.IsOpen ? openCode : 0);
    }
}

public record FieldValue(
    bool HasBomb,
    bool HasFlag,
    bool IsOpen,
    int Number
);