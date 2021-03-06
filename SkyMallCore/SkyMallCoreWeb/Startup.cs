﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkyMallCore.Core;
using SkyMallCore.Services;
namespace SkyMallCoreWeb
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json",optional:true,reloadOnChange:true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            ServiceFactory.Initialize(services, Configuration);
            AuthenticationFactory.Initialize(services);
            services.AddMvc();
            
            services.AddSession();
            CoreProviderContext.ServiceCollection = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            CoreProviderContext.HttpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();
            CoreProviderContext.Configuration = Configuration;
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //使用授权
            app.UseAuthentication();

            app.UseStaticFiles();
            //SEssion
            app.UseSession();
            //mvc路由
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute(
                  name: "SystemManage", areaName: "SystemManage",
                     template: "SystemManage/{controller=Home}/{action=Index}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                    ,defaults: new string[] { "SkyMallCoreWeb.Controllers" }
                    );

             

            });
        }
    }


    


}
