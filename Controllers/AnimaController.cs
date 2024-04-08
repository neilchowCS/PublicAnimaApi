using AnimaApi.Game;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AnimaApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AnimaController : ControllerBase
    {
        private static ConcurrentQueue<(int matchId, ManualResetEventSlim handle)> waitingPlayers = new ConcurrentQueue<(int, ManualResetEventSlim)>();

        [Authorize]
        [Route("join")]
        [HttpPost]
        public async Task<ActionResult<string>> JoinMatchmaking([FromBody] BasicGameState init)
        {
            Console.WriteLine("join");

            string userIdString = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            Console.WriteLine($"{userIdString}");
            int userId = -1;

            if (userIdString != null && !int.TryParse(userIdString, out userId) && userId > 0)
            {
                return BadRequest($"bad user id, input is {userIdString}");
            }

            if (Program.PlayerToInstance.ContainsKey(userId))
            {
                Console.WriteLine(userId);
                return BadRequest($"user {userId} already in game instance");
            }

            if (!ValidateGameState(init))
            {
                return BadRequest("bad init game state");
            }

            ClientBattleResponse response;

            // Try to dequeue and signal readiness
            if (waitingPlayers.TryDequeue(out var waitingPlayer))
            {
                SetGameState(Program.GameInstances[waitingPlayer.matchId], false, userId, init);
                response = new ClientBattleResponse(Program.GameInstances[waitingPlayer.matchId]);
                response.isPlayer1 = false;
                Program.PlayerToInstance.AddOrUpdate(userId, waitingPlayer.matchId, (key, oldValue) => waitingPlayer.matchId);

                waitingPlayer.handle.Set(); // Signal the waiting player
            }
            else
            {
                GameInstance newInstance = new(Program.InstanceCounter);
                Program.InstanceCounter++;
                SetGameState(newInstance, true, userId, init);
                newInstance.dateTimes.Push(DateTime.UtcNow);

                Program.GameInstances.AddOrUpdate(newInstance.Id, newInstance, (key, oldValue) => newInstance);
                Program.PlayerToInstance.AddOrUpdate(userId, newInstance.Id, (key, oldValue) => newInstance.Id);

                var playerEvent = new ManualResetEventSlim(false);
                waitingPlayers.Enqueue((newInstance.Id, playerEvent)); // Add self to the queue
                await playerEvent.WaitHandle.WaitOneAsync(); // Wait until another player joins
                response = new ClientBattleResponse(Program.GameInstances[waitingPlayer.matchId]);
                playerEvent.Dispose();
            }

            return Ok(response);
        }

        private bool ValidateGameState(BasicGameState gameState)
        {
            return true;
        }

        private void SetGameState(GameInstance instance, bool isPlayer1, int userId, BasicGameState basicGameState)
        {
            if (isPlayer1)
            {
                instance.User1 = userId;
                instance.State1 = new();
                for (int i = 0; i < basicGameState.gamePieceId.Count; i++)
                {
                    GamePiece temp = new(instance, basicGameState.gamePieceId[i], isPlayer1, basicGameState.posX[i], basicGameState.posY[i], 0);
                    instance.State1.Pieces.Add(temp);
                    instance.State1.Board[temp.X][temp.Y] = temp;
                }
            }
            else
            {
                instance.User2 = userId;
                instance.State2 = new();
                for (int i = 0; i < basicGameState.gamePieceId.Count; i++)
                {
                    GamePiece temp = new(instance, basicGameState.gamePieceId[i], isPlayer1, basicGameState.posX[i], basicGameState.posY[i], 0);
                    instance.State2.Pieces.Add(temp);
                    instance.State2.Board[temp.X][temp.Y] = temp;
                }
            }
        }

        // GET: api/<AnimaController>
        [HttpGet]
        [Route("getInstances")]
        public IActionResult Get()
        {
            Console.WriteLine("get");
            List<string> output = new();
            foreach (GameInstance instance in Program.GameInstances.Values)
            {
               output.Add(instance.ToString());
            }
            return Ok(output);
        }

        [HttpGet("clear")]
        public IActionResult Clear()
        {
            Program.GameInstances = new();
            Program.PlayerToInstance = new();
            return Ok();
        }
        
        /*
          * 
        
        // GET api/<AnimaController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }*/


        /*
        // PUT api/<AnimaController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AnimaController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
