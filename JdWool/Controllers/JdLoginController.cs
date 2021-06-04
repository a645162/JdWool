using JdWool.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace JdWool.Controllers
{
    public class JdLoginController : Controller
    {
        private readonly LoginService _loginService;

        public JdLoginController(LoginService loginService)
        {
            this._loginService = loginService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTmauthAsync()
        {
            var result = await _loginService.GetTmauthAsync();

            return Ok(new
            {
                id = result.Item1,
                tmauth = result.Item2
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCookieAsync(Guid id)
        {
            var result = await _loginService.GetCookieAsync(id);

            return Ok(new
            {
                code = result.Item1,
                cookie = result.Item2
            });
        }
    }
}
