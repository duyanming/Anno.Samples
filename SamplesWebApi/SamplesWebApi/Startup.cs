using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anno.Rpc.Client;
using Anno.Rpc.Client.DynamicProxy;
using SamplesWebApi.Contract;

namespace SamplesWebApi
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
            #region 注册接口代理
           /* var helloAnnoService = AnnoProxyBuilder.GetService<IHelloAnnoService>();
            services.AddSingleton(helloAnnoService);


            var currentLimitingService = AnnoProxyBuilder.GetService<ICurrentLimitingService>();
            services.AddSingleton(currentLimitingService);

           
            var nonstandardService = AnnoProxyBuilder.GetService<INonstandardService>();
            services.AddSingleton(nonstandardService);*/

            AnnoProxyBuilder.InjectAnnoContainer(System.Reflection.Assembly.GetExecutingAssembly(),services);
            #endregion

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SamplesWebApi",
                    Version = "v1",
                    Contact = new OpenApiContact()
                    {
                        Name = "Anno&Viper",
                        Email = "dym880@163.com",
                        Url = new Uri("https://github.com/duyanming")
                    },
                    Description = "Anno&Viper 对接asp.net core WebApi",
                    License = new OpenApiLicense()
                    {
                        Name = "MIT",
                        Url=new Uri("https://github.com/duyanming/Viper/blob/master/LICENSE")
                    }
                }
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SamplesWebApi v1");
                });
            }

            #region Anno配置中心设置
            DefaultConfigManager
                  .SetDefaultConfiguration(_annoConfig.AppName
                      , _annoConfig.IpAddress
                      , _annoConfig.Port
                      , _annoConfig.TraceOnOff); 
            #endregion

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

            });
        }
    }
}
