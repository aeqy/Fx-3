using Fx.Application.Interfaces;
using Fx.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using System.Threading.Tasks;

namespace Fx.Application.Services
{
    public class SeedDataService : ISeedDataService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IOpenIddictApplicationManager _applicationManager;

        public SeedDataService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IOpenIddictApplicationManager applicationManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationManager = applicationManager;
        }

        public async Task SeedAsync()
        {
            // 1. 添加角色种子数据
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "Admin" });
            }

            if (!await _roleManager.RoleExistsAsync("User"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = "User" });
            }

            // 2. 添加管理员用户
            var adminUser = await _userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                var user = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // 3. 配置 OpenIddict 应用程序种子数据
            var existingApp = await _applicationManager.FindByClientIdAsync("my-client");

            if (existingApp == null)
            {
                // 创建新的客户端应用程序
                await _applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "my-client",
                    DisplayName = "My Client Application",
                    ClientType = OpenIddictConstants.ClientTypes.Public, // 设置 ClientType
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,         // 允许访问 Token 端点
                        OpenIddictConstants.Permissions.GrantTypes.Password,     // 允许使用密码模式
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken, // 允许使用刷新令牌
                        OpenIddictConstants.Permissions.Scopes.Email,            // 允许获取电子邮件
                        OpenIddictConstants.Permissions.Scopes.Profile           // 允许获取用户信息
                    }
                });
            }
            else
            {
                // 如果客户端已存在，更新权限
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "my-client",
                    ClientType = OpenIddictConstants.ClientTypes.Public, // 设置 ClientType
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken
                    }
                };
                await _applicationManager.UpdateAsync(existingApp, descriptor);
            }
        }
    }
}
