using System.Security.Claims;
using Fx.Domain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Fx.WebApi.Controllers
{
    [ApiController]
    [Route("connect")]
    public class TokenController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
       
        // 构造函数，依赖注入 UserManager, SignInManager 和 OpenIddictTokenManager
        public TokenController(
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            
        }

        /// <summary>
        /// 处理 Token 请求 (密码模式)
        /// </summary>
        /// <returns>返回访问令牌或错误信息</returns>
        [HttpPost("token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            // 获取 OpenIddict 服务器的请求
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                // 请求不合法，返回错误
                return BadRequest(new { error = "Invalid request." });
            }

            // 检查是否为密码模式
            if (request.IsPasswordGrantType())
            {
                // 确保用户名和密码不为空
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest(new { error = "Invalid username or password." });
                }

                // 根据用户名查找用户
                var user = await _userManager.FindByNameAsync(request.Username);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    // 用户不存在或密码不匹配，返回错误
                    return BadRequest(new { error = "Invalid username or password." });
                }

                // 检查用户的邮箱是否已确认
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    return BadRequest(new { error = "Email not confirmed." });
                }

                // 检查用户是否被锁定
                if (await _userManager.IsLockedOutAsync(user))
                {
                    return BadRequest(new { error = "Account is locked." });
                }

                // 创建基于声明的身份标识 (ClaimsIdentity)
                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                // 添加用户 ID, 邮箱, 和用户名到声明
                identity.AddClaim(Claims.Subject, user.Id.ToString(), Destinations.AccessToken);
                identity.AddClaim(Claims.Email, user.Email, Destinations.AccessToken);
                identity.AddClaim(Claims.Name, user.UserName, Destinations.AccessToken);

                // 创建一个声明的主体 (ClaimsPrincipal) 并设置访问令牌的作用域
                var principal = new ClaimsPrincipal(identity);
                principal.SetScopes(new[]
                {
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.OfflineAccess // 支持刷新令牌
                });

                // 通过 SignInAsync 方法签发访问令牌
                await HttpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, principal);
                
                // 返回令牌信息
                return Ok(new
                {
                    access_token = "Your token will be automatically managed by OpenIddict."
                });
            }

            // 如果不是密码模式，返回错误
            return BadRequest(new { error = "Unsupported grant type." });
        }
    }
}
