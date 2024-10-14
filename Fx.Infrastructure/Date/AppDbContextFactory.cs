using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Fx.Infrastructure.Date;

// 实现 IDesignTimeDbContextFactory 接口
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // // 配置应用根目录读取 appsettings.json 文件
        // IConfigurationRoot configuration = new ConfigurationBuilder()
        //     .SetBasePath(Directory.GetCurrentDirectory())
        //     .AddJsonFile("appsettings.json")
        //     .Build();

        // 获取当前目录的上级目录（解决 Web API 和 Infrastructure 项目不同目录的问题）
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Fx.WebApi");

        // 读取 Web API 项目的 appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath) // 指定 Web API 项目目录
            .AddJsonFile("appsettings.json") // 加载 appsettings.json
            .Build();

        // 读取 PostgreSQL 连接字符串
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 配置 DbContextOptions 使用 PostgreSQL
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // 返回配置后的 AppDbContext 实例
        return new AppDbContext(optionsBuilder.Options);
    }
}