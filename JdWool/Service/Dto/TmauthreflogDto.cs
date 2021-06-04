using System.Text.Json.Serialization;

namespace JdWool.Service.Dto
{
    public class TmauthreflogDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("okl_token")]
        public string OkToken { get; set; }
    }
}
