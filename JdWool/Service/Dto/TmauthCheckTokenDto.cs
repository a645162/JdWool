using JdWool.Service.Enum;
using System.Text.Json.Serialization;

namespace JdWool.Service.Dto
{
    public class TmauthCheckTokenDto
    {
        [JsonPropertyName("check_ip")]
        public int CheckIp { get; set; }

        [JsonPropertyName("errcode")]
        public ErrCodeEnum ErrCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

    }
}
