namespace AnimaApi.Game
{
    public class ClientBattleResponse(GameInstance instance)
    {
        public bool isPlayer1 { get; set; } = true;

        //public List<BattleEvent> eventList;
        public List<int> gamePieceId1 { get; set; } = instance.State1.Pieces.Select(s => s.Id).ToList();
        public List<int> posX1 { get; set; } = instance.State1.Pieces.Select(s => s.X).ToList();
        public List<int> posY1 { get; set; } = instance.State1.Pieces.Select(s => s.Y).ToList();
        public List<int> energy1 { get; set; } = instance.State1.Pieces.Select(s => s.Energy).ToList();

        public List<int> gamePieceId2 { get; set; } = instance.State2.Pieces.Select(s => s.Id).ToList();
        public List<int> posX2 { get; set; } = instance.State2.Pieces.Select(s => s.X).ToList();
        public List<int> posY2 { get; set; } = instance.State2.Pieces.Select(s => s.Y).ToList();
        public List<int> energy2 { get; set; } = instance.State2.Pieces.Select(s => s.Energy).ToList();
    }
}
