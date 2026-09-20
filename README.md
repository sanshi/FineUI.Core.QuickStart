# FineUI.Core.QuickStart

**FineUI.Core.QuickStart** 是 **FineUI（ASP.NET Core 版）** 的官方入门示例，采用 **RazorForms**
开发模式（Razor Pages 三件套 `.cshtml` + `.cshtml.cs` + `.cshtml.designer.cs`，以类似 WebForms 的
事件驱动方式写服务端逻辑），基于 **.NET 8** 构建，是一个完整的电影管理小应用（Movie CRUD）。

它演示了如何用 FineUI 快速搭建一个带增删改查、登录、主题切换的后台页面：

- **服务端控件**：数据表格、表单、按钮、下拉、消息框等 FineUI 控件的声明式用法；
- **数据访问**：用 **EF Core（Code First）** 连接 SQL Server LocalDB，完成电影数据的列表 / 新增 / 编辑 / 删除；
- **主要页面**：电影列表（`Movie`）、新增（`MovieNew`）、编辑（`MovieEdit`）、登录（`Login`）、主题切换（`Themes`）。

适合作为学习 FineUI.Core、或快速起步搭建自己项目的模板。

## 环境要求

| 依赖 | 说明 |
|------|------|
| .NET 8 SDK | 项目 `TargetFramework` 为 `net8.0` |
| SQL Server LocalDB | 随 Visual Studio 一起安装的轻量本地数据库（实例名 `MSSQLLocalDB`） |
| Visual Studio 或 `dotnet` CLI | 二选一即可运行 |

项目调用 `AddFineUI` 后，FineUI.Core 会自动登记 `JArray` / `JObject` 模型绑定器，并在启用 RazorForms 时
登记所需过滤器。应用仍需保留 `AddRazorPages().AddNewtonsoftJson()`、`UseFineUI()` 和 Razor Pages 路由。

## 依赖方式

项目文件已声明从公共软件包仓库获取的 NuGet 包 `FineUI.Core`。正常联网构建时，包管理器会自动还原依赖；仓库不包含 FineUI.Core.dll、FineUI.Pro.dll、fineui-java.jar，也不包含 FineUI 框架源码。

## 构建

安装 .NET 8 SDK 后，在仓库根目录运行：

```powershell
dotnet restore FineUI.Core.QuickStart.sln
dotnet build FineUI.Core.QuickStart.sln -c Release --no-restore
```

## LocalDB 简介与确认数据库已启动

**LocalDB** 是 SQL Server 的精简本地版，按需启动、无需安装完整 SQL Server，专为本地开发设计。
本项目连接的实例名为 `MSSQLLocalDB`。

启动应用前，先确认 LocalDB 实例在运行。打开命令行（cmd / PowerShell）执行：

```bat
:: 启动实例（若已在运行会直接返回）
sqllocaldb start MSSQLLocalDB

:: 查看实例状态
sqllocaldb info MSSQLLocalDB
```

`info` 输出里 **`State: Running`** 表示实例已就绪，例如：

```
Name:               MSSQLLocalDB
Version:            15.0.4382.1
Owner:              你的机器名\你的用户名
Auto-create:        Yes
State:              Running
Last start time:    2026/8/24 11:45:20
Instance pipe name: np:\\.\pipe\LOCALDB#XXXXXXXX\tsql\query
```

其他常用命令：

```bat
sqllocaldb info            :: 列出本机所有 LocalDB 实例
sqllocaldb stop  MSSQLLocalDB   :: 停止实例
sqllocaldb create MSSQLLocalDB  :: 若实例不存在则创建
```

## 数据库与 EF Core 说明

### 连接字符串

在 [`appsettings.json`](FineUI.Core.QuickStart/appsettings.json) 的 `ConnectionStrings:SQLServer`：

```json
"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MovieContext;Integrated Security=True;TrustServerCertificate=True"
```

- `(localdb)\MSSQLLocalDB` —— 连接的 LocalDB 实例
- `Initial Catalog=MovieContext` —— 数据库名（**首次运行时由程序自动创建，见下文**）
- `Integrated Security=True` —— 用当前 Windows 账户登录

### EF Core Code First 与 `Migrations/` 目录

本项目用 **EF Core 的 Code First** 模式：先写 C# 实体类，再由 EF 生成数据库结构。

- 实体模型：[`Code/Movie.cs`](FineUI.Core.QuickStart/Code/Movie.cs)（电影：名称/发布日期/类型/价格）
- 数据库上下文：[`Code/MovieContext.cs`](FineUI.Core.QuickStart/Code/MovieContext.cs)

