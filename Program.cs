void ia(IGameData game)
{
    var i = Random.Shared.Next(12);
    var j = Random.Shared.Next(12);
    game.Open(i, j);
}

await MineSweeperApp.Run(args, ia);