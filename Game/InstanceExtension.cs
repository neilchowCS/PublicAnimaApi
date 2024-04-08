using System.Runtime.CompilerServices;

namespace AnimaApi.Game
{
    public static class InstanceExtension
    {
        public static bool MovePiece(this GameState gameState, 
            int preX, int preY, int postX, int postY)
        {
            if ( gameState != null) { 
                if (gameState.Board[preX][preY] != null)
                {
                    if (gameState.Board[postX][postY] != null) { 
                        gameState.SwapPiece(preX, preY, postX, postY);
                    }
                    else
                    {
                        gameState.Board[preX][preY]!.X = postX;
                        gameState.Board[preX][preY]!.Y = postY;
                        gameState.Board[preX][preY] = null;
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        private static void SwapPiece(this GameState gameState, int preX, int preY, int postX, int postY)
        {
            GamePiece temp = gameState.Board[postX][postX]!;
            gameState.Board[preX][preY]!.X = postX;
            gameState.Board[preX][preY]!.Y = postY;
            gameState.Board[postX][postX] = gameState.Board[preX][preY];
            temp.X = preX;
            temp.Y = preY;
            gameState.Board[preX][preY] = temp;
        }
    }

    

}
