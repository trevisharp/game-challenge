public interface IGameData
{
    int Size { get; }
    void Open(int i, int j);
    int GetNumber(int i, int j);
}