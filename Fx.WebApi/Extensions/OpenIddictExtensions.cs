using System.Security.Cryptography.X509Certificates;
using Fx.Infrastructure.Date;
using OpenIddict.Abstractions;

namespace Fx.WebApi.Extensions;

/// <summary>
/// 扩展类, 用于配置OpenIddict
/// </summary>
public static class OpenIddictExtensions
{
    public static IServiceCollection ConfigureOpenIddict(this IServiceCollection services, X509Certificate2 certificate)
    {
        // 配置 OpenIddict
        services.AddOpenIddict()
            .AddCore(options =>
            {
                // 使用 Entity Framework 存储
                options.UseEntityFrameworkCore()
                    .UseDbContext<AppDbContext>()
                    .ReplaceDefaultEntities<Guid>(); // 使用 Guid 作为主键
            })
            .AddServer(options =>
            {
                options
                    .AllowPasswordFlow() // 允许密码流
                    .AllowAuthorizationCodeFlow() // 允许授权码流
                    .AllowRefreshTokenFlow() // 允许刷新令牌
                    .SetTokenEndpointUris("/connect/token") // 设置 Token 端点
                    .SetAuthorizationEndpointUris("/connect/authorize"); // 设置授权端点

                options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile);

                // 开发环境使用自签名证书
                // .AddDevelopmentEncryptionCertificate()
                // .AddDevelopmentSigningCertificate();

                // 自签证书, 生产环境请使用正式证书
                options.AddSigningCertificate(certificate);
                options.AddEncryptionCertificate(certificate);

                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough(); // 通过 API 直接处理请求
                // .EnableAuthorizationEndpointPassthrough();

                // 限制返回的声明
                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(30)) // 访问令牌有效期30分钟
                    .SetRefreshTokenLifetime(TimeSpan.FromDays(6)); // 刷新令牌有效期6天

                // 只包含必要的声明
                options.DisableAccessTokenEncryption(); // 如果不需要加密访问令牌，可以禁用加密

                // options.UseDataProtection(); // 使用对称加密
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        return services;
    }
}