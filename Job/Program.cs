using System.Reflection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using NHibernate;
using NLog.Web;
using shunda.Core;
using shunda.Core.Binder;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Hibernate.MiniProfilerMySql;
using shunda.Core.Json;
using shunda.Core.Log;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Job.Filters;
using shunda.Job.Middlewares;
using shunda.Service;
using StackExchange.Profiling;
using ISession = NHibernate.ISession;

namespace shunda.Job;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost
            .UseUrls("http://*:9002")
            .UseKestrel(opt => {
                // https://q.cnblogs.com/q/109103/  Reading the request body timed out due to data arriving too slowly. See MinRequestBodyDataRate.
                opt.Limits.MinRequestBodyDataRate = null;
                opt.Limits.MinResponseDataRate = null;
            });

        NLogConfiguration.Config();
        builder.Logging.AddNLogWeb();
        NHibernateLogger.SetLoggersFactory(new HibernateLoggerFactory());

        // Add services to the container.
        var services = builder.Services;

        services.AddScoped<HibernateActionFilter>();
        services.AddScoped<HeaderActionFilter>();
        services.AddScoped<ExceptionFilter>();

        var mvcBuilder = services.AddControllersWithViews()
            .AddMvcOptions(options => {
                // 执行顺序是从上到下的
                options.Filters.AddService<HibernateActionFilter>();
                options.Filters.AddService<HeaderActionFilter>();
                options.Filters.AddService<ExceptionFilter>();

                // 数据绑定相关
                options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(name => $"{name}为必填项！");
                options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, name) => $"{value}不是合法的{name}的值！");
                options.ModelBinderProviders.Insert(0, new IntModelBinderProvider());
                options.ModelBinderProviders.Insert(0, new UIntModelBinderProvider());
                options.ModelBinderProviders.Insert(0, new DateTimeFormatModelBinderProvider());
            })
            // .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = new AppJsonNamingPolicy(); }) 改用 NewtonsoftJson 后，这里实际已经不起作用
            .AddNewtonsoftJson(options => {
                // options.SerializerSettings.ContractResolver = new DefaultContractResolver(); // 这个ContractResolver【不会】改变属性大小写
                // options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); // 这个ContractResolver【会】改变属性大小写
                options.SerializerSettings.ContractResolver = new NHibernateContractResolver(); // 当前基于 CamelCasePropertyNamesContractResolver 实现
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm";
            });

#if DEBUG
        mvcBuilder.AddRazorRuntimeCompilation();
#endif
        services.Configure<FormOptions>(options => {
            options.ValueCountLimit = HttpFormConfig.ValueCountLimit;
            options.ValueLengthLimit = HttpFormConfig.ValueLengthLimit;
        });

        services.Configure<CookiePolicyOptions>(options => {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => false;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddHttpClient();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // 添加 Core 项目中的 service
        services.AddCoreServices();

        // 添加 Service 项目中的 service
        services.AddAppServices();

        // 添加 Job 项目中的 Service
        Assembly assembly = typeof(QuartzStartup).Assembly;
        ServiceUtil.AddAssemblyService(services, assembly);

        // 必须 Singleton
        services.AddSingleton<ISessionFactory>((provider) =>
            HibernateSessionFactory.BuildSessionFactory(DataBaseConfig.DataBaseType.MySQL, (new DomainMock()).GetType().Assembly)
        );

        // 必须 Scoped 
        services.AddScoped<ISession>((provider) => {
                // ISessionFactory sessionFactory = provider.GetService<ISessionFactory>()!;
                // return sessionFactory.OpenSession();
                return HibernateSessionFactory.OpenSession(provider.GetService<ISessionFactory>()!);
            }
        );

#if DEBUG
        services.AddMiniProfiler(options => {
            options.RouteBasePath = "/profiler"; // /profiler/results-index   /profiler/results
            options.TrackConnectionOpenClose = true;
            options.ColorScheme = ColorScheme.Auto;
            options.EnableMvcFilterProfiling = true;
            options.EnableMvcViewProfiling = true;
            options.SqlFormatter = new MiniProfilerMySqlFormatter();
        });
#endif

        var app = builder.Build();

        app.UseExceptionHandler(SiteConfig.JobErrorHandlerUrl);

        // app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseMiddleware<SessionIdMiddleware>();
        app.UseMiddleware<RequestLogMiddleware>(); // 还是放在有了 sessionId 后放心点
        app.UseMiddleware<HibernateMiddleware>();

#if DEBUG
        app.UseMiniProfiler();
#endif

        app.UseEndpoints(endpoints => {
            endpoints.MapControllerRoute(
                name: "defaultWithArea",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // endpoints.MapHub<HandleHub>("/hub/handle");
        });

        var quartz = new QuartzStartup(app.Services, app.Environment.IsDevelopment());
        app.Lifetime.ApplicationStarted.Register(quartz.Start);
        app.Lifetime.ApplicationStopping.Register(quartz.Stop);

        app.Run();
    }
}