**`Migrations/` 目录**保存"数据库结构的版本历史"。每次模型有变动，用 EF 工具生成一个迁移文件，
里面记录了这次变动对应的建表/改表 SQL 逻辑：

| 文件 | 作用 |
|------|------|
| `20240909140512_InitialCreate.cs` | 初始迁移：定义如何创建 `Movies` 表 |
| `MovieContextModelSnapshot.cs` | 当前模型的快照，EF 用它比对下次的模型差异 |
| `Movies.data.sql` | **示例种子数据**（几条电影记录），非 EF 自动执行，需手动导入（见「（可选）导入示例数据」） |

> 数据库里有一张 `__EFMigrationsHistory` 表，记录"哪些迁移已经跑过"，EF 据此做增量更新。

### 关键点：数据库是"启动时自动创建"的

⚠️ **EF Core 不会在首次访问数据时自动建库**（这点和老版 EF6 不同）。库的创建必须显式触发。

本项目已在 [`Startup.cs`](FineUI.Core.QuickStart/Startup.cs) 的 `Configure` 方法开头加入：

```csharp
using (var scope = app.ApplicationServices.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<MovieContext>().Database.Migrate();
}
```

它在**每次应用启动**时执行，逻辑是幂等的、**不会丢数据**：

- 库不存在 → 建库 + 应用全部迁移；
- 库已存在、迁移都跑过 → 什么都不做；
- 库已存在、但有新迁移 → 只增量应用新迁移，已有数据保留。

所以你**不需要手动建库**，直接运行项目即可。

## 运行

在仓库根目录启动（首次会先还原 NuGet 包并编译）：

```powershell
dotnet run --project FineUI.Core.QuickStart/FineUI.Core.QuickStart.csproj
```

启动后打开 <http://localhost:52419/> —— 地址来自 `FineUI.Core.QuickStart/Properties/launchSettings.json` 里的 `FineUI.Core.QuickStart` 配置。

也可以用 Visual Studio 打开 `FineUI.Core.QuickStart.sln`：

- 按 F5 / Ctrl+F5 默认走上面那个 `FineUI.Core.QuickStart` 配置；
- 想用 IIS Express，就在工具栏把启动配置切成 `IIS Express`，地址是 **http://localhost:52424/**。

端口被占用时，改 `Properties/launchSettings.json` 里对应配置的 `applicationUrl` 即可。

首次运行时，程序会自动在 LocalDB 上创建 `MovieContext` 数据库和 `Movies` 表。

**不需要授权文件**：本仓库引用的是公共 NuGet 包 `FineUI.Core`（社区版），社区版不做授权校验，克隆下来就能直接跑。

## （可选）导入示例数据

`Migrations/InitialCreate` 只建**空表**。若想看到示例电影数据，手动执行一次种子脚本：

```bat
sqlcmd -S "(localdb)\MSSQLLocalDB" -d MovieContext -i FineUI.Core.QuickStart\Migrations\Movies.data.sql
```

（或在 VS 的 SQL Server 对象资源管理器 / SSMS 里打开 `Movies.data.sql` 执行。）

## 常见问题

**运行报错：`transient failure ... EnableRetryOnFailure`（EF Core 连接异常）**

这多半不是"网络抖动"，真正原因通常是**数据库还没创建 / LocalDB 没启动**：

1. 先按「LocalDB 简介与确认数据库已启动」确认 `sqllocaldb info MSSQLLocalDB` 显示 `State: Running`；
2. 本项目已配置启动时自动 `Database.Migrate()` 建库，正常情况下运行即可自愈；
3. 若仍报错，检查 `appsettings.json` 的连接字符串实例名是否与 `sqllocaldb info` 里一致。

**如何重置数据库？**

删掉库后重新运行项目即可自动重建：

```bat
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DROP DATABASE MovieContext"
```

## 许可边界

本仓库中由合肥三生石上软件有限公司拥有著作权的示例或应用项目源代码采用 [MIT 许可证](LICENSE)。FineUI 各端框架源码、二进制软件包、内嵌的 FineUI.js 运行时以及 FineUI 名称、标识和商标不属于 MIT 授权范围，仍适用各自的商业或社区版许可。具体边界见 [NOTICE.md](NOTICE.md)。

## 参与贡献

请先阅读 `CONTRIBUTING.md`。安全问题请按 `SECURITY.md` 私下报告。
