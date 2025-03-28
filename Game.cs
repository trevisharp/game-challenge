public class Game : IGameData
{
    const int length = 12;
    const int bombCode = 0b1_0000;
    const int flagCode = 0b10_0000;
    const int openCode = 0b100_0000;

    readonly int[] board = new int[length * length];
    
    public bool GameOver { get; set; } = false;

    public bool Win { get; set; } = false;

    public int Size => length;

    private int bombsCount;
    public int Bombs => bombsCount;

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
        Win = false;
        GameOver = false;
        this.bombsCount = bombsCount;
        var bombs = FillList(bombsCount, board.Length);
        InitBoardValues(bombs);
        RandomSafeClick();
    }

    public void Open(int i, int j)
    {
        if (GameOver)
            return;
        
        OpenTile(i, j);

        ValidateWin();
    }

    public int GetNumber(int i, int j)
    {
        if (i is < 0 or >= length)
            throw new Exception($"A coluna escolhida foi {i}, mas os únicos válidos são de 0 a {length}");
            
        if (j is < 0 or >= length)
            throw new Exception($"A linha escolhida foi {j}, mas os únicos válidos são de 0 a {length}");
        
        var field = this[i, j];
        if (field.IsOpen)
            return field.Number;
        
        return -1;
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

    void OpenTile(int i, int j)
    {
        var field = this[i, j];
        if (field.IsOpen)
            return;
        
        this[i, j] = field with { IsOpen = true };
        
        if (field.HasBomb)
        {
            LoseGame();
            return;
        }
        
        if (field.Number > 0)
            return;

        int x0 = int.Max(i - 1, 0), xf = int.Min(i + 1, length - 1);
        int y0 = int.Max(j - 1, 0), yf = int.Min(j + 1, length - 1);
        for (int x = x0; x <= xf; x++)
            for (int y = y0; y <= yf; y++)
                OpenTile(x, y);
    }

    void ValidateWin()
    {
        for (int i = 0; i < board.Length; i++)
        {
            var field = this[i];
            if (field.IsOpen ^ field.HasBomb)
                continue;
            
            return;
        }

        GameOver = true;
        Win = true;
    }

    void LoseGame()
    {
        OpenAllBombs();
        GameOver = true;
        Win = false;
    }

    void OpenAllBombs()
    {
        for (int i = 0; i < board.Length; i++)
        {
            var field = this[i];
            if (!field.HasBomb)
                continue;
            
            this[i] = field with { IsOpen = true };
        }
    }

    static int DiscoverValue(int pos, List<int> bombs)
    {
        int count = 0;

        List<int> deltas = [];

        if (pos % length != 0)
        {
            if (pos / length != 0)
                deltas.Add(-length - 1);
            
            deltas.Add(-1);
            
            if (pos / length != length - 1)
                deltas.Add(length - 1);
        }

        if (pos % length != length - 1)
        {
            if (pos / length != 0)
                deltas.Add(-length + 1);
            
            deltas.Add(1);
            
            if (pos / length != length - 1)
                deltas.Add(length + 1);
        }

        if (pos / length != 0)
            deltas.Add(-length);
        
        if (pos / length != length - 1)
            deltas.Add(length);
        
        foreach (var delta in deltas)
        {
            if (bombs.Contains(pos + delta))
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