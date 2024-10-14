using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Fx.WebApi.Controllers;
[Route("auth/connect")]
public class AuthController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;
        
    public AuthController(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    [HttpPost("authorize")]
    [Authorize]
    public async Task<IActionResult> Authorize([FromForm] string client_id, [FromForm] string redirect_uri)
    {
        // 处理授权请求，生成授权码
        // 根据 OAuth2 标准流程生成授权码返回
        return Ok();
    }

    [HttpPost("token")]
    public async Task<IActionResult> Token([FromForm] string client_id, [FromForm] string client_secret)
    {
        // 处理 Token 请求，返回 JWT
        return Ok();
    }
}