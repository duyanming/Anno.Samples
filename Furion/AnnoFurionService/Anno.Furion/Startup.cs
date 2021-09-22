using Anno.Rpc.Client;
using Anno.Rpc.Client.DynamicProxy;
using Furion.Application.AnnoContract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anno.Furion
{
    public class Startup
    {
        readonly AnnoConfig _annoConfig = new AnnoConfig();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 注册配置
            Configuration.GetSection("Anno").Bind(_annoConfig);
            services.AddSingleton(_annoConfig);
            #endregion

            services.AddControllers()
                    .AddInject();

            //Anno动态接口
            var helloAnnoService = AnnoProxyBuilder.GetService<IHelloAnnoService>();
            services.AddSingleton(helloAnnoService);

            var currentLimitingService = AnnoProxyBuilder.GetService<ICurrentLimitingService>();
            services.AddSingleton(currentLimitingService);

            var productAnnoService = AnnoProxyBuilder.GetService<IProductAnnoService>();
            services.AddSingleton(productAnnoService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            DefaultConfigManager
               .SetDefaultConfiguration(_annoConfig.AppName
                   , _annoConfig.IpAddress
                   , _annoConfig.Port
                   , _annoConfig.TraceOnOff);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // 添加这一行，如果是 MVC和API共存项目，无需添加 string.Empty
            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
