﻿#region using
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using dotNET.Web.Host.Framework;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text;
using Microsoft.Extensions.Logging;
using dotNET.Core;
using NLog.Web;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using dotNET.Web.Host.Framework.Middlewares;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Mvc.Controllers;
using dotNET.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using dotNET.Application;
using System.Reflection;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
#endregion
namespace dotNET.Web.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            if (bool.Parse(Configuration["IsIdentity"]))
            {
                System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                    .AddCookie("Cookies")
                    .AddOpenIdConnect("oidc", options =>
                    {
                        options.Authority = Configuration["IdentityUrl"];
                        options.RequireHttpsMetadata = false;

                        options.ClientId = "dotNET.Mvc";
                        options.SaveTokens = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = "name",
                            RoleClaimType = "role",
                        };
                    });
            }
            else
            {
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            }

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
            });
            services.AddMemoryCache();
            services.AddOptions();
            services.Configure<SiteConfig>(Configuration.GetSection("SiteConfig"));
            services.AddLogging();
            services.AddCloudscribePagination();
            services.AddResponseCompression();
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            if (bool.Parse(Configuration["IsUseMiniProfiler"]))
            {
                services.AddMiniProfiler().AddEntityFramework();
            }

            services.AddMvc(cfg =>
            {
                cfg.Filters.Add(typeof(ExceptionAttribute));//异常捕获
            });
            services.AddMvc(cfg =>
            {
                cfg.Filters.Add(typeof(MvcMenuFilter));
            });
            services.AddHangfire(x =>
            {
                var connectionString = Configuration["Data:Redis:ConnectionString"];
                x.UseRedisStorage(connectionString, new Hangfire.Redis.RedisStorageOptions() { Db = int.Parse(Configuration["Data:Redis:Db"]) });

            });
            services.AddDbContext<EFCoreDBContext>(options => options.UseMySql(Configuration["Data:MyCat:ConnectionString"]));
            return new AutofacServiceProvider(AutofacExt.InitAutofac(services, Assembly.GetExecutingAssembly()));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ICompositeViewEngine engine)
        {
            app.UseAuthentication();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);//这是为了防止中文乱码
            loggerFactory.AddNLog();//添加NLog
            env.ConfigureNLog("nlog.config");//读取Nlog配置文件
            app.UseDefaultImage(defaultImagePath: Configuration.GetSection("defaultImagePath").Value);
            app.UseResponseCompression();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            if (bool.Parse(Configuration["IsUseMiniProfiler"]))
            {
                app.UseMiniProfiler();
            }

            app.UseStaticFiles();
            //页面的执行时间
            //app.UseExecuteTime();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "cloudscribeWebPagination",
                template: "pager/{page?}"
                , defaults: new { controller = "Paging", action = "Index" }
            );
                routes.MapRoute("areaRoute", "{area:exists}/{controller}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            var jobOptions = new BackgroundJobServerOptions
            {
                Queues = new[] { "" }//队列名称，只能为小写

            };

            app.UseHangfireServer(jobOptions);
        }
    }
}