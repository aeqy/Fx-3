using System.Security.Cryptography.X509Certificates;
using Fx.Application.Interfaces;
using Fx.Application.Services;
using Fx.Domain.Entities;
using Fx.Infrastructure.Date;
using Fx.WebApi.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// 配置日志记录
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 设置 OpenIddict 日志级别为 Debug
builder.Logging.AddFilter("OpenIddict", LogLevel.Debug);


// 加载配置文件
// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true); 
var certificate = new X509Certificate2(
    builder.Configuration["Certificate:Path"] ?? "certificate.pfx",
    builder.Configuration["Certificate:Password"]);

// 配置数据库和 Identity
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict<Guid>(); // 使用 OpenIddict
});

// 配置 Identity
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 使用扩展方法来配置 OpenIddict
builder.Services.ConfigureOpenIddict(certificate);

// 配置 JWT 验证
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7214"; // 配置 Authority
        options.Audience = "api";
        options.RequireHttpsMetadata = false; // 开发环境禁用 https 验证
    });

// 注入业务服务, 依赖注入 IAccountService
builder.Services.AddScoped<IAccountService, AccountService>();

// 注入种子数据服务
builder.Services.AddScoped<ISeedDataService, SeedDataService>();


builder.Services.AddControllers();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseSwaggerUI(c =>
    // {
    //     c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    //     c.RoutePrefix = "swagger"; // 使Swagger UI 在根路径加载
    //
    //     c.EnableDeepLinking(); // 开启深色模式支持
    //
    //     c.OAuthClientId("your-client-id"); // 配置 OAuth2 客户端ID
    //     c.OAuthAppName("My API - Swagger");
    //     c.OAuthUsePkce(); // 启用 PKCE 支持
    //
    //     c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion
    //         .List); // 折叠所有文档, List：按列表展示Full：默认展开所有 API。None：默认折叠所有 API。
    //
    //     c.DefaultModelExpandDepth(2); // 设置模型展开的深度
    //     c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model); // 渲染模型还是 schema
    //
    //     c.EnableTryItOutByDefault(); // 默认启用 "Try it out" 功能
    //
    //     // c.InjectStylesheet("/swagger-ui/custom.css"); // 注入自定义的 CSS 样式
    //
    //     c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put); // 限制显示的提交方法
    // });
}

// 执行种子数据
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<AppDbContext>();
    var seedDataService = services.GetRequiredService<ISeedDataService>();

    try
    {
        // 应用数据库迁移
        dbContext.Database.Migrate();

        // 调用种子数据服务
        await seedDataService.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection(); // 先重定向

// app.UseStaticFiles(); //先加载静态文件
app.UseRouting(); // 先注册路由
app.UseAuthentication(); //先验证身份
app.UseAuthorization(); //后验证权限

app.MapControllers(); //顶级路由注册

app.Run();