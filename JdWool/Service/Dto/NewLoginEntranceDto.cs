using System.Text.Json.Serialization;

namespace JdWool.Service.Dto
{
    public class NewLoginEntranceDto
    {
        [JsonPropertyNameAttribute("s_token")]
        public string SToken { get; set; }
    }
}
