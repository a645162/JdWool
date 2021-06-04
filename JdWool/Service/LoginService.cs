using JdWool.Service.Dto;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using JdWool.Service.Enum;

namespace JdWool.Service
{
    public class LoginService
    {

        private readonly string NewLoginEntranceUrl = "https://plogin.m.jd.com/cgi-bin/mm/new_login_entrance?lang=chs&appid=300";

        private readonly string TmauthreflogUrl = "https://plogin.m.jd.com/cgi-bin/m/tmauthreflogurl?lang=chs&appid=300&s_token={0}&remember=true";

        private readonly string TmauthUrl = "https://plogin.m.jd.com/cgi-bin/m/tmauth?appid=300&client_type=m&token={0}";

        private readonly string TmauthCheckToken = "https://plogin.m.jd.com/cgi-bin/m/tmauthchecktoken?lang=chs&appid=300&returnurl=&token={0}&ou_state=0&okl_token={1}";


        private readonly DictionaryCache _dictionaryCache;
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginService(DictionaryCache dictionaryCache, IHttpClientFactory httpClientFactory)
        {
            this._dictionaryCache = dictionaryCache;
            this._httpClientFactory = httpClientFactory;
        }

        public async Task<(Guid, string)> GetTmauthAsync()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("Jd");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://plogin.m.jd.com/login/login?appid=300");

            HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, NewLoginEntranceUrl);

            var result = await httpClient.SendAsync(httpRequestMessage);
            var newLoginEntranceDto = await result.Content.ReadFromJsonAsync<NewLoginEntranceDto>();

            httpRequestMessage = new(HttpMethod.Get, string.Format(TmauthreflogUrl, newLoginEntranceDto.SToken));
            Parallel.ForEach(result.Headers, item =>
            {
                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            });
            result = await httpClient.SendAsync(httpRequestMessage);
            var tmauthreflogDto = await result.Content.ReadFromJsonAsync<TmauthreflogDto>();
            var oklToken = result.Headers.FirstOrDefault(x => x.Key == "Set-Cookie").Value
                .FirstOrDefault(x => x.StartsWith("okl_token")).Split(';')[0].Split('=')[1];
            tmauthreflogDto.OkToken = oklToken;

            var msg = string.Format(TmauthUrl, tmauthreflogDto.Token);

            var id = Guid.NewGuid();
            _dictionaryCache.SetValue(id.ToString(), JsonSerializer.Serialize(tmauthreflogDto));

            return (id, msg);
        }

        public async Task<(ErrCodeEnum, string)> GetCookieAsync(Guid id)
        {
            var tmauthreflog = _dictionaryCache.GetValue(id.ToString());
            var tmauthreflogDto = JsonSerializer.Deserialize<TmauthreflogDto>(tmauthreflog);            

            var tmauthCheckTokenUrl = string.Format(TmauthCheckToken, tmauthreflogDto.Token, tmauthreflogDto.OkToken);

            HttpClient httpClient = _httpClientFactory.CreateClient("Jd");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://plogin.m.jd.com/login/login?appid=300");

            HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, tmauthCheckTokenUrl);
            var result = await httpClient.SendAsync(httpRequestMessage);

            var tmauthCheckTokenDto = await result.Content.ReadFromJsonAsync<TmauthCheckTokenDto>();
            
            var cookie = string.Empty;

            if (tmauthCheckTokenDto.ErrCode == ErrCodeEnum.Ok)
            {
                var cookies = result.Headers.FirstOrDefault(x => x.Key == "Set-Cookie").Value
                    .Where(x => x.StartsWith("pt_key") || x.StartsWith("pt_pin"));
                cookie = string.Join(";", cookies) + ";";

                _dictionaryCache.RemoveValue(id.ToString());
            }

            return (tmauthCheckTokenDto.ErrCode, cookie);
        }
    }
}
