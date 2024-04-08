namespace AnimaApi.Game
{
    public class GamePiece(GameInstance instance, int pieceId, bool isPlayer1,
        int x, int y, int energy)
    {
        public GameInstance Instance { get; set; } = instance;
        public int Id { get; set; } = pieceId;
        public bool IsPlayer1 { get; set; } = isPlayer1;
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public int Energy { get; set; } = energy;
    }
}
