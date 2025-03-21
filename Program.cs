void ia(IGameData game)
{
    // Exemplo de uma IA burrinha :v
    var i = Random.Shared.Next(12);
    var j = Random.Shared.Next(12);
    if (game.GetNumber(i, j) == -1)
        game.Open(i, j);
}

await MineSweeperApp.Run(args, ia);