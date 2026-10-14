# FineUI.Core.QuickStart

**FineUI.Core.QuickStart** 是 **FineUI（ASP.NET Core 版）** 的官方入门示例，采用 **RazorForms**
开发模式（Razor Pages 三件套 `.cshtml` + `.cshtml.cs` + `.cshtml.designer.cs`，以类似 WebForms 的
事件驱动方式写服务端逻辑），基于 **.NET 8** 构建，是一个完整的电影管理小应用（Movie CRUD）。

它演示了如何用 FineUI 快速搭建一个带增删改查、登录、主题切换的后台页面：

- **服务端控件**：数据表格、表单、按钮、下拉、消息框等 FineUI 控件的声明式用法；
- **数据访问**：用 **EF Core（Code First）** 连接 SQL Server LocalDB，完成电影数据的列表 / 新增 / 编辑 / 删除；
- **主要页面**：电影列表（`Movie`）、新增（`MovieNew`）、编辑（`MovieEdit`）、登录（`Login`）、主题切换（`Themes`）。

适合作为学习 FineUI.Core、或快速起步搭建自己项目的模板。

<!-- fineui-community:start -->
## 加入 FineUI 社区

欢迎[加入 FineUI 社区](https://fineui.com/fans/)，一站式获取社区版、完整示例、空项目、快速入门和 AppBox 等配套资源，及时了解版本更新，交流控件用法与项目实践。**FineUI.Core、FineUI.Pro 与 FineUI.Java 社区版均可永久免费商用**，欢迎一起分享经验、讨论问题。
<!-- fineui-community:end -->

## 环境要求

| 依赖 | 说明 |
|------|------|
| .NET 8 SDK | 项目 `TargetFramework` 为 `net8.0` |
| SQL Server LocalDB | 随 Visual Studio 一起安装的轻量本地数据库（实例名 `MSSQLLocalDB`） |
| Visual Studio 或 `dotnet` CLI | 二选一即可运行 |

项目调用 `AddFineUI` 后，FineUI.Core 会自动登记 `JArray` / `JObject` 模型绑定器，并在启用 RazorForms 时
登记所需过滤器。应用仍需保留 `AddRazorPages().AddNewtonsoftJson()`、`UseFineUI()` 和 Razor Pages 路由。

## 依赖方式

项目文件已声明从公共软件包仓库获取的 NuGet 包 `FineUI.Core`。正常联网构建时，包管理器会自动还原依赖。

## 生成设计时文件

安装 Node.js 后，双击仓库根目录的 `生成并监听设计时文件.bat`。脚本会先扫描页面，再持续监听 `.cshtml` 与对应后台文件的改动；按回车或关闭窗口即可停止。

只有页面声明了可生成字段的 FineUI 控件时，才会生成 `.cshtml.designer.cs`；没有控件字段的页面不会生成空的设计时文件。工具会清理已不再需要的自动生成文件，保留无法确认来源或包含手写成员的文件。

只检查文件是否需要生成、更新或清理，可在仓库根目录运行 `.\生成并监听设计时文件.bat --check`；只生成一次而不监听，可运行 `node tools/generate-designers.mjs`。

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

## 页面配置

全站默认值放在 `appsettings.json` 的 `FineUI` 节。每用户的主题 Cookie 由应用的 `AppPageManagerInitializer.Initialize(pm, request)` 读取；页面基类在 GET 处理器之前调用它。

RazorForms 单页的安全策略放在每轮都会执行的 Razor 页面开头或公共服务端初始化中，并在第一个控件声明前确定。回发会重跑模板，但不会重跑 `OnGet`，因此不能只在 `OnGet` 设置这些配置：

```cshtml
@{
    var pm = PageManager.Instance;
    pm.EnableWatermark = true;
    pm.WatermarkText = "I❤︎FineUI";
}
```

配置顺序是“全站默认 → 应用公共初始化 → 单页设置 → 控件声明”。布局保留 `@F.PageManager`、样式与脚本输出；依赖控件字段的首次数据绑定放在 `Page_Load` 的 `!IsPostBack` 分支。主题 Cookie 初始化只用于 GET；CSP 安全策略及当前声明需每轮从服务器取得。

## RazorForms 升级与严格 CSP

本项目默认开启严格脚本 CSP：`CspScripts`、`CspScriptsAllowSelf`、`CspScriptsAllowNonce` 为 `true`，`CspScriptsAllowUnsafeInline`、`CspScriptsAllowUnsafeEval` 为 `false`。可信 Razor 模板脚本由框架添加每请求 nonce；回发动作使用结构化命令或具名函数。Examples 中实际加载第三方 HtmlEditor 的页面通过 `BaseThirdPartyEditorModel` 使用本页例外，打开编辑窗口的外层页面保持严格策略。

RazorForms 默认使用 JSON 首屏配置和 JSON 回发，普通控件事件回发到当前完整 URL，保留原查询参数。无需添加新的架构启用属性。MVC、RazorPages 以及明确指定网址的上传、下载、自定义 JSON 入口继续使用各自的调用方式。

升级时一起更新 FineUI.Core 包、DLL 和配套客户端资源，并刷新已经打开的页面。旧页签不能继续使用原脚本响应协议；保存结果未知时先核查业务数据，再决定是否重新提交。回滚也必须同时恢复服务器和客户端资源并刷新页面。

普通策略下，原来的 `RegisterStartupScript(Window1.GetMaximizeReference())` 和字符串消息回调仍兼容。开启严格 CSP 时，这些写法须迁为控件即时辅助方法、`PageContext.Execute`、具名函数或命令回调。例如回发中可调用 `Window1.Maximize()`；消息确认后的动作可以写成：

```csharp
new Alert
{
    Message = "保存成功！",
    OkCommand = ActiveWindow.CreateHidePostBackCommand("saved")
}.Show();
```

执行页面函数使用 `PageContext.InvokeFunction("app.afterSave", data)`，参数是 JSON 数据。不要把脚本字符串或 `Get...Reference()` 的返回值当作函数名。首屏初始化用控件声明属性或模板中的具名函数；控件即时辅助方法保留仅在回发生效的边界。

`RendererFunction="renderSalary(2)"` 这样的 JSON 工厂参数可以保留；内联函数、脚本参数、树节点 `OnClientClick`、HTML `onclick` 和 `javascript:` 网址须迁移。可信 Razor 模板内的普通脚本可以使用页面 nonce；通过 `Html.Raw` 或用户内容注入的脚本不能自动获得授权。第三方组件需要放宽策略时应说明具体原因，不能把所有页面都视为严格 CSP 示例。

**不兼容提醒**：AJAX 的 `complete`、`success` 和全局成功钩子仍接收原始响应文本，但 RazorForms 文本已是 JSON。`AuthenticationTimeout` 等通过查找 `window.location.href` 判断跳转的代码必须改为 `JSON.parse(data)` 后检查 `commands` 中的 `page.redirect`，不会提供旧脚本正文模拟。

RazorForms 每轮回发真正重跑当前 Razor（含布局与分部模板）取声明，丢弃 HTML，不执行 OnGet 或 Page_Load；随后合并当前值，再运行一次 Page_Load 和可信事件。完整控件/事件声明不在浏览器或整页 Session 中保存。业务实际修改的有限属性与浏览器当前值继续累计，空串、null、false、0、空集合不会被模板初值覆盖。

把首次数据装载从 `DataSource="@Model.GetRows()"` 移到后台：

```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        Grid1.DataSource = GetRows();
        Grid1.DataBind();
    }
}
```

未迁移的无副作用查询通常只多执行一次；只在 OnGet 初始化的对象、一次性资源或有副作用的模板表达式可能失败，必须调整。固定选项、页面安全策略等声明每轮从服务器取得；不要覆盖原生绑定已收到的输入模型。动态新增控件在回发 Page_Load 中为已恢复控件重新登记事件，不接受浏览器方法名。当前固定工厂不支持任意自定义派生控件，使用前须核查。

条件/循环影响匿名位置时使用稳定业务 ID。无名或重名的表格命令改变文字、参数后，须使用唯一 CommandName 或在每轮 Page_Load 重设。由数据绑定生成且没有对应模板项的富 HTML Display，以及动态 HTML 数据字段/格式化配置，每轮在服务器重设。普通策略的旧确认/网址表达式可显式放在模板或每轮后台登记，不能作为浏览器当前文字往返。

编辑器工具栏等参数使用 `OptionItem.JsonValue`：`new OptionItem { Key = "toolbar", JsonValue = new JArray { new JArray("bold", "italic") } }`，模板可写 `JsonValue="@Model.ToolbarOptions"`。布尔使用 JValue，显式 JSON null 使用 JValue.CreateNull()。原 Value/PersistOriginal 仍兼容；字符串即使长得像 JSON，也不会自动改成结构化参数。第三方编辑器自身的 CSP 要求须另外核查。

安装 Node.js 后，在仓库根目录运行 `node tools/check-razorforms-upgrade.mjs`，可定位“协议升级必须核查”“严格 CSP 时迁移”和“人工核查”三类候选；加 `--json` 输出清单。扫描只读源码，不自动重写；原生 `?handler=...` 与 DataSource 是核查项，不能批量删除。

## Core 三种模式的 JSON 与页面模式

RazorForms、MVC 和 RazorPages 共用 JSON 首屏与 UI 响应解释器。MVC/RazorPages 保留 Action、OnPost、参数绑定及 `UIHelper.Result()`，使用无状态子集，不重跑模板，也不上传 RF 控件状态和事件身份字段。请求仍使用 URL 编码表单或 multipart，不要求改成 JSON body；独立业务 JSON 使用明确的数据响应通道。

官方项目显式开启严格内容安全策略（Content Security Policy，CSP）：允许同源脚本与每请求 nonce，禁止 unsafe-inline 和 unsafe-eval。三种模式未显式配置 unsafe-eval 时都默认禁止字符串执行。RazorForms 直接调用已有控件的动作方法，原生模式继续使用 UIHelper；共同操作可调用 `PageContext.InvokeFunction` 或结构化 ClientCommand。

应用级 `EnableRazorForms` 配置保留。全局开启 RF 时，特定原生 FineUI 页面在模型类或共同基类上添加可继承的 `[NoRazorForms]`；PageManager 的旧单页模式属性及对应标签、流式设置已废弃，设置时会报告迁移错误。普通独立上传、下载或 JSON OnPost 接口不要求此标记，不带 RF 专属字段时自然进入宿主处理器。

服务器与客户端资源一起升级，已经打开的旧页签应刷新后使用。首屏不增加数字协议版本，也不发送固定模式标记或签名模式凭据。

## 许可边界

本仓库中由合肥三生石上软件有限公司拥有著作权的示例或应用项目源代码采用 [MIT 许可证](LICENSE)。FineUI 各端框架源码、二进制软件包、内嵌的 FineUI.js 运行时以及 FineUI 名称、标识和商标不属于 MIT 授权范围，仍适用各自的商业或社区版许可。具体边界见 [NOTICE.md](NOTICE.md)。

## 参与贡献

请先阅读 `CONTRIBUTING.md`。安全问题请按 `SECURITY.md` 私下报告。
