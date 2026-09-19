using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Schema;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace FineUI.Core.QuickStart
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // 运行时调用此方法；在这里把服务注册进容器。
        // 如何配置应用的更多说明见 https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddSession();

            // 配置请求参数限制
            services.Configure<FormOptions>(x =>
            {
                x.ValueCountLimit = 1024;   // 请求参数的个数限制（默认值：1024）
                x.ValueLengthLimit = 4194304;   // 单个请求参数值的长度限制（默认值：4194304 = 1024 * 1024 * 4）
            });
            
            // 本地化服务：FineUI.Core 解析模型注解（Display / Required 等）时会用到 IStringLocalizerFactory，
            // 缺了它，带 For="Movie.Xxx" 的表单页会在渲染时抛「No service for type ...IStringLocalizerFactory」
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // FineUI 服务
            services.AddFineUI(Configuration);

            // AddFineUI 已自动登记 FineUI 专属模型绑定器，以及启用 RazorForms 时所需的过滤器。
            services.AddRazorPages().AddNewtonsoftJson().AddRazorRuntimeCompilation();
			

            services.AddDbContext<MovieContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SQLServer")));
        }

        // 运行时调用此方法；在这里配置 HTTP 请求管道。
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 启动时自动应用 EF Core 迁移：库不存在则建库并跑全部迁移；
            // 库已存在则只增量应用尚未执行的迁移（幂等，不会删除或重建已有数据）。
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<MovieContext>().Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();   
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            // FineUI 中间件（确保 UseFineUI 位于 UseEndpoints 的前面）
            app.UseFineUI();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
