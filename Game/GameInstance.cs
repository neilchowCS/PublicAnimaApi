using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace AnimaApi.Game
{
    public class GameInstance(int id)
    {
        public int Id { get; set; } = id;
        public int User1 { get; set; } = -1;
        public int User2 { get; set; } = -1;


        public GameState? State1 { get; set; }
        public GameState? State2 { get; set; }
        
        public ConcurrentQueue<bool> turnDone1 = new ConcurrentQueue<bool>();
        public ConcurrentQueue<bool> turnDone2 = new ConcurrentQueue<bool>();
        public ConcurrentStack<DateTime> dateTimes { get; set; } = new();

        public List<InputHistoryModel> History1 { get; set; } = new();
        public List<InputHistoryModel> History2 { get; set; } = new();

        public string Response { get; set; } = "";

        public override string ToString()
        {
            string output = $"Instance {Id}: user {User1}, user {User2}";
            if (State1 != null &&  State2 != null) {
                foreach (GamePiece item in State1.Pieces)
                {
                    output += $"\\np1 piece {item.Id}";
                }
                foreach (GamePiece item in State2.Pieces)
                {
                    output += $"\\np2 piece {item.Id}";
                }
            }
            return output;
        }

        public GameState? GetState(bool isPlayer1)
        {
            return isPlayer1 ? State1 : State2;
        }

        public List<InputHistoryModel> GetHistory(bool isPlayer1)
        {
            return isPlayer1 ? History1 : History2;
        }

        public void AddHistory(InputHistoryModel? inputHistory, bool isPlayer1)
        {
            if (inputHistory == null)
            {
                return;
            }
            List<InputHistoryModel> history = GetHistory(isPlayer1);
            if (history.IsNullOrEmpty())
            {
                history.Add(inputHistory);
                Console.WriteLine("add new hist");
                Console.WriteLine(JsonConvert.SerializeObject(history[^1]));
                return;
            }
            history[^1].playerInputs = history[^1].playerInputs.Concat(inputHistory.playerInputs).ToList();
        }

        public void ExecuteTurn()
        {
            if (History1.Count > 0)
            {
                Console.WriteLine("execute hist 1");
                Console.WriteLine(JsonConvert.SerializeObject(History1[^1].playerInputs));
                History1[^1].execute(this, true);
            }
            if (History2.Count > 0)
            {
                Console.WriteLine("execute hist 2");
                Console.WriteLine(JsonConvert.SerializeObject(History2[^1].playerInputs));
                History2[^1].execute(this, false);
            }

            //FIXME

            History1.Add(new());
            History2.Add(new());
            dateTimes.Push(DateTime.UtcNow);

            this.ExecuteBattle();

            Response = JsonConvert.SerializeObject(new ClientBattleResponse(this));
        }

    }
}
