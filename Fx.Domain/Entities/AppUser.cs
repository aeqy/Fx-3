using Microsoft.AspNetCore.Identity;

namespace Fx.Domain.Entities;

/// <summary>
/// 用户实体类，存储用户基本信息
/// </summary>
public class AppUser : IdentityUser<Guid>
{
    // public Guid Id { get; set; }    // 用户唯一标识
    // public string UserName { get; set; }    // 用户名
    // public string Email { get; set; }    // 用户邮箱
    // public string PasswordHash { get; set; }    // 密码哈希
}