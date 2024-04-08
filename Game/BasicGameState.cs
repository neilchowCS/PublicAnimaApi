namespace AnimaApi.Game
{
    public class BasicGameState(List<int> gamePieceId, List<int> posX, List<int> posY)
    {
        public List<int> gamePieceId { get; set; } = gamePieceId;
        public List<int> posX { get; set; } = posX;
        public List<int> posY { get; set; } = posY;
    }
}
