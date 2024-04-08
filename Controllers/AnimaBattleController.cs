using AnimaApi.Game;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AnimaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimaBattleController : ControllerBase
    {
        //FIXME lock api request after commit turn
        // POST api/yourcontroller
        [HttpPost]
        [Authorize]
        public IActionResult UpdateHistory([FromBody] string historyJson)
        {
            //FIX ME make global
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new PlayerInputModelsConverter());

            try {
                
                string userIdString = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                int userId = -1;

                if (userIdString != null)
                {
                    if (int.TryParse(userIdString, out userId))
                    {
                        // userId is now an integer
                        // Use userId as needed

                        if (Program.PlayerToInstance.TryGetValue(userId, out var instanceId))
                        {
                            if (Program.GameInstances.TryGetValue(instanceId, out var instance))
                            {
                                InputHistoryModel? history = JsonConvert.DeserializeObject<InputHistoryModel>(historyJson, settings);
                                Console.WriteLine(JsonConvert.SerializeObject(history));
                                bool isPlayer1 = instance.User1 == userId;
                                instance.AddHistory(history, isPlayer1);
                            }
                        }

                        return Ok(); // Return 200 OK status
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return BadRequest();
        }

        [HttpGet("result")]
        [Authorize]
        public ActionResult GetTurnResult()
        {
            string userIdString = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            int userId = -1;

            if (userIdString != null)
            {
                if (int.TryParse(userIdString, out userId))
                {
                    // userId is now an integer
                    // Use userId as needed

                    if (Program.PlayerToInstance.TryGetValue(userId, out var instanceId))
                    {
                        if (Program.GameInstances.TryGetValue(instanceId, out var instance))
                        {
                            bool user1 = instance.User1 == userId;
                            if (instance.turnDone1.IsEmpty && instance.turnDone2.IsEmpty)
                            {
                                Console.WriteLine("first request");
                                instance.turnDone1.Enqueue(user1);
                                return Ok("waiting for opponent");
                            } 
                            else if (instance.turnDone2.IsEmpty)
                            {
                                instance.turnDone1.TryPeek(out bool o);


                                if (o != user1)
                                {
                                    Console.WriteLine("second request");
                                    instance.turnDone2.Enqueue(user1);

                                    //EXECUTE BATTLE HERE
                                    instance.ExecuteTurn();

                                    return Ok(instance.Response);
                                }
                                else
                                {
                                    Console.WriteLine("waiting");
                                    return Ok("waiting for opponent");
                                }
                            } else
                            {
                                Console.WriteLine("third request");
                                instance.turnDone1.Clear();
                                instance.turnDone2.Clear();
                                string t = instance.Response;
                                instance.Response = "";

                                return Ok(t);
                            }
                        }
                    }
                }
            }
            return BadRequest();
        }
    }
}
