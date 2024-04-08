namespace AnimaApi.Game
{
    public class GameState
    {
        public List<List<GamePiece?>> Board { get; set; } = new();
        public List<GamePiece> Pieces { get; set; } = new();

        public GameState()
        {
            for (int i = 0; i < 8; i++)
            {
                Board.Add(new());
                for (int j = 0; j < 4; j++)
                {
                    Board[^1].Add(null);
                }
            }
        }

        public BasicGameState toBasicGameState()
        {
            return new BasicGameState(Pieces.Select(s => s.Id).ToList(), Pieces.Select(s => s.X).ToList(), Pieces.Select(s => s.Y).ToList());
        }

    }
}
