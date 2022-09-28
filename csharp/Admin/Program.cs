using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using NHibernate;
using NLog.Web;
using shunda.Admin.Filters;
using shunda.Admin.Middlewares;
using shunda.Core;
using shunda.Core.Binder;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Json;
using shunda.Core.Log;
using shunda.Domain;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost
            .UseUrls("http://*:9001")
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

        services.AddScoped<RequestLogActionFilter>();
        services.AddScoped<LoginTimeoutResetActionFilter>();
        services.AddScoped<HibernateActionFilter>();
        services.AddScoped<HeaderActionFilter>();
        services.AddScoped<AdminAuthActionFilter>();
        services.AddScoped<ExceptionFilter>();

        var mvcBuilder = services.AddControllersWithViews()
            .AddMvcOptions(options => {
                // 执行顺序是从上到下的
                options.Filters.AddService<RequestLogActionFilter>(); // 这个Filter执行时 sessionId 应该已经有了
                options.Filters.AddService<LoginTimeoutResetActionFilter>(); // 这个Filter执行时 sessionId 应该已经有了
                options.Filters.AddService<HibernateActionFilter>();
                options.Filters.AddService<HeaderActionFilter>();
                options.Filters.AddService<AdminAuthActionFilter>();
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

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        // 添加 Core 项目中的 service
        services.AddCoreServices();

        // 添加 Service 项目中的 service
        services.AddAppServices();

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
            // All of this is optional. You can simply call .AddMiniProfiler() for all defaults

            // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
            options.RouteBasePath = "/profiler";


            // (Optional) You can disable "Connection Open()", "Connection Close()" (and async variant) tracking.
            // (defaults to true, and connection opening/closing is tracked)
            options.TrackConnectionOpenClose = true;

            // (Optional) Use something other than the "light" color scheme.
            // (defaults to "light")
            options.ColorScheme = StackExchange.Profiling.ColorScheme.Auto;

            // The below are newer options, available in .NET Core 3.0 and above:

            // (Optional) You can disable MVC filter profiling
            // (defaults to true, and filters are profiled)
            options.EnableMvcFilterProfiling = true;
            // ...or only save filters that take over a certain millisecond duration (including their children)
            // (defaults to null, and all filters are profiled)
            // options.MvcFilterMinimumSaveMs = 1.0m;

            // (Optional) You can disable MVC view profiling
            // (defaults to true, and views are profiled)
            options.EnableMvcViewProfiling = true;
            // ...or only save views that take over a certain millisecond duration (including their children)
            // (defaults to null, and all views are profiled)
            // options.MvcViewMinimumSaveMs = 1.0m;

            // (Optional) listen to any errors that occur within MiniProfiler itself
            // options.OnInternalError = e => MyExceptionLogger(e);

            // (Optional - not recommended) You can enable a heavy debug mode with stacks and tooltips when using memory storage
            // It has a lot of overhead vs. normal profiling and should only be used with that in mind
            // (defaults to false, debug/heavy mode is off)
            //options.EnableDebugMode = true;
        });
#endif

        var app = builder.Build();

        app.UseExceptionHandler(SiteConfig.ErrorHandlerUrl);

        // app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseMiddleware<SessionIdCookieMiddleware>();
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

        // var quartz = new QuartzStartup(app.Services);
        // app.Lifetime.ApplicationStarted.Register(quartz.Start);
        // app.Lifetime.ApplicationStopping.Register(quartz.Stop);

        app.Run();
    }
}