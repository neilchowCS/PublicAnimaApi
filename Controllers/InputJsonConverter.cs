using AnimaApi.Game;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace AnimaApi.Controllers
{
    public class PlayerInputModelsConverter : JsonConverter<PlayerInputModels>
    {
        public override PlayerInputModels ReadJson(JsonReader reader, Type objectType, PlayerInputModels existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            int id = obj["Id"].Value<int>();

            switch (id)
            {
                case 1:
                    return obj.ToObject<MovementInput>();
                default:
                    throw new NotSupportedException("Unsupported input type.");
            }
        }

        public override void WriteJson(JsonWriter writer, PlayerInputModels value, JsonSerializer serializer)
        {
            this.WriteJson(writer, value, serializer);
        }
    }
}
