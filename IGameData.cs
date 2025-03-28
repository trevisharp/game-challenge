public interface IGameData
{
    int Size { get; }
    int Bombs { get; }
    void Open(int i, int j);
    int GetNumber(int i, int j);
}