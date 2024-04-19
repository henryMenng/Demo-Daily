using Microsoft.EntityFrameworkCore;

namespace Daily.API.DataModel;

/// <summary>
/// Daily的数据库上下文
/// </summary>
public class DailyDbContext : DbContext
{
    /// <summary>
    /// Daily的数据库上下文的配置构造函数
    /// </summary>
    /// <param name="options"></param>
    public DailyDbContext(DbContextOptions<DailyDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 用户信息表
    /// </summary>
    public virtual DbSet<AccountInfo> AccountInfo { get; set; }

    /// <summary>
    /// 待办事项信息表
    /// </summary>
    public virtual DbSet<ToDoInfo> ToDoInfo { get; set; }

    /// <summary>
    /// 备忘录信息表
    /// </summary>
    public virtual DbSet<MemoInfo> MemoInfo { get; set; }

    /// <summary>
    /// 初始化表
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //添加管理员 账号admin 密码123 用户名 admin
        modelBuilder.Entity<AccountInfo>().HasData(
                       new AccountInfo { 
                           AccountId = 1,
                           Name = "管理员", 
                           Pwd = "202CB962AC59075B964B07152D234B70", 
                           Account = "admin" });
    }
}
