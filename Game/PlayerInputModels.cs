using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace AnimaApi.Game
{
    public class InputHistoryModel
    {
        public int order { get; set; }
        public List<PlayerInputModels> playerInputs { get; set; }

        

        public void execute(GameInstance instance, bool isPlayer1)
        {
            Console.WriteLine(JsonConvert.SerializeObject(this));
            foreach (PlayerInputModels input in playerInputs) {
                Console.WriteLine(JsonConvert.SerializeObject(input));
                input.execute(instance, isPlayer1); }
        }
    }

    public class PlayerInputModels
    {
        public int Id { get; set; }
        public virtual void execute(GameInstance instance, bool isPlayer1) { }
    }

    public class MovementInput(int preX, int preY, int postX, int postY) : PlayerInputModels
    {
        public int preX { get; set; } = preX;
        public int preY { get; set; } = preY;
        public int postX { get; set; } = postX;
        public int postY { get; set; } = postY;

        public int Id { get; set; } = 1;

        public override void execute(GameInstance instance, bool isPlayer1)
        {
            GameState state = instance.GetState(isPlayer1);
            Console.WriteLine(preX + " " + preY + " " + postX + " " + postY);
            if (state.Board[preX][preY] != null)
            {
                if (state.Board[postX][postY] != null)
                {
                    GamePiece temp = state.Board[postX][postY];
                    state.Board[postX][postY] = state.Board[preX][preY];
                    state.Board[preX][preY] = temp;
                    state.Board[preX][preY].X = preX;
                    state.Board[preX][preY].Y = preY;
                    state.Board[postX][postY].X = postX;
                    state.Board[postX][postY].Y = postY;
                }
                else
                {
                    state.Board[postX][postY] = state.Board[preX][preY];
                    state.Board[preX][preY] = null;
                    state.Board[postX][postY].X = postX;
                    state.Board[postX][postY].Y = postY;
                }
            }
        }

    }
}
