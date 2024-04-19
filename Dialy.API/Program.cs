using Daily.API.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using Microsoft.OpenApi.Models;
using Daily.API.IServices;
using Daily.API.Services;
using Daily.API.Repositories;
using Daily.API.Helper;

/// <summary>
/// 项目类Program类
/// </summary>
public class Program
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const string LocalhostUrl = "http://localhost:4450";
    private const string SecureLocalhostUrl = "https://localhost:7100";

    /// <summary>
    /// 程序入口，创建WebApplication实例
    /// </summary>
    /// <param name="args">Main方法参数</param>
    public static void Main(string[] args)
    {
        ShowOrHideConsole();

        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.UseUrls(builder.Configuration["Urls:Localhost"] ?? LocalhostUrl, builder.Configuration["Urls:SecureLocalhost"] ?? SecureLocalhostUrl);

        ConfigureServices(builder);

        var app = builder.Build();

        DatabaseMigrate(app);

        ConfigureApp(app);

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }

    private static void ShowOrHideConsole()
    {
        var handle = GetConsoleWindow();
#if DEBUG
        ShowWindow(handle, 5);
#else 
        ShowWindow(handle, 0);
#endif
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(temp =>
        {
            temp.SwaggerDoc("v1", new OpenApiInfo { Title = "Daily.API", Version = "v1" });
            string path = AppContext.BaseDirectory + "Daily.API.xml";
            temp.IncludeXmlComments(path, true);
        });
        builder.Services.AddDbContext<DailyDbContext>(db => db.UseSqlite(builder.Configuration.GetConnectionString("ConStr")));

        builder.Services.AddScoped<IAccountRepoService, AccountRepoService>();
        builder.Services.AddScoped<IAccountService, AccountService>();

        builder.Services.AddScoped<IToDoService, ToDoService>();
        builder.Services.AddScoped<IToDoInfoRepoService, ToDoInfoRepoService>();

        builder.Services.AddScoped<IMemoRepoService, MemoRepoService>();
        builder.Services.AddScoped<IMemoService, MemoService>();

        builder.Services.AddScoped<IApiResponseHelper, ApiResponseHelper>();
    }

    private static void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();
        app.MapControllers();
    }

    private static void DatabaseMigrate(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<DailyDbContext>();
            context.Database.Migrate();
    }
}